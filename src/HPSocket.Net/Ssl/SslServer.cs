using System;
using HPSocket.Tcp;

namespace HPSocket.Ssl
{
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

            return Sdk.Ssl.HP_SSLServer_AddSSLContext(SenderPtr, verifyMode, pemCertFile, pemKeyFile, keyPassword, caPemCertFileOrPath);
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
            return Sdk.Ssl.HP_SSLServer_GetSSLSessionInfo(SenderPtr, connId, info, ref sessionInfo);
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
