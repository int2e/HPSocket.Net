using System;
using HPSocket.Ssl;

namespace HPSocket.Http
{
    public class HttpsClient : HttpClient, IHttpsClient
    {
        #region 保护成员

        /// <summary>
        /// 获取或设置是否初始化ssl环境
        /// </summary>
        protected bool IsInitSsl { get; set; }

        #endregion

        public HttpsClient()
            : base(Sdk.Http.Create_HP_HttpClientListener,
                Sdk.Http.Create_HP_HttpsClient,
                Sdk.Http.Destroy_HP_HttpsClient,
                Sdk.Http.Destroy_HP_HttpClientListener)
        {
            OnProxyConnected += HttpClientOnProxyConnected;
        }

        protected HttpsClient(Sdk.CreateListenerDelegate createListenerFunction, Sdk.CreateServiceDelegate createServiceFunction, Sdk.DestroyListenerDelegate destroyServiceFunction, Sdk.DestroyListenerDelegate destroyListenerFunction)
            : base(createListenerFunction, createServiceFunction, destroyServiceFunction, destroyListenerFunction)
        {
            OnProxyConnected += HttpClientOnProxyConnected;
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
            get => Sdk.Ssl.HP_SSLClient_GetSSLCipherList(SenderPtr).PtrToAnsiString();
            set => Sdk.Ssl.HP_SSLClient_SetSSLCipherList(SenderPtr, value);
        }

        /// <inheritdoc />
        public bool AutoHandShake
        {
            get => Sdk.Ssl.HP_SSLClient_IsSSLAutoHandShake(SenderPtr);
            set => Sdk.Ssl.HP_SSLClient_SetSSLAutoHandShake(SenderPtr, value);
        }

        /// <inheritdoc />
        public bool Initialize(bool memory)
        {
            PemCertFile = String.IsNullOrWhiteSpace(PemCertFile) ? null : PemCertFile;
            PemKeyFile = String.IsNullOrWhiteSpace(PemKeyFile) ? null : PemKeyFile;
            KeyPassword = String.IsNullOrWhiteSpace(KeyPassword) ? null : KeyPassword;
            CaPemCertFileOrPath = String.IsNullOrWhiteSpace(CaPemCertFileOrPath) ? null : CaPemCertFileOrPath;
            IsInitSsl = memory
                ? Sdk.Ssl.HP_SSLClient_SetupSSLContextByMemory(SenderPtr, VerifyMode, PemCertFile, PemKeyFile, KeyPassword, CaPemCertFileOrPath)
                : Sdk.Ssl.HP_SSLClient_SetupSSLContext(SenderPtr, VerifyMode, PemCertFile, PemKeyFile, KeyPassword, CaPemCertFileOrPath);
            return IsInitSsl;
        }

        /// <inheritdoc />
        public void UnInitialize()
        {
            if (IsInitSsl)
            {
                Sdk.Ssl.HP_SSLClient_CleanupSSLContext(SenderPtr);
                IsInitSsl = false;
            }
        }

        /// <inheritdoc />
        public bool StartHandShake() => Sdk.Ssl.HP_SSLClient_StartSSLHandShake(SenderPtr);

        /// <inheritdoc />
        public bool GetSessionInfo(SslSessionInfo info, out IntPtr sessionInfo)
        {
            sessionInfo = IntPtr.Zero;
            return Sdk.Ssl.HP_SSLClient_GetSSLSessionInfo(SenderPtr, info, ref sessionInfo);
        }

        /// <inheritdoc />
        public override bool Connect()
        {
            if (_proxyList?.Count > 0)
            {
                AutoHandShake = false;
                HttpAutoStart = false;
            }

            return base.Connect();
        }

        /// <inheritdoc />
        public override bool Connect(string address, ushort port)
        {
            if (_proxyList?.Count > 0)
            {
                AutoHandShake = false;
                HttpAutoStart = false;
            }

            return base.Connect(address, port);
        }

        protected override void HttpClientOnProxyConnected(IClient sender, IProxy proxy)
        {
            StartHandShake();
            base.HttpClientOnProxyConnected(sender, proxy);
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
