using System;
using HPSocket.Ssl;

namespace HPSocket.Http
{
    public class HttpsEasyAgent : HttpEasyAgent, IHttpsEasyAgent
    {
        #region 保护成员

        /// <summary>
        /// 获取或设置是否初始化ssl环境
        /// </summary>
        protected bool IsInitSsl { get; set; }

        #endregion

        public HttpsEasyAgent()
           : base(Sdk.Http.Create_HP_HttpAgentListener,
               Sdk.Http.Create_HP_HttpsAgent,
               Sdk.Http.Destroy_HP_HttpsAgent,
               Sdk.Http.Destroy_HP_HttpAgentListener)
        {
        }

        protected HttpsEasyAgent(Sdk.CreateListenerDelegate createListenerFunction, Sdk.CreateServiceDelegate createServiceFunction, Sdk.DestroyListenerDelegate destroyServiceFunction, Sdk.DestroyListenerDelegate destroyListenerFunction)
            : base(createListenerFunction, createServiceFunction, destroyServiceFunction, destroyListenerFunction)
        {
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
            get => Sdk.Ssl.HP_SSLAgent_GetSSLCipherList(SenderPtr).PtrToAnsiString();
            set => Sdk.Ssl.HP_SSLAgent_SetSSLCipherList(SenderPtr, value);
        }

        /// <inheritdoc />
        public bool AutoHandShake
        {
            get => Sdk.Ssl.HP_SSLAgent_IsSSLAutoHandShake(SenderPtr);
            set => Sdk.Ssl.HP_SSLAgent_SetSSLAutoHandShake(SenderPtr, value);
        }

        /// <inheritdoc />
        public bool Initialize(bool memory)
        {
            PemCertFile = String.IsNullOrWhiteSpace(PemCertFile) ? null : PemCertFile;
            PemKeyFile = String.IsNullOrWhiteSpace(PemKeyFile) ? null : PemKeyFile;
            KeyPassword = String.IsNullOrWhiteSpace(KeyPassword) ? null : KeyPassword;
            CaPemCertFileOrPath = String.IsNullOrWhiteSpace(CaPemCertFileOrPath) ? null : CaPemCertFileOrPath;
            IsInitSsl = memory
                ? Sdk.Ssl.HP_SSLAgent_SetupSSLContextByMemory(SenderPtr, VerifyMode, PemCertFile, PemKeyFile, KeyPassword, CaPemCertFileOrPath)
                : Sdk.Ssl.HP_SSLAgent_SetupSSLContext(SenderPtr, VerifyMode, PemCertFile, PemKeyFile, KeyPassword, CaPemCertFileOrPath);
            return IsInitSsl;
        }

        /// <inheritdoc />
        public void UnInitialize()
        {
            if (IsInitSsl)
            {
                Sdk.Ssl.HP_SSLAgent_CleanupSSLContext(SenderPtr);
                IsInitSsl = false;
            }
        }

        /// <inheritdoc />
        public bool StartHandShake(IntPtr connId) => Sdk.Ssl.HP_SSLAgent_StartSSLHandShake(SenderPtr, connId);

        /// <inheritdoc />
        public bool GetSessionInfo(IntPtr connId, SslSessionInfo info, out IntPtr sessionInfo)
        {
            sessionInfo = IntPtr.Zero;
            return Sdk.Ssl.HP_SSLAgent_GetSSLSessionInfo(SenderPtr, connId, info, ref sessionInfo);
        }

        public override bool Start()
        {
            if (!IsInitSsl)
            {
                throw new InvalidOperationException("请先初始化ssl环境, 调用 Initialize() 方法");
            }

            if (ProxyList?.Count > 0)
            {
                AutoHandShake = false;
                HttpAutoStart = false;
            }

            return base.Start();
        }

        protected override void HttpAgentOnProxyConnected(IAgent sender, IntPtr connId, IProxy proxy)
        {
            StartHandShake(connId);
            StartHttp(connId);
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
