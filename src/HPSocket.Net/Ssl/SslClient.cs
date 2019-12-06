using System;
using HPSocket.Tcp;

namespace HPSocket.Ssl
{
    public class SslClient : TcpClient, ISslClient
    {
        #region 保护成员

        /// <summary>
        /// 获取或设置是否初始化ssl环境
        /// </summary>
        protected bool IsInitSsl { get; set; }

        #endregion

        public SslClient()
            : base(Sdk.Tcp.Create_HP_TcpClientListener,
                Sdk.Ssl.Create_HP_SSLClient,
                Sdk.Ssl.Destroy_HP_SSLClient,
                Sdk.Tcp.Destroy_HP_TcpClientListener)
        {
        }

        protected SslClient(Sdk.CreateListenerDelegate createListenerFunction, Sdk.CreateServiceDelegate createServiceFunction, Sdk.DestroyListenerDelegate destroyServiceFunction, Sdk.DestroyListenerDelegate destroyListenerFunction)
            : base(createListenerFunction, createServiceFunction, destroyServiceFunction, destroyListenerFunction)
        {
        }

        ~SslClient() => Dispose(false);

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
        public bool AutoHandShake
        {
            get => Sdk.Ssl.HP_SSLClient_IsSSLAutoHandShake(SenderPtr);
            set => Sdk.Ssl.HP_SSLClient_SetSSLAutoHandShake(SenderPtr, value);
        }

        /// <summary>
        /// 启动通讯组件并连接到服务器
        /// </summary>
        /// <returns></returns>
        public new bool Connect()
        {
            if (!IsInitSsl)
            {
                throw new InvalidOperationException("请先初始化ssl环境, 调用 Initialize() 方法");
            }
            return base.Connect();
        }

        /// <summary>
        /// 启动通讯组件并连接到服务器
        /// </summary>
        /// <param name="address">远程服务器地址</param>
        /// <param name="port">远程服务器端口</param>
        /// <returns></returns>
        public new bool Connect(string address, ushort port)
        {
            if (!IsInitSsl)
            {
                throw new InvalidOperationException("请先初始化ssl环境, 调用 Initialize() 方法");
            }

            return base.Connect(address, port);
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
                Sdk.Ssl.HP_SSLServer_CleanupSSLContext(SenderPtr);
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
