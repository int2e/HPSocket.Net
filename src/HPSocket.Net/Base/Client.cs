using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
#if !NET20 && !NET30 && !NET35
using System.Threading.Tasks;
#endif
using HPSocket.Sdk;

namespace HPSocket.Base
{
    public abstract class Client : IClient
    {
        #region 私有成员

        /// <summary>
        /// 是否释放了
        /// </summary>
        private bool _disposed;

        #endregion

        #region 保护成员

        /// <summary>
        /// 监听对象指针
        /// </summary>
        protected IntPtr ListenerPtr = IntPtr.Zero;

        protected CreateListenerDelegate CreateListenerFunction;
        protected CreateServiceDelegate CreateServiceFunction;
        protected DestroyListenerDelegate DestroyListenerFunction;
        protected DestroyListenerDelegate DestroyServiceFunction;

        #endregion

        protected Client(CreateListenerDelegate createListenerFunction, CreateServiceDelegate createServiceFunction,
            DestroyListenerDelegate destroyServiceFunction, DestroyListenerDelegate destroyListenerFunction)
        {
#if !NET20 && !NET30 && !NET35
            SysErrorCode = new ThreadLocal<int>(() => System.Threading.Thread.CurrentThread.ManagedThreadId);
#endif
            CreateListenerFunction = createListenerFunction;
            CreateServiceFunction = createServiceFunction;
            DestroyListenerFunction = destroyListenerFunction;
            DestroyServiceFunction = destroyServiceFunction;
            if (!CreateListener())
            {
                throw new InitializationException("未能正确初始化监听");
            }
        }

        /// <inheritdoc />
        public IntPtr SenderPtr { get; protected set; } = IntPtr.Zero;

        /// <inheritdoc />
        public object Tag { get; set; }

        /// <inheritdoc />
        public string Address { get; set; } = "0.0.0.0";

        /// <inheritdoc />
        public ushort Port { get; set; }

        /// <inheritdoc />
        public string BindAddress { get; set; } = "0.0.0.0";

        /// <inheritdoc />
        public ushort BindPort { get; set; } = 0;

        /// <inheritdoc />
        public bool Async { get; set; } = true;

        /// <inheritdoc />
        public object ExtraData { get; set; }

        /// <inheritdoc />
        public event ClientPrepareConnectEventHandler OnPrepareConnect;

        /// <inheritdoc />
        public event ClientConnectEventHandler OnConnect;

        /// <inheritdoc />
        public event ClientSendEventHandler OnSend;

        /// <inheritdoc />
        public event ClientReceiveEventHandler OnReceive;

        /// <inheritdoc />
        public event ClientCloseEventHandler OnClose;

        /// <inheritdoc />
        public event ClientHandShakeEventHandler OnHandShake;

        /// <inheritdoc />
        public uint FreeBufferPoolSize
        {
            get => Sdk.Client.HP_Client_GetFreeBufferPoolSize(SenderPtr);
            set => Sdk.Client.HP_Client_SetFreeBufferPoolSize(SenderPtr, value);
        }

        /// <inheritdoc />
        public uint FreeBufferPoolHold
        {
            get => Sdk.Client.HP_Client_GetFreeBufferPoolHold(SenderPtr);
            set => Sdk.Client.HP_Client_SetFreeBufferPoolHold(SenderPtr, value);
        }

        /// <inheritdoc />
        public bool HasStarted => Sdk.Client.HP_Client_HasStarted(SenderPtr);

        /// <inheritdoc />
        public bool IsConnected => Sdk.Client.HP_Client_IsConnected(SenderPtr);

        /// <inheritdoc />
        public ServiceState State => Sdk.Client.HP_Client_GetState(SenderPtr);

        /// <inheritdoc />
        public IntPtr ConnectionId => Sdk.Client.HP_Client_GetConnectionID(SenderPtr);

        /// <inheritdoc />
        public bool IsSecure => Sdk.Client.HP_Client_IsSecure(SenderPtr);

        /// <inheritdoc />
        public ReceiveState PauseReceive
        {
            get
            {
                var state = -1;

                if (Sdk.Client.HP_Client_IsPauseReceive(SenderPtr, ref state))
                {
                    return (ReceiveState)state;
                }
                return ReceiveState.Unknown;
            }

            set
            {
                if (value == ReceiveState.Unknown)
                {
                    throw new InvalidOperationException("PauseReceive属性不允许设置为Unknown");
                }

                Sdk.Client.HP_Client_PauseReceive(SenderPtr, value == ReceiveState.Pause);
            }
        }

        /// <inheritdoc />
        public ReuseAddressPolicy ReuseAddressPolicy
        {
            get => Sdk.Client.HP_Client_GetReuseAddressPolicy(SenderPtr);
            set => Sdk.Client.HP_Client_SetReuseAddressPolicy(SenderPtr, value);
        }

        /// <inheritdoc />
        public SocketError ErrorCode => Sdk.Client.HP_Client_GetLastError(SenderPtr);

        /// <inheritdoc />
        public string Version => Sys.GetVersion();

#if !NET20 && !NET30 && !NET35
        /// <inheritdoc />
        public ThreadLocal<int> SysErrorCode { get; protected set; }
#endif

        /// <inheritdoc />
        public string ErrorMessage => Sdk.Client.HP_Client_GetLastErrorDesc(SenderPtr).PtrToAnsiString();

        /// <summary>
        /// 创建socket监听和服务组件
        /// </summary>
        /// <returns></returns>
        private bool CreateListener()
        {
            if (ListenerPtr != IntPtr.Zero || SenderPtr != IntPtr.Zero)
            {
                return false;
            }

            ListenerPtr = CreateListenerFunction.Invoke();
            if (ListenerPtr == IntPtr.Zero)
            {
                return false;
            }

            SenderPtr = CreateServiceFunction.Invoke(ListenerPtr);
            if (SenderPtr == IntPtr.Zero)
            {
                Destroy();
                return false;
            }

            SetCallback();

            return true;
        }

        /// <summary>
        /// 终止服务并释放资源
        /// </summary>
        private void Destroy()
        {
            Stop();

            if (SenderPtr != IntPtr.Zero)
            {

                DestroyServiceFunction?.Invoke(SenderPtr);
                SenderPtr = IntPtr.Zero;
            }

            if (ListenerPtr != IntPtr.Zero)
            {
                DestroyListenerFunction?.Invoke(ListenerPtr);
                ListenerPtr = IntPtr.Zero;
            }
        }

        /// <inheritdoc />
        public bool Wait(int milliseconds = -1)
        {
            var ok = Sdk.Client.HP_Client_Wait(SenderPtr, milliseconds);
#if !NET20 && !NET30 && !NET35
            SysErrorCode.Value = ok ? 0 : Sdk.Sys.SYS_GetLastError();
#endif
            return ok;
        }

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
        public bool Connect()
        {
            if (String.IsNullOrWhiteSpace(Address))
            {
                throw new InvalidOperationException("Address属性未设置正确的目标服务器IP地址");
            }

            if (Port < 1)
            {
                throw new InvalidOperationException("Port属性未设置正确的目标服务器端口号");
            }

            if (IsConnected)
            {
                return true;
            }

            if (!String.IsNullOrWhiteSpace(BindAddress) && BindPort > 0)
            {
                return Sdk.Client.HP_Client_StartWithBindAddressAndLocalPort(SenderPtr, Address, Port, Async, BindAddress, BindPort);
            }

            if (!String.IsNullOrWhiteSpace(BindAddress))
            {
                return Sdk.Client.HP_Client_StartWithBindAddress(SenderPtr, Address, Port, Async, BindAddress);
            }

            return Sdk.Client.HP_Client_Start(SenderPtr, Address, Port, Async);
        }

        /// <inheritdoc />
        public bool Connect(string address, ushort port)
        {
            Address = address;
            Port = port;
            return Connect();
        }

        /// <inheritdoc />
        public bool Stop() => State != ServiceState.Stopped && State != ServiceState.Stopping && Sdk.Client.HP_Client_Stop(SenderPtr);

        /// <inheritdoc />
        public bool Send(byte[] bytes, int length)
        {
            var gch = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            var ok = Sdk.Client.HP_Client_Send(SenderPtr, gch.AddrOfPinnedObject(), length);
#if !NET20 && !NET30 && !NET35
            SysErrorCode.Value = ok ? 0 : Sdk.Sys.SYS_GetLastError();
#endif
            gch.Free();
            return ok;
        }

        /// <inheritdoc />
        public bool Send(byte[] bytes, int offset, int length)
        {
            var gch = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            var ok = Sdk.Client.HP_Client_SendPart(SenderPtr, gch.AddrOfPinnedObject(), length, offset);
#if !NET20 && !NET30 && !NET35
            SysErrorCode.Value = ok ? 0 : Sdk.Sys.SYS_GetLastError();
#endif
            gch.Free();
            return ok;
        }

        /// <inheritdoc />
        public bool SendPackets(Wsabuf[] buffers, int count)
        {
            var ok = Sdk.Client.HP_Client_SendPackets(SenderPtr, buffers, count);
#if !NET20 && !NET30 && !NET35
            SysErrorCode.Value = ok ? 0 : Sdk.Sys.SYS_GetLastError();
#endif
            return ok;
        }

        /// <inheritdoc />
        public bool GetPendingDataLength(out int length)
        {
            length = 0;
            return Sdk.Client.HP_Client_GetPendingDataLength(SenderPtr, ref length);
        }

        /// <inheritdoc />
        public bool GetListenAddress(out string host, out ushort port)
        {
            var length = 60;
            port = 0;
            var sb = new StringBuilder(length);
            var ok = Sdk.Client.HP_Client_GetLocalAddress(SenderPtr, sb, ref length, ref port);
            host = ok ? sb.ToString() : string.Empty;
            return ok;
        }

        /// <inheritdoc />
        public bool GetRemoteHost(out string host, out ushort port)
        {
            var length = 60;
            port = 0;
            var sb = new StringBuilder(length);
            var ok = Sdk.Client.HP_Client_GetRemoteHost(SenderPtr, sb, ref length, ref port);
            host = ok ? sb.ToString() : string.Empty;
            return ok;
        }

        #region SDK事件

        #region SDK回调委托,防止GC

        private OnPrepareConnect _onPrepareConnect;
        private OnConnect _onConnect;
        private OnSend _onSend;
        private OnReceive _onReceive;
        private OnClose _onClose;
        private OnHandShake _onHandShake;

        #endregion

        /// <summary>
        /// 设置回调
        /// </summary>
        /// <returns></returns>
        protected virtual void SetCallback()
        {
            _onPrepareConnect = SdkOnPrepareConnect;
            _onConnect = SdkOnConnect;
            _onSend = SdkOnSend;
            _onReceive = SdkOnReceive;
            _onClose = SdkOnClose;
            _onHandShake = SdkOnHandShake;

            Sdk.Client.HP_Set_FN_Client_OnPrepareConnect(ListenerPtr, _onPrepareConnect);
            Sdk.Client.HP_Set_FN_Client_OnConnect(ListenerPtr, _onConnect);
            Sdk.Client.HP_Set_FN_Client_OnSend(ListenerPtr, _onSend);
            Sdk.Client.HP_Set_FN_Client_OnReceive(ListenerPtr, _onReceive);
            Sdk.Client.HP_Set_FN_Client_OnClose(ListenerPtr, _onClose);
            Sdk.Client.HP_Set_FN_Client_OnHandShake(ListenerPtr, _onHandShake);

            GC.KeepAlive(_onPrepareConnect);
            GC.KeepAlive(_onConnect);
            GC.KeepAlive(_onSend);
            GC.KeepAlive(_onReceive);
            GC.KeepAlive(_onClose);
            GC.KeepAlive(_onHandShake);
        }

        protected HandleResult SdkOnPrepareConnect(IntPtr sender, IntPtr connId, IntPtr socket) => OnPrepareConnect?.Invoke(this, socket) ?? HandleResult.Ignore;

        protected HandleResult SdkOnConnect(IntPtr sender, IntPtr connId) => OnConnect?.Invoke(this) ?? HandleResult.Ignore;

        protected HandleResult SdkOnReceive(IntPtr sender, IntPtr connId, IntPtr data, int length)
        {
            if (OnReceive == null) return HandleResult.Ignore;
            var bytes = new byte[length];
            if (bytes.Length > 0)
            {
                Marshal.Copy(data, bytes, 0, length);
            }
            return OnReceive(this, bytes);
        }
        protected HandleResult SdkOnSend(IntPtr sender, IntPtr connId, IntPtr data, int length)
        {
            if (OnSend == null) return HandleResult.Ignore;
            var bytes = new byte[length];
            if (bytes.Length > 0)
            {
                Marshal.Copy(data, bytes, 0, length);
            }
            return OnSend(this, bytes);
        }

        protected HandleResult SdkOnClose(IntPtr sender, IntPtr connId, SocketOperation socketOperation, int errorCode) => OnClose?.Invoke(this, socketOperation, errorCode) ?? HandleResult.Ignore;

        protected HandleResult SdkOnHandShake(IntPtr sender, IntPtr connId) => OnHandShake?.Invoke(this) ?? HandleResult.Ignore;

        #endregion

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
#if !NET20 && !NET30 && !NET35
                SysErrorCode?.Dispose();
#endif
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
    }
}
