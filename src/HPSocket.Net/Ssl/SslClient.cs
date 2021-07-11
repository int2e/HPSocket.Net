using System;

using HPSocket.Adapter;
using HPSocket.Tcp;

namespace HPSocket.Ssl
{
    /// <summary>
    /// ssl client
    /// </summary>
    /// <typeparam name="TRequestBodyType">包体解析对象类型</typeparam>
    public class SslClient<TRequestBodyType> : SslClient, ISslClient<TRequestBodyType>
    {
        public SslClient()
        {
            OnConnect += SslClient_OnConnect;
            base.OnReceive += SslClient_OnReceive;
            OnClose += SslClient_OnClose;
        }

#pragma warning disable 0067
        [Obsolete("adapter组件无需添加OnReceive事件, 请添加OnParseRequestBody事件", true)]
        public new event ClientReceiveEventHandler OnReceive;
#pragma warning restore

        /// <inheritdoc />
        public event ParseRequestBody<ISslClient, TRequestBodyType> OnParseRequestBody;

        /// <inheritdoc />
        public DataReceiveAdapter<TRequestBodyType> DataReceiveAdapter { get; set; }

        private HandleResult SslClient_OnConnect(IClient sender)
        {
            DataReceiveAdapter.OnOpen(ConnectionId);
            return HandleResult.Ok;
        }

        private HandleResult SslClient_OnReceive(IClient sender, byte[] data)
        {
            return DataReceiveAdapter.OnReceive(this, ConnectionId, data, OnParseRequestBody);
        }

        private HandleResult SslClient_OnClose(IClient sender, SocketOperation socketOperation, int errorCode)
        {
            DataReceiveAdapter.OnClose(ConnectionId);
            return HandleResult.Ok;
        }

        #region 重写父类方法

        /// <inheritdoc />
        public new bool Connect()
        {
            if (DataReceiveAdapter == null || OnParseRequestBody == null)
            {
                throw new InitializationException("DataReceiveAdapter属性和OnParseRequestBody事件必须赋值");
            }

            return base.Connect();
        }

        /// <inheritdoc />
        public new bool Connect(string address, ushort port)
        {
            if (DataReceiveAdapter == null || OnParseRequestBody == null)
            {
                throw new InitializationException("DataReceiveAdapter属性和OnParseRequestBody事件必须赋值");
            }

            return base.Connect(address, port);
        }

        #endregion
    }


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
                Sdk.Ssl.HP_SSLClient_CleanupSSLContext(SenderPtr);
                IsInitSsl = false;
            }
        }

        /// <inheritdoc />
        public bool StartHandShake()
        {
            var ok = Sdk.Ssl.HP_SSLClient_StartSSLHandShake(SenderPtr);
#if !NET20 && !NET30 && !NET35
            SysErrorCode.Value = ok ? 0 : Sdk.Sys.SYS_GetLastError();
#endif
            return ok;
        }

        /// <inheritdoc />
        public bool GetSessionInfo(SslSessionInfo info, out IntPtr sessionInfo)
        {
            sessionInfo = IntPtr.Zero;
            var ok = Sdk.Ssl.HP_SSLClient_GetSSLSessionInfo(SenderPtr, info, ref sessionInfo);
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
