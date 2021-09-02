using System;
using HPSocket.Adapter;
using HPSocket.Tcp;

namespace HPSocket.Ssl
{
    /// <summary>
    /// ssl agent
    /// </summary>
    /// <typeparam name="TRequestBodyType">包体解析对象类型</typeparam>
    public class SslAgent<TRequestBodyType> : SslAgent, ISslAgent<TRequestBodyType>
    {
        public SslAgent()
        {
            OnConnect += SslAgent_OnConnect;
            base.OnReceive += SslAgent_OnReceive;
            OnClose += SslAgent_OnClose;
        }

#pragma warning disable 0067
        [Obsolete("adapter组件无需添加OnReceive事件, 请添加OnParseRequestBody事件", true)]
        public new event AgentReceiveEventHandler OnReceive;
#pragma warning restore

        /// <inheritdoc />
        public event ParseRequestBody<ITcpAgent, TRequestBodyType> OnParseRequestBody;

        /// <inheritdoc />
        public DataReceiveAdapter<TRequestBodyType> DataReceiveAdapter { get; set; }

        private HandleResult SslAgent_OnConnect(IAgent sender, IntPtr connId, IProxy proxy)
        {
            DataReceiveAdapter.OnOpen(connId);
            return HandleResult.Ok;
        }

        private HandleResult SslAgent_OnReceive(IAgent sender, IntPtr connId, byte[] data)
        {
            return DataReceiveAdapter.OnReceive(this, connId, data, OnParseRequestBody);
        }

        private HandleResult SslAgent_OnClose(IAgent sender, IntPtr connId, SocketOperation socketOperation, int errorCode)
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


    public class SslAgent : TcpAgent, ISslAgent
    {
        #region 保护成员

        /// <summary>
        /// 获取或设置是否初始化ssl环境
        /// </summary>
        protected bool IsInitSsl { get; set; }

        #endregion

        public SslAgent()
            : base(Sdk.Tcp.Create_HP_TcpAgentListener,
                Sdk.Ssl.Create_HP_SSLAgent,
                Sdk.Ssl.Destroy_HP_SSLAgent,
                Sdk.Tcp.Destroy_HP_TcpAgentListener)
        {
        }

        protected SslAgent(Sdk.CreateListenerDelegate createListenerFunction, Sdk.CreateServiceDelegate createServiceFunction, Sdk.DestroyListenerDelegate destroyServiceFunction, Sdk.DestroyListenerDelegate destroyListenerFunction)
            : base(createListenerFunction, createServiceFunction, destroyServiceFunction, destroyListenerFunction)
        {
        }

        ~SslAgent() => Dispose(false);

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
        public bool Initialize(bool memory)
        {
            PemCertFile = String.IsNullOrWhiteSpace(PemCertFile) ? null : PemCertFile;
            PemKeyFile = String.IsNullOrWhiteSpace(PemKeyFile) ? null : PemKeyFile;
            KeyPassword = String.IsNullOrWhiteSpace(KeyPassword) ? null : KeyPassword;
            CaPemCertFileOrPath = String.IsNullOrWhiteSpace(CaPemCertFileOrPath) ? null : CaPemCertFileOrPath;
            IsInitSsl = memory
                ? Sdk.Ssl.HP_SSLAgent_SetupSSLContextByMemory(SenderPtr, VerifyMode, PemCertFile, PemKeyFile, KeyPassword, CaPemCertFileOrPath)
                : Sdk.Ssl.HP_SSLAgent_SetupSSLContext(SenderPtr, VerifyMode, PemCertFile, PemKeyFile, KeyPassword, CaPemCertFileOrPath);
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
                Sdk.Ssl.HP_SSLAgent_CleanupSSLContext(SenderPtr);
                IsInitSsl = false;
            }
        }

        /// <inheritdoc />
        public bool StartHandShake(IntPtr connId)
        {
            var ok = Sdk.Ssl.HP_SSLAgent_StartSSLHandShake(SenderPtr, connId);
#if !NET20 && !NET30 && !NET35
            SysErrorCode.Value = ok ? 0 : Sdk.Sys.SYS_GetLastError();
#endif
            return ok;
        }

        /// <inheritdoc />
        public bool GetSessionInfo(IntPtr connId, SslSessionInfo info, out IntPtr sessionInfo)
        {
            sessionInfo = IntPtr.Zero;
            var ok = Sdk.Ssl.HP_SSLAgent_GetSSLSessionInfo(SenderPtr, connId, info, ref sessionInfo);
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
