#region license
/*
 *
 * 部分代码和编码风格来自 websocket-sharp (https://github.com/sta/websocket-sharp)
 *
 */
#endregion
using System;
using System.Collections.Generic;
#if !NET20 && !NET30 && !NET35
using System.Linq;
using System.Threading.Tasks;
#endif
using System.Security.Cryptography;
using System.Text;
using HPSocket.Http;
using Timer = System.Timers.Timer;

namespace HPSocket.WebSocket
{
    public class WebSocketServer : IWebSocketServer
    {
        #region 私有成员

        private bool _disposed;
        private readonly IHttpServer _httpServer;
        private SslConfiguration _sslConfiguration;
        private readonly ExtraData<IntPtr, WebSocketSession> _sessions = new ExtraData<IntPtr, WebSocketSession>();
        private readonly ExtraData<string, IHub> _services = new ExtraData<string, IHub>();

        #endregion

        #region 公有成员

        /// <summary>
        /// 开放式 http/https server 对象, 对 http 连接有 cookie 或者 header 操作, 直接调用这个对象操作
        /// </summary>
        public IHttpMultiId Http => _httpServer;

        /// <summary>
        /// Uri
        /// </summary>
        public Uri Uri { get; }

        /// <summary>
        /// 是否安全连接
        /// </summary>
        public bool IsSecure { get; }

        /// <summary>
        /// 忽略压缩扩展, 默认false
        /// <para>如果忽略, 则不支持压缩解压缩</para>
        /// </summary>
        public bool IgnoreCompressionExtensions { get; set; } = false;

        /// <summary>
        /// 自动发送ping消息的时间间隔
        /// <para>毫秒，0不自动发送，默认不发送（多数分机房的防火墙都在1分钟检测空连接，超时无交互则被踢，如果间隔过长，可能被机房防火墙误杀）</para>
        /// <para>目前浏览器都不支持在客户端发送ping消息，所以一般在服务器发送ping，在客户端响应接收到ping消息之后再对服务器发送pong，或客户端主动pong，服务器响应pong再发送ping给客户端</para>
        /// </summary>
        public uint PingInterval { get; set; }

        /// <summary>
        /// 组件对象指针
        /// </summary>
        public IntPtr SenderPtr => Http.SenderPtr;

        /// <summary>
        /// 当前组件版本
        /// </summary>
        public string Version => Sdk.Sys.GetVersion();

        /// <summary>
        /// 等待通信组件停止运行
        /// <para>可用在控制台程序, 用来阻塞主线程, 防止程序退出</para>
        /// </summary>
        /// <param name="milliseconds">超时时间（毫秒，默认：-1，永不超时）</param>
        public bool Wait(int milliseconds = -1) => _httpServer.Wait(milliseconds);

#if !NET20 && !NET30 && !NET35
        /// <summary>
        /// 等待通信组件停止运行
        /// <para>可用在控制台程序, 用来阻塞主线程, 防止程序退出</para>
        /// </summary>
        /// <param name="milliseconds">超时时间（毫秒，默认：-1，永不超时）</param>
        public Task<bool> WaitAsync(int milliseconds = -1)
        {
            return new TaskFactory().StartNew((obj) => Wait((int)obj), milliseconds);
        }
#endif

        /// <summary>
        /// 最大封包长度
        /// </summary>
        public uint MaxPacketSize { get; set; } = 0;

        /// <summary>
        /// ssl环境配置
        /// </summary>
        public SslConfiguration SslConfiguration
        {
            get => _sslConfiguration;
            set
            {
                if (!IsSecure)
                {
                    throw new WebSocketException("非安全连接无需配置ssl环境");
                }
                _sslConfiguration = value;
                if (!(_httpServer is IHttpsServer httpsServer)) return;

                httpsServer.VerifyMode = _sslConfiguration.VerifyMode;
                httpsServer.PemCertFile = _sslConfiguration.PemCertFile;
                httpsServer.PemKeyFile = _sslConfiguration.PemKeyFile;
                httpsServer.KeyPassword = _sslConfiguration.KeyPassword;
                httpsServer.CaPemCertFileOrPath = _sslConfiguration.CaPemCertFileOrPath;
                if (!httpsServer.Initialize(_sslConfiguration.FromMemory))
                {
                    throw new WebSocketException(httpsServer.ErrorCode, httpsServer.ErrorMessage);
                }
            }
        }

        /// <summary>
        /// 支持的子协议, 默认空, 不限制
        /// </summary>
        public string SubProtocols { get; set; } = "";

        #endregion

        #region 公有方法

        /// <summary>
        /// 创建 websocket server 对象
        /// </summary>
        /// <param name="url">url必须是 ws 或 wss
        /// <para>如果是 wss 则为安全连接, 必须设置 SslConfiguration 属性</para>
        /// </param>
        public WebSocketServer(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException("url");
            }

            if (!url.TryCreateWebSocketUri(out var uri, out var msg))
            {
                throw new ArgumentException(msg, nameof(url));
            }

            Uri = uri;

            IsSecure = Uri.Scheme == "wss";
            _httpServer = IsSecure ? new HttpsServer() : new HttpServer();
            _httpServer.ReleaseDelay = 3000;
            _httpServer.OnUpgrade += OnHttpServerUpgrade;
            _httpServer.OnWsMessageHeader += OnWsMessageHeader;
            _httpServer.OnWsMessageBody += OnWsMessageBody;
            _httpServer.OnWsMessageComplete += OnWsMessageComplete;
            _httpServer.OnClose += OnHttpServerClose;
            if (IsSecure)
            {
                _httpServer.OnHandShake += OnHttpsServerHandShake;
            }
        }

        ~WebSocketServer() => Dispose(false);

        /// <summary>
        /// 启动服务
        /// </summary>
        public void Start()
        {
            if (IsSecure && SslConfiguration == null)
            {
                throw new WebSocketException("wss必须设置SslConfiguration属性的值");
            }

            _httpServer.Address = Uri.Host;
            _httpServer.Port = (ushort)Uri.Port;

            if (!_httpServer.Start())
            {
                throw new WebSocketException(_httpServer.ErrorCode, _httpServer.ErrorMessage);
            }
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public void Stop()
        {
            _httpServer.Stop();
        }

        /// <summary>
        /// 尝试发送数据
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="final"></param>
        /// <param name="opCode"></param>
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public bool Send(IntPtr connId, bool final, OpCode opCode, byte[] data, int length)
        {
            var session = _sessions.Get(connId);
            if (session == null)
            {
                return false;
            }

            if (opCode.IsData() && session.Compression != CompressionMethod.None && data?.Length > 0)
            {
                data = data.Compress(session.Compression);
                length = data.Length;
            }

            return _httpServer.SendWsMessage(connId, final, length == 0 ? Rsv.Off : session.Rsv, opCode, data, length);
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="opCode"></param>
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public bool Send(IntPtr connId, OpCode opCode, byte[] data, int length)
        {
            return Send(connId, true, opCode, data, length);
        }

        /// <summary>
        /// 发送文本消息
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool Text(IntPtr connId, string text)
        {
            var data = Encoding.UTF8.GetBytes(text);
            return Send(connId, OpCode.Text, data, data.Length);
        }

        /// <summary>
        /// 发送ping消息
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="data"></param>
        /// <param name="length"></param>
        public void Ping(IntPtr connId, byte[] data, int length)
        {
            Send(connId, OpCode.Ping, data, length);
        }

        /// <summary>
        /// 发送pong消息
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="data"></param>
        /// <param name="length"></param>
        public void Pong(IntPtr connId, byte[] data, int length)
        {
            if (data?.Length == 0)
            {
                data = null;
            }
            Send(connId, OpCode.Pong, data, length);
        }

        /// <summary>
        /// 发送关闭消息同时关闭连接
        /// </summary>
        /// <param name="connId"></param>
        public void Close(IntPtr connId)
        {
            Send(connId, OpCode.Close, null, 0);

            // 延迟释放
            _httpServer.Release(connId);
        }

        /// <summary>
        /// 获取是否启动
        /// </summary>
        public bool HasStarted => _httpServer.HasStarted;

        /// <summary>
        /// 对path注册特定hub
        /// <para>例如: AddHub&lt;ChatHub&gt;("/chat")</para>
        /// </summary>
        /// <typeparam name="THubWithNew">继承自HPSocket.WebSocket.IHub或HPSocket.WebSocket.Hub的类</typeparam>
        /// <param name="path">url path</param>
        public void AddHub<THubWithNew>(string path) where THubWithNew : IHub, new()
        {
            if (!_services.Set(path, new THubWithNew()))
            {
                throw new WebSocketException("绑定失败");
            }
        }

        /// <summary>
        /// 移除已注册的hub
        /// </summary>
        /// <param name="path">url path</param>
        public void RemoveHub(string path)
        {
            _services.Remove(path);
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 处理 Sec-WebSocket-Extensions 头
        /// <remarks>抄自 websocket-sharp <see>
        ///         <cref>https://github.com/sta/websocket-sharp</cref>
        ///     </see>
        /// </remarks>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="compression"></param>
        /// <param name="extensions"></param>
        private bool ProcessSecWebSocketExtensionsClientHeader(string value, out CompressionMethod compression, out string extensions)
        {
            compression = CompressionMethod.None;
            extensions = string.Empty;
            if (value == null)
            {
                return false;
            }

            var buff = new StringBuilder(80);
            var comp = false;

            foreach (var item in value.SplitHeaderValue(','))
            {
                var extension = item.Trim();
                if (extension.Length == 0)
                    continue;

                if (!comp)
                {
                    if (extension.IsCompressionExtension(CompressionMethod.Deflate))
                    {
                        compression = CompressionMethod.Deflate;
                        buff.AppendFormat(
                            "{0}, ",
                            compression.ToExtensionString(
                                "client_no_context_takeover", "server_no_context_takeover"
                            )
                        );

                        comp = true;
                    }
                }
            }

            var len = buff.Length;
            if (len <= 2)
            {
                return false;
            }

            buff.Length = len - 2;
            extensions = buff.ToString();
            return true;
        }


        /// <summary>
        /// ping消息定时器
        /// </summary>
        /// <param name="connId"></param>
        private void PingTimer(IntPtr connId)
        {
            if (PingInterval == 0)
            {
                return;
            }
#pragma warning disable IDE0067 // 丢失范围之前释放对象
            var timer = new Timer(PingInterval)
            {
                AutoReset = true
            };
#pragma warning restore IDE0067 // 丢失范围之前释放对象

            timer.Elapsed += (sender, args) =>
            {
                if (_httpServer.IsConnected(connId))
                {
                    Ping(connId, null, 0);
                    return;
                }
                timer.Close();
                timer.Stop();
                timer.Dispose();
            };
            timer.Start();
        }

        #endregion

        #region httpserver组件对象回调


        // ReSharper disable once InconsistentNaming
        private HttpParseResult OnHttpServerUpgrade(IHttp sender, IntPtr connId, HttpUpgradeType upgradeType)
        {
            if (!(sender is IHttpServer httpServer))
            {
                return HttpParseResult.Error;
            }

            var ok = false;
            if (upgradeType == HttpUpgradeType.HttpTunnel)
            {
                ok = httpServer.SendResponse(connId, HttpStatusCode.Ok, null, null, 0);
            }
            else if (upgradeType == HttpUpgradeType.WebSocket)
            {
                const string supportedVersion = "13";
                var secWebSocketVersion = httpServer.GetHeader(connId, "Sec-WebSocket-Version").Trim();
                if (secWebSocketVersion != supportedVersion)
                {
                    if (!httpServer.SendResponse(connId, HttpStatusCode.BadRequest, new List<NameValue>
                    {
                        new NameValue
                        {
                            Name = "Sec-WebSocket-Version",
                            Value = supportedVersion
                        }
                    }, null, 0))
                    {
                        return HttpParseResult.Error;
                    }
                    // 延迟释放
                    Close(connId);
                    return HttpParseResult.Ok;
                }

                var headers = new List<NameValue>
                {
                    new NameValue { Name = "Connection", Value = "Upgrade" },
                    new NameValue { Name = "Upgrade", Value = "WebSocket" },
                };

                var keyName = "Sec-WebSocket-Key";

                var secWebSocketKey = httpServer.GetHeader(connId, keyName);
                if (string.IsNullOrEmpty(secWebSocketKey))
                {
                    return HttpParseResult.Error;
                }

                using (var sha1 = new SHA1CryptoServiceProvider())
                {
                    headers.Add(new NameValue
                    {
                        Name = "Sec-WebSocket-Accept",
                        Value = Convert.ToBase64String(sha1.ComputeHash(Encoding.UTF8.GetBytes($"{secWebSocketKey}258EAFA5-E914-47DA-95CA-C5AB0DC85B11"))),
                    });
                }

                var secWebSocketProtocol = httpServer.GetHeader(connId, "Sec-WebSocket-Protocol");
                if (!string.IsNullOrEmpty(secWebSocketProtocol))
                {
                    if (!string.IsNullOrEmpty(SubProtocols))
                    {
                        var subProtocols = SubProtocols.Split(',', ' ');
                        var headerProtocols = secWebSocketProtocol.Split(',', ' ');
#if NET20 || NET30 || NET35
                        var list = new List<string>();
                        foreach (var protocol in headerProtocols)
                        {
                            foreach (var item in subProtocols)
                            {
                                if (item == protocol)
                                {
                                    list.Add(protocol);
                                    break;
                                }
                            }
                        }
                        var arr = list.ToArray();
#else
                        var arr = subProtocols.Intersect(headerProtocols).ToArray();
#endif
                        if (arr.Length > 0)
                        {
                            // 支持的子协议
                            headers.Add(new NameValue
                            {
                                Name = "Sec-WebSocket-Protocol",
                                Value = string.Join(", ", arr),
                            });
                        }
                        else
                        {
                            // 全都不是服务器支持的子协议, 踢掉, 告诉客户端服务器支持什么子协议

                            if (!httpServer.SendResponse(connId, HttpStatusCode.BadRequest, new List<NameValue>
                            {
                                new NameValue
                                {
                                    Name = "Sec-WebSocket-Protocol",
                                    Value = SubProtocols,
                                },
                            }, null, 0))
                            {
                                return HttpParseResult.Error;
                            }
                            // 延迟释放
                            Close(connId);
                            return HttpParseResult.Ok;
                        }
                    }
                    else
                    {
                        // 原样返回
                        // TODO: 待定
                        headers.Add(new NameValue
                        {
                            Name = "Sec-WebSocket-Protocol",
                            Value = secWebSocketProtocol,
                        });
                    }
                }

                var compressionMethod = CompressionMethod.None;
                var extensions = string.Empty;
                if (!IgnoreCompressionExtensions)
                {
                    var secWebSocketExtensions = httpServer.GetHeader(connId, "Sec-WebSocket-Extensions");
                    if (!string.IsNullOrEmpty(secWebSocketExtensions)
                        && ProcessSecWebSocketExtensionsClientHeader(secWebSocketExtensions, out compressionMethod, out extensions))
                    {
                        if (!string.IsNullOrEmpty(extensions))
                        {
                            headers.Add(new NameValue
                            {
                                Name = "Sec-WebSocket-Extensions",
                                Value = extensions,
                            });
                        }
                    }
                }

                var path = _httpServer.GetUrlField(connId, HttpUrlField.Path);
                if (string.IsNullOrEmpty(path))
                {
                    path = "/";
                }

                var behavior = _services.Get(path);
                if (behavior == null)
                {
                    httpServer.SendResponse(connId, HttpStatusCode.BadRequest, headers, null, 0);
                    Close(connId);
                    return HttpParseResult.Ok;
                }

                ok = httpServer.SendResponse(connId, HttpStatusCode.SwitchingProtocols, headers, null, 0);
                if (ok)
                {
                    var session = new WebSocketSession
                    {
                        Path = path,
                        SecWebSocketKey = secWebSocketKey,
                        SecWebSocketProtocol = secWebSocketProtocol,
                        SecWebSocketExtensions = extensions,
                        Compression = compressionMethod,
                        Rsv = compressionMethod == CompressionMethod.None ? Rsv.Off : Rsv.Compression,
                    };
                    ok = _sessions.Set(connId, session);
                    if (ok)
                    {
                        ok = behavior.OnOpen(this, connId) != HandleResult.Error;
                        PingTimer(connId);
                    }
                }
            }
            return ok ? HttpParseResult.Ok : HttpParseResult.Error;
        }

        // ReSharper disable once InconsistentNaming
        private HandleResult OnWsMessageHeader(IHttp sender, IntPtr connId, bool final, Rsv rsv, OpCode opCode, byte[] mask, ulong bodyLength)
        {
            //if (!_httpServer.GetWsMessageState(connId, out var state))
            //{
            //    return HandleResult.Error;
            //}

            var session = _sessions.Get(connId);
            if (session == null)
            {
                return HandleResult.Error;
            }

            if (!_services.ContainsKey(session.Path))
            {
                return HandleResult.Error;
            }

            session.Final = final;
            session.Rsv = rsv;
            session.OpCode = opCode;
            session.Mask = mask;
            session.Data = null;
            session.Data = new List<byte>((int)bodyLength);

            switch (opCode)
            {
                case OpCode.Cont:
                case OpCode.Text:
                case OpCode.Binary:
                    if (MaxPacketSize != 0 && MaxPacketSize < bodyLength)
                    {
                        return HandleResult.Error;
                    }
                    break;
                case OpCode.Close:
                    Close(connId);
                    return HandleResult.Ok;
                case OpCode.Ping:
                case OpCode.Pong:
                    return HandleResult.Ok;
            }

            return HandleResult.Ok;
        }

        // ReSharper disable once InconsistentNaming
        private HandleResult OnWsMessageBody(IHttp sender, IntPtr connId, byte[] data)
        {
            var session = _sessions.Get(connId);
            if (session == null)
            {
                return HandleResult.Error;
            }

            if (session.OpCode == OpCode.Pong || session.OpCode == OpCode.Ping)
            {
                return HandleResult.Ok;
            }

            if (session.Data == null)
            {
                return HandleResult.Error;
            }

            session.Data.AddRange(data);

            return HandleResult.Ok;
        }

        // ReSharper disable once InconsistentNaming
        private HandleResult OnWsMessageComplete(IHttp sender, IntPtr connId)
        {
            var session = _sessions.Get(connId);
            if (session?.Data == null)
            {
                return HandleResult.Error;
            }

            var data = session.Data.ToArray();
            if (data.Length > 0 && session.Compression != CompressionMethod.None && session.Rsv == Rsv.Compression)
            {
                data = data.Decompress(session.Compression);
            }

            var behavior = _services.Get(session.Path);

            switch (session.OpCode)
            {
                case OpCode.Ping:
                    Pong(connId, data, data.Length);
                    behavior.OnPing(this, connId, data);
                    return HandleResult.Ok;
                case OpCode.Pong:
                    if (PingInterval == 0)
                    {
                        Ping(connId, null, 0);
                    }
                    behavior.OnPong(this, connId, data);
                    return HandleResult.Ok;
            }

            return behavior.OnMessage(this, connId, session.Final, session.OpCode, session.Mask, data);
        }

        // ReSharper disable once InconsistentNaming
        private HandleResult OnHttpServerClose(IServer sender, IntPtr connId, SocketOperation socketOperation, int errorCode)
        {
            var session = _sessions.Get(connId);
            if (session == null)
            {
                return HandleResult.Error;
            }

            session.Data = null;
            _sessions.Remove(connId);

            return _services.Get(session.Path).OnClose(this, connId, socketOperation, errorCode);
        }

        // ReSharper disable once InconsistentNaming
        private HandleResult OnHttpsServerHandShake(IServer sender, IntPtr connId)
        {
            return HandleResult.Ok;
        }
        #endregion

        #region 释放资源

        /// <summary>
        /// 释放资源
        /// </summary>
        private void Destroy()
        {
            _httpServer?.Dispose();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                // 释放托管对象资源
                _sessions.Clear();
                _services.Clear();
            }

            Destroy();

            _disposed = true;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

    }
}
