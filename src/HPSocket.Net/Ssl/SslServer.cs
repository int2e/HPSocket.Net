using System;

using HPSocket.Adapter;
using HPSocket.Tcp;

namespace HPSocket.Ssl
{
    /// <summary>
    /// ssl server
    /// </summary>
    /// <typeparam name="TRequestBodyType">包体解析对象类型</typeparam>
    public class SslServer<TRequestBodyType> : SslServer, ISslServer<TRequestBodyType>
    {
        public SslServer()
        {
            OnAccept += SslServer_OnAccept;
            base.OnReceive += SslServer_OnReceive;
            OnClose += SslServer_OnClose;
        }

#pragma warning disable 0067
        [Obsolete("adapter组件无需添加OnReceive事件, 请添加OnParseRequestBody事件", true)]
        public new event ServerReceiveEventHandler OnReceive;
#pragma warning restore

        /// <inheritdoc />
        public event ParseRequestBody<ITcpServer, TRequestBodyType> OnParseRequestBody;

        /// <inheritdoc />
        public DataReceiveAdapter<TRequestBodyType> DataReceiveAdapter { get; set; }

        private HandleResult SslServer_OnAccept(IServer sender, IntPtr connId, IntPtr client)
        {
            DataReceiveAdapter.OnOpen(connId);
            return HandleResult.Ok;
        }

        private HandleResult SslServer_OnReceive(IServer sender, IntPtr connId, byte[] data)
        {
            return DataReceiveAdapter.OnReceive(this, connId, data, OnParseRequestBody);
        }

        private HandleResult SslServer_OnClose(IServer sender, IntPtr connId, SocketOperation socketOperation, int errorCode)
        {
            DataReceiveAdapter.OnClose(connId);
            return HandleResult.Ok;
        }

        #region 重写父类方法

        /// <inheritdoc />
        public override bool Start()
        {
            if (DataReceiveAdapter == null || OnParseRequestBody == null)
            {
                throw new InitializationException("DataReceiveAdapter属性和OnParseRequestBody事件必须赋值");
            }

            return base.Start();
        }

        #endregion
    }


    /// <summary>
    /// ssl server
    /// </summary>
    public class SslServer : TcpServer, ISslServer
    {
        #region 保护成员

        /// <summary>
        /// 获取或设置是否初始化ssl环境
        /// </summary>
        protected bool IsInitSsl { get; set; }

        #endregion

        public SslServer()
            : base(Sdk.Tcp.Create_HP_TcpServerListener,
                Sdk.Ssl.Create_HP_SSLServer,
                Sdk.Ssl.Destroy_HP_SSLServer,
                Sdk.Tcp.Destroy_HP_TcpServerListener)
        {
        }

        protected SslServer(Sdk.CreateListenerDelegate createListenerFunction, Sdk.CreateServiceDelegate createServiceFunction, Sdk.DestroyListenerDelegate destroyServiceFunction, Sdk.DestroyListenerDelegate destroyListenerFunction)
            : base(createListenerFunction, createServiceFunction, destroyServiceFunction, destroyListenerFunction)
        {
        }

        ~SslServer() => Dispose(false);

        /// <summary>
        /// 启动服务
        /// </summary>
        /// <returns></returns>
        public override bool Start()
        {
            if (!IsInitSsl)
            {
                throw new InvalidOperationException("请先初始化ssl环境, 调用 Initialize() 方法");
            }
            return base.Start();
        }

        /// <inheritdoc />
        public SslVerifyMode VerifyMode { get; set; }

        /// <inheritdoc />
        public string PemCertFile { get; set; }

        /// <inheritdoc />
        public string PemKeyFile { get; set; }

        /// <inheritdoc />
        public string KeyPassword { get; set; }

        /// <inheritdoc />
        public string CaPemCertFileOrPath { get; set; }

        /// <inheritdoc />
        public string CipherList
        {
            get => Sdk.Ssl.HP_SSLServer_GetSSLCipherList(SenderPtr).PtrToAnsiString();
            set => Sdk.Ssl.HP_SSLServer_SetSSLCipherList(SenderPtr, value);
        }

        /// <inheritdoc />
        public bool AutoHandShake
        {
            get => Sdk.Ssl.HP_SSLServer_IsSSLAutoHandShake(SenderPtr);
            set => Sdk.Ssl.HP_SSLServer_SetSSLAutoHandShake(SenderPtr, value);
        }

        /// <inheritdoc />
        public bool Initialize(bool memory)
        {
            PemCertFile = String.IsNullOrWhiteSpace(PemCertFile) ? null : PemCertFile;
            PemKeyFile = String.IsNullOrWhiteSpace(PemKeyFile) ? null : PemKeyFile;
            KeyPassword = String.IsNullOrWhiteSpace(KeyPassword) ? null : KeyPassword;
            CaPemCertFileOrPath = String.IsNullOrWhiteSpace(CaPemCertFileOrPath) ? null : CaPemCertFileOrPath;
            IsInitSsl = memory
                ? Sdk.Ssl.HP_SSLServer_SetupSSLContextByMemory(SenderPtr, VerifyMode, PemCertFile, PemKeyFile, KeyPassword, CaPemCertFileOrPath, null)
                : Sdk.Ssl.HP_SSLServer_SetupSSLContext(SenderPtr, VerifyMode, PemCertFile, PemKeyFile, KeyPassword, CaPemCertFileOrPath, null);
#if !NET20 && !NET30 && !NET35
            SysErrorCode.Value = IsInitSsl ? 0 : Sdk.Sys.SYS_GetLastError();
#endif
            return IsInitSsl;
        }

        /// <inheritdoc />
        public void UnInitialize()
        {
            if (IsInitSsl)
            {
                Sdk.Ssl.HP_SSLServer_CleanupSSLContext(SenderPtr);
                IsInitSsl = false;
            }
        }

        /// <inheritdoc />
        public int AddContext(SslVerifyMode verifyMode, string pemCertFile, string pemKeyFile, string keyPassword, string caPemCertFileOrPath)
        {
            if (String.IsNullOrWhiteSpace(pemCertFile))
            {
                throw new ArgumentException("参数无效", pemCertFile);
            }
            if (String.IsNullOrWhiteSpace(pemKeyFile))
            {
                throw new ArgumentException("参数无效", pemKeyFile);
            }
            keyPassword = String.IsNullOrWhiteSpace(keyPassword) ? null : keyPassword;
            caPemCertFileOrPath = String.IsNullOrWhiteSpace(caPemCertFileOrPath) ? null : caPemCertFileOrPath;

            var result = Sdk.Ssl.HP_SSLServer_AddSSLContext(SenderPtr, verifyMode, pemCertFile, pemKeyFile, keyPassword, caPemCertFileOrPath);
#if !NET20 && !NET30 && !NET35
            SysErrorCode.Value = result > 0 ? 0 : Sdk.Sys.SYS_GetLastError();
#endif
            return result;
        }

        /// <inheritdoc />
        public int AddContextByMemory(SslVerifyMode verifyMode, string pemCert, string pemKey, string keyPassword = null, string caPemCert = null) => Sdk.Ssl.HP_SSLServer_AddSSLContextByMemory(SenderPtr, verifyMode, pemCert, pemKey, keyPassword, caPemCert);

        /// <inheritdoc />
        public bool BindServerName(string serverName, int contextIndex) => Sdk.Ssl.HP_SSLServer_BindSSLServerName(SenderPtr, serverName, contextIndex);

        /// <inheritdoc />
        public bool StartHandShake(IntPtr connId) => Sdk.Ssl.HP_SSLServer_StartSSLHandShake(SenderPtr, connId);

        /// <inheritdoc />
        public bool GetSessionInfo(IntPtr connId, SslSessionInfo info, out IntPtr sessionInfo)
        {
            sessionInfo = IntPtr.Zero;
            var ok = Sdk.Ssl.HP_SSLServer_GetSSLSessionInfo(SenderPtr, connId, info, ref sessionInfo);
#if !NET20 && !NET30 && !NET35
            SysErrorCode.Value = ok ? 0 : Sdk.Sys.SYS_GetLastError();
#endif
            return ok;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // 清理托管资源
            }

            UnInitialize();

            base.Dispose(disposing);
        }
    }
}
