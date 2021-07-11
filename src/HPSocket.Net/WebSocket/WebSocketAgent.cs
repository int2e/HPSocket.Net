using HPSocket.Http;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
#if !NET20 && !NET30 && !NET35
using System.Threading.Tasks;
#endif

namespace HPSocket.WebSocket
{
    public class WebSocketAgent : IWebSocketAgent
    {
        #region 私有成员

        private bool _disposed;
        private readonly HttpAgent _httpAgent;
        private SslConfiguration _sslConfiguration;
        private readonly ExtraData<IntPtr, WebSocketSession> _sessions = new ExtraData<IntPtr, WebSocketSession>();
        #endregion

        #region 公有成员

        /// <inheritdoc />
        public object Tag { get; set; }

        /// <inheritdoc />
        public string BindAddress { get; set; } = "0.0.0.0";

        /// <inheritdoc />
        public bool IgnoreCompressionExtensions { get; set; }

        /// <inheritdoc />
        public IHttpMultiId Http => _httpAgent;

        /// <inheritdoc />
        public Uri Uri { get; }

        /// <inheritdoc />
        public bool IsSecure { get; }

        /// <inheritdoc />
        public uint MaxPacketSize { get; set; } = 0;

        /// <inheritdoc />
        public IntPtr SenderPtr => Http.SenderPtr;

        /// <inheritdoc />
        public string Version => Sdk.Sys.GetVersion();

#if !NET20 && !NET30 && !NET35
        /// <inheritdoc />
        public ThreadLocal<int> SysErrorCode => Http.SysErrorCode;
#endif

        /// <inheritdoc />
        public byte[] DefaultMask { get; set; } = new byte[] { 0x01, 0x002, 0x3, 0x04 };

        /// <inheritdoc />
        public string UserAgent { get; set; } = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/78.0.3904.97 Safari/537.36";

        /// <inheritdoc />
        public string Cookie { get; set; }

        /// <inheritdoc />
        public List<NameValue> RequestHeaders { get; set; }

        /// <inheritdoc />
        public int ConnectionTimeout { get => _httpAgent.ConnectionTimeout; set => _httpAgent.ConnectionTimeout = value; }

        /// <inheritdoc />
        public bool HasStarted => _httpAgent.HasStarted;

        /// <inheritdoc />
        public event MessageEventHandler OnMessage;

        /// <inheritdoc />
        public event OpenEventHandler OnOpen;

        /// <inheritdoc />
        public event CloseEventHandler OnClose;

        /// <inheritdoc />
        public event PingEventHandler OnPing;

        /// <inheritdoc />
        public event PongEventHandler OnPong;

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
                if (!(_httpAgent is IHttpsAgent httpsAgent)) return;

                httpsAgent.VerifyMode = _sslConfiguration.VerifyMode;
                httpsAgent.PemCertFile = _sslConfiguration.PemCertFile;
                httpsAgent.PemKeyFile = _sslConfiguration.PemKeyFile;
                httpsAgent.KeyPassword = _sslConfiguration.KeyPassword;
                httpsAgent.CaPemCertFileOrPath = _sslConfiguration.CaPemCertFileOrPath;
                if (!httpsAgent.Initialize(_sslConfiguration.FromMemory))
                {
                    throw new WebSocketException(httpsAgent.ErrorCode, httpsAgent.ErrorMessage);
                }
            }
        }

        /// <inheritdoc />
        public string SubProtocols { get; set; } = "";

        #endregion

        #region 公有方法

        /// <summary>
        /// 创建websocket客户端实例
        /// </summary>
        /// <param name="url">协议地址, 例如ws://127.0.0.1:8080/chat或wss://127.0.0.1:8080/chat</param>
        /// <param name="protocols">支持的子协议</param>
        public WebSocketAgent(string url, params string[] protocols)
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

            if (protocols?.Length > 0)
            {
                SubProtocols = string.Join(", ", protocols);
            }

            IsSecure = Uri.Scheme == "wss";
            _httpAgent = IsSecure ? new HttpsAgent() : new HttpAgent();
            _httpAgent.Async = true;
            _httpAgent.OnUpgrade += OnHttpAgentUpgrade;
            _httpAgent.OnWsMessageHeader += OnWsMessageHeader;
            _httpAgent.OnWsMessageBody += OnWsMessageBody;
            _httpAgent.OnWsMessageComplete += OnWsMessageComplete;
            _httpAgent.OnClose += OnHttpAgentClose;
            _httpAgent.OnHandShake += OnHttpsAgentHandShake;
        }

        ~WebSocketAgent() => Dispose(false);

        /// <inheritdoc />
        public bool Start()
        {
            if (IsSecure && SslConfiguration == null)
            {
                throw new WebSocketException("wss必须设置SslConfiguration属性的值");
            }

            _httpAgent.Address = BindAddress;

            return _httpAgent.Start();
        }

        /// <inheritdoc />
        public bool Stop()
        {
            return _httpAgent.Stop();
        }

        /// <inheritdoc />
        public bool Wait(int milliseconds = -1) => _httpAgent.Wait(milliseconds);

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
        public void Connect()
        {
            if (!_httpAgent.Connect(Uri.Host, (ushort)Uri.Port))
            {
#if !NET20 && !NET30 && !NET35
                throw new WebSocketException($"sys error code: {SysErrorCode.Value}");
#else
                throw new WebSocketException($"sys error code: {Sdk.Sys.SYS_GetLastError()}");
#endif

            }
        }

        /// <inheritdoc />
        public bool Send(IntPtr connId, bool final, OpCode opCode, byte[] mask, byte[] data, int length)
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

            var ok = _httpAgent.SendWsMessage(connId, final, length == 0 ? Rsv.Off : session.Rsv, opCode, mask, data, length);
            return ok;
        }


        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="final"></param>
        /// <param name="opCode"></param>
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public bool Send(IntPtr connId, bool final, OpCode opCode, byte[] data, int length)
        {
            return Send(connId, final, opCode, DefaultMask, data, length);
        }

        /// <inheritdoc />
        public bool Send(IntPtr connId, OpCode opCode, byte[] data, int length)
        {
            return Send(connId, true, opCode, DefaultMask, data, length);
        }

        /// <inheritdoc />
        public bool Text(IntPtr connId, string text)
        {
            var data = Encoding.UTF8.GetBytes(text);
            return Send(connId, true, OpCode.Text, DefaultMask, data, data.Length);
        }

        /// <inheritdoc />
        public void Ping(IntPtr connId, byte[] data, int length)
        {
            Send(connId, true, OpCode.Ping, DefaultMask, data, length);
        }

        /// <inheritdoc />
        public void Pong(IntPtr connId, byte[] data, int length)
        {
            Send(connId, true, OpCode.Pong, DefaultMask, data, length);
        }

        /// <inheritdoc />
        public void Close(IntPtr connId)
        {
            Send(connId, true, OpCode.Close, DefaultMask, null, 0);
            _httpAgent.Disconnect(connId);
        }

        /// <inheritdoc />
        public List<IntPtr> GetAllConnectionIds() => _httpAgent.GetAllConnectionIds();

        #endregion

        #region httpAgent组件对象回调

        // ReSharper disable once InconsistentNaming
        private HandleResult OnHttpsAgentHandShake(IAgent sender, IntPtr connId)
        {
            var headers = new List<NameValue>
            {
                new NameValue
                {
                    Name = "User-Agent",
                    Value = UserAgent,
                },
                new NameValue
                {
                    Name = "Upgrade",
                    Value = "websocket",
                },
                new NameValue
                {
                    Name = "Connection",
                    Value = "Upgrade",
                },
                new NameValue
                {
                    Name = "Sec-WebSocket-Version",
                    Value = "13",
                },
                new NameValue
                {
                    Name = "Sec-WebSocket-Key",
                    Value = "".GetRandomWebSocketKey(),
                },
            };

            // cookie
            if (!String.IsNullOrWhiteSpace(Cookie))
            {
                headers.Insert(1, new NameValue
                {
                    Name = "Cookie",
                    Value = Cookie,
                });
            }

            if (!string.IsNullOrEmpty(SubProtocols))
            {
                headers.Add(new NameValue
                {
                    Name = "Sec-WebSocket-Protocol",
                    Value = SubProtocols,
                });
            }

            if (!IgnoreCompressionExtensions)
            {
                headers.Add(new NameValue
                {
                    Name = "Sec-WebSocket-Extensions",
                    Value = $"{CompressionMethod.Deflate.ToExtensionString("server_no_context_takeover", "client_no_context_takeover")}"
                });
            }

            // 附加请求头
            if (RequestHeaders != null && RequestHeaders.Count > 0)
            {
                headers.AddRange(RequestHeaders);
            }

            var ok = _httpAgent.SendRequest(connId, HttpMethod.Get, Uri.PathAndQuery, headers);
            return ok ? HandleResult.Ok : HandleResult.Error;
        }


        // ReSharper disable once InconsistentNaming
        private HttpParseResult OnHttpAgentUpgrade(IHttp sender, IntPtr connId, HttpUpgradeType upgradeType)
        {
            if (upgradeType == HttpUpgradeType.WebSocket)
            {
                var compressionMethod = IgnoreCompressionExtensions ? CompressionMethod.None : CompressionMethod.Deflate;

                _sessions.Set(connId, new WebSocketSession
                {
                    Compression = compressionMethod,
                    Final = true,
                    Mask = DefaultMask,
                    OpCode = OpCode.Text,
                    Rsv = compressionMethod == CompressionMethod.None ? Rsv.Off : Rsv.Compression,
                    Path = Uri.PathAndQuery,
                });

                OnOpen?.Invoke(this, connId);
            }
            return HttpParseResult.Ok;
        }

        // ReSharper disable once InconsistentNaming
        private HandleResult OnWsMessageHeader(IHttp sender, IntPtr connId, bool final, Rsv rsv, OpCode opCode, byte[] mask, ulong bodyLength)
        {
            var session = _sessions.Get(connId);
            if (session == null)
            {
                return HandleResult.Error;
            }

            session.OpCode = opCode;
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

            switch (session.OpCode)
            {
                case OpCode.Ping:
                    Pong(connId, data, data.Length);
                    OnPing?.Invoke(this, connId, data);
                    return HandleResult.Ok;
                case OpCode.Pong:
                    OnPong?.Invoke(this, connId, data);
                    return HandleResult.Ok;
            }

            return OnMessage?.Invoke(this, connId, session.Final, session.OpCode, session.Mask, data) ?? HandleResult.Ignore;
        }

        // ReSharper disable once InconsistentNaming
        private HandleResult OnHttpAgentClose(IAgent sender, IntPtr connId, SocketOperation socketOperation, int errorCode)
        {
            var session = _sessions.Get(connId);
            if (session != null)
            {
                session.Data = null;
                _sessions.Remove(connId);

            }

            return OnClose?.Invoke(this, connId, socketOperation, errorCode) ?? HandleResult.Ignore;
        }

        #endregion

        #region 释放资源

        /// <summary>
        /// 释放资源
        /// </summary>
        private void Destroy()
        {
            _httpAgent?.Dispose();
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
