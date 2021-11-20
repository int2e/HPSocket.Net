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
using System.Threading;

namespace HPSocket.WebSocket
{
    public class WebSocketServer : IWebSocketServer
    {
        #region 私有成员

        private bool _disposed;
        private readonly IHttpServer _httpServer;
        private SslConfiguration _sslConfiguration;
        private readonly ExtraData<IntPtr, WebSocketSession> _wsSessions = new ExtraData<IntPtr, WebSocketSession>();
        private readonly ExtraData<IntPtr, HttpSession> _httpSessions = new ExtraData<IntPtr, HttpSession>();
        private readonly ExtraData<string, IHub> _services = new ExtraData<string, IHub>();

        #endregion

        #region 公有成员

        /// <inheritdoc />
        public object Tag { get; set; }

        /// <inheritdoc />
        public IHttpMultiId Http => _httpServer;

        /// <inheritdoc />
        public Uri Uri { get; }

        /// <inheritdoc />
        public bool IsSecure { get; }

        /// <inheritdoc />
        public bool IgnoreCompressionExtensions { get; set; } = false;

        /// <inheritdoc />
        public uint PingInterval { get; set; }

        /// <inheritdoc />
        public IntPtr SenderPtr => Http.SenderPtr;

        /// <inheritdoc />
        public string Version => Sdk.Sys.GetVersion();

#if !NET20 && !NET30 && !NET35
        /// <inheritdoc />
        public ThreadLocal<int> SysErrorCode => Http.SysErrorCode;
#endif

        /// <inheritdoc />
        public uint MaxPacketSize { get; set; } = 0;

        /// <inheritdoc />
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

        /// <inheritdoc />
        public string SubProtocols { get; set; } = "";

        #endregion

        #region 公有方法

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
            _httpServer.OnHeadersComplete += OnHttpServerHeadersComplete;
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

        /// <inheritdoc />
        public bool Start()
        {
            if (IsSecure && SslConfiguration == null)
            {
                throw new WebSocketException("wss必须设置SslConfiguration属性的值");
            }

            _httpServer.Address = Uri.Host;
            _httpServer.Port = (ushort)Uri.Port;

            return _httpServer.Start();
        }

        /// <inheritdoc />
        public bool Stop()
        {
            return _httpServer.Stop();
        }

        /// <inheritdoc />
        public bool Wait(int milliseconds = -1) => _httpServer.Wait(milliseconds);

#if !NET20 && !NET30 && !NET35
        /// <inheritdoc />
        public Task<bool> WaitAsync(int milliseconds = -1)
        {
            return Task.Factory.StartNew((obj) => Wait((int)obj), milliseconds);
        }

        /// <inheritdoc />
        public Task<bool> StopAsync()
        {
            return Task.Factory.StartNew(Stop);
        }
#endif

        /// <inheritdoc />
        public bool Send(IntPtr connId, bool final, OpCode opCode, byte[] data, int length)
        {
            var session = _wsSessions.Get(connId);
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

        /// <inheritdoc />
        public bool Send(IntPtr connId, OpCode opCode, byte[] data, int length)
        {
            return Send(connId, true, opCode, data, length);
        }

        /// <inheritdoc />
        public bool Text(IntPtr connId, string text)
        {
            var data = Encoding.UTF8.GetBytes(text);
            return Send(connId, OpCode.Text, data, data.Length);
        }

        /// <inheritdoc />
        public void Ping(IntPtr connId, byte[] data, int length)
        {
            Send(connId, OpCode.Ping, data, length);
        }

        /// <inheritdoc />
        public void Pong(IntPtr connId, byte[] data, int length)
        {
            if (data?.Length == 0)
            {
                data = null;
            }
            Send(connId, OpCode.Pong, data, length);
        }

        /// <inheritdoc />
        public void Close(IntPtr connId)
        {
            Send(connId, OpCode.Close, null, 0);

            // 延迟释放
            _httpServer.Release(connId);
        }

        /// <inheritdoc />
        public List<IntPtr> GetAllConnectionIds() => _httpServer.GetAllConnectionIds();

        /// <inheritdoc />
        public bool HasStarted => _httpServer.HasStarted;

        /// <inheritdoc />
        public string GetSubProtocol(IntPtr connId)
        {
            var session = _wsSessions.Get(connId);
            return session != null ? session.SecWebSocketProtocol : "";
        }

        /// <inheritdoc />
        public HttpSession GetHttpSession(IntPtr connId)
        {
            return _httpSessions.Get(connId);
        }

        /// <inheritdoc />
        public void AddHub<THub>(string path) where THub : IHub, new()
        {
            if (String.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("参数不能为空", nameof(path));
            }

            if (!_services.Set(path, new THub()))
            {
                throw new WebSocketException("绑定失败");
            }
        }

        /// <inheritdoc />
        public void AddHub<THub>(string path, THub obj) where THub : IHub
        {
            if (String.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("参数不能为空", nameof(path));
            }

            if (obj == null)
            {
                throw new ArgumentException("参数不能为null", nameof(obj));
            }

            if (!_services.Set(path, obj))
            {
                throw new WebSocketException("绑定失败");
            }
        }

        /// <inheritdoc />
        public THub GetHub<THub>(string path) where THub : IHub
        {
            if (String.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("参数不能为空", nameof(path));
            }

            var obj = _services.Get(path);
            if (obj is THub hub)
            {
                return hub;
            }
            return default;
        }

        /// <inheritdoc />
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

        private HttpParseResultEx OnHttpServerHeadersComplete(IHttp sender, IntPtr connId)
        {
            var session = _httpSessions.Get(connId);
            if (session == null)
            {
                session = new HttpSession();
                _httpSessions.Set(connId, session);
            }

            var httpServer = (IHttpServer)sender;
            session.Cookies = httpServer.GetAllCookies(connId);
            session.Headers = httpServer.GetAllHeaders(connId);
            session.QueryString = httpServer.GetUrlField(connId, HttpUrlField.QueryString);
            return HttpParseResultEx.Ok;
        }

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

                using (var sha1 = SHA1.Create())
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
                        var subProtocols = SubProtocols.Split(new[] { ',', ' ' });
                        var headerProtocols = secWebSocketProtocol.Split(new[] { ',', ' ' });
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

                var hub = _services.Get(path);
                if (hub == null)
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
                    ok = _wsSessions.Set(connId, session);
                    if (ok)
                    {
                        ok = hub.OnOpen(this, connId) != HandleResult.Error;
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

            var session = _wsSessions.Get(connId);
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
            var session = _wsSessions.Get(connId);
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
            var session = _wsSessions.Get(connId);
            if (session?.Data == null)
            {
                return HandleResult.Error;
            }

            var data = session.Data.ToArray();
            if (data.Length > 0 && session.Compression != CompressionMethod.None && session.Rsv == Rsv.Compression)
            {
                data = data.Decompress(session.Compression);
            }

            var hub = _services.Get(session.Path);

            switch (session.OpCode)
            {
                case OpCode.Ping:
                    Pong(connId, data, data.Length);
                    hub.OnPing(this, connId, data);
                    return HandleResult.Ok;
                case OpCode.Pong:
                    if (PingInterval == 0)
                    {
                        Ping(connId, null, 0);
                    }
                    hub.OnPong(this, connId, data);
                    return HandleResult.Ok;
            }

            return hub.OnMessage(this, connId, session.Final, session.OpCode, session.Mask, data);
        }

        // ReSharper disable once InconsistentNaming
        private HandleResult OnHttpServerClose(IServer sender, IntPtr connId, SocketOperation socketOperation, int errorCode)
        {
            _httpSessions.Remove(connId);

            var session = _wsSessions.Get(connId);
            if (session == null)
            {
                return HandleResult.Error;
            }

            session.Data = null;
            _wsSessions.Remove(connId);

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
                _wsSessions.Clear();
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
