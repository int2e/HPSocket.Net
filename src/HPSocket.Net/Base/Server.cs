using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
#if !NET20 && !NET30 && !NET35
using System.Threading.Tasks;
using System.Collections.Concurrent;
#endif
using HPSocket.Sdk;

namespace HPSocket.Base
{
    public abstract class Server : IServer
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
        /// <summary>
        /// 附加数据
        /// </summary>
        protected ExtraData<IntPtr, object> ExtraData = new ExtraData<IntPtr, object>();

        protected CreateListenerDelegate CreateListenerFunction;
        protected CreateServiceDelegate CreateServiceFunction;
        protected DestroyListenerDelegate DestroyListenerFunction;
        protected DestroyListenerDelegate DestroyServiceFunction;
        #endregion

        protected Server(CreateListenerDelegate createListenerFunction, CreateServiceDelegate createServiceFunction,
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

        ~Server()
        {
            Dispose(false);
        }

        /// <inheritdoc />
        public IntPtr SenderPtr { get; protected set; } = IntPtr.Zero;

        /// <inheritdoc />
        public object Tag { get; set; }

        /// <inheritdoc />
        public string Address { get; set; } = "0.0.0.0";

        /// <inheritdoc />
        public ushort Port { get; set; } = 0;

        /// <inheritdoc />
        public event ServerAcceptEventHandler OnAccept;

        /// <inheritdoc />
        public event ServerSendEventHandler OnSend;

        /// <inheritdoc />
        public event ServerPrepareListenEventHandler OnPrepareListen;

        /// <inheritdoc />
        public event ServerReceiveEventHandler OnReceive;

        /// <inheritdoc />
        public event ServerCloseEventHandler OnClose;

        /// <inheritdoc />
        public event ServerShutdownEventHandler OnShutdown;

        /// <inheritdoc />
        public event ServerHandShakeEventHandler OnHandShake;

        /// <inheritdoc />
        public uint MaxConnectionCount
        {
            get => Sdk.Server.HP_Server_GetMaxConnectionCount(SenderPtr);
            set => Sdk.Server.HP_Server_SetMaxConnectionCount(SenderPtr, value);
        }

        /// <inheritdoc />
        public uint WorkerThreadCount
        {
            get => Sdk.Server.HP_Server_GetWorkerThreadCount(SenderPtr);
            set => Sdk.Server.HP_Server_SetWorkerThreadCount(SenderPtr, value);
        }

        /// <inheritdoc />
        public uint FreeSocketObjLockTime
        {
            get => Sdk.Server.HP_Server_GetFreeSocketObjLockTime(SenderPtr);
            set => Sdk.Server.HP_Server_SetFreeSocketObjLockTime(SenderPtr, value);
        }

        /// <inheritdoc />
        public uint FreeSocketObjPool
        {
            get => Sdk.Server.HP_Server_GetFreeSocketObjPool(SenderPtr);
            set => Sdk.Server.HP_Server_SetFreeSocketObjPool(SenderPtr, value);
        }

        /// <inheritdoc />
        public uint FreeBufferObjPool
        {
            get => Sdk.Server.HP_Server_GetFreeBufferObjPool(SenderPtr);
            set => Sdk.Server.HP_Server_SetFreeBufferObjPool(SenderPtr, value);
        }

        /// <inheritdoc />
        public uint FreeSocketObjHold
        {
            get => Sdk.Server.HP_Server_GetFreeSocketObjHold(SenderPtr);
            set => Sdk.Server.HP_Server_SetFreeSocketObjHold(SenderPtr, value);
        }

        /// <inheritdoc />
        public uint FreeBufferObjHold
        {
            get => Sdk.Server.HP_Server_GetFreeBufferObjHold(SenderPtr);
            set => Sdk.Server.HP_Server_SetFreeBufferObjHold(SenderPtr, value);
        }

        /// <inheritdoc />
        public bool IsMarkSilence
        {
            get => Sdk.Server.HP_Server_IsMarkSilence(SenderPtr);
            set => Sdk.Server.HP_Server_SetMarkSilence(SenderPtr, value);
        }

        /// <inheritdoc />
        public SendPolicy SendPolicy
        {
            get => Sdk.Server.HP_Server_GetSendPolicy(SenderPtr);
            set => Sdk.Server.HP_Server_SetSendPolicy(SenderPtr, value);
        }

        /// <inheritdoc />
        public OnSendSyncPolicy OnSendSyncPolicy
        {
            get => Sdk.Server.HP_Server_GetOnSendSyncPolicy(SenderPtr);
            set => Sdk.Server.HP_Server_SetOnSendSyncPolicy(SenderPtr, value);
        }

        /// <inheritdoc />
        public ReuseAddressPolicy ReuseAddressPolicy
        {
            get => Sdk.Server.HP_Server_GetReuseAddressPolicy(SenderPtr);
            set => Sdk.Server.HP_Server_SetReuseAddressPolicy(SenderPtr, value);
        }

        /// <inheritdoc />
        public bool HasStarted => SenderPtr != IntPtr.Zero && Sdk.Server.HP_Server_HasStarted(SenderPtr);

        /// <inheritdoc />
        public ServiceState State => Sdk.Server.HP_Server_GetState(SenderPtr);

        /// <inheritdoc />
        public uint ConnectionCount => Sdk.Server.HP_Server_GetConnectionCount(SenderPtr);

        /// <inheritdoc />
        public bool IsSecure => Sdk.Server.HP_Server_IsSecure(SenderPtr);

        /// <inheritdoc />
        public SocketError ErrorCode => Sdk.Server.HP_Server_GetLastError(SenderPtr);

        /// <inheritdoc />
        public string Version => Sys.GetVersion();

#if !NET20 && !NET30 && !NET35
        /// <inheritdoc />
        public ThreadLocal<int> SysErrorCode { get; protected set; }
#endif

        /// <inheritdoc />
        public string ErrorMessage => Sdk.Server.HP_Server_GetLastErrorDesc(SenderPtr).PtrToAnsiString();


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
        public virtual bool Start()
        {

            if (String.IsNullOrWhiteSpace(Address))
            {
                throw new InvalidOperationException("Address属性未设置正确的本机IP地址");
            }

            if (Port < 1)
            {
                throw new InvalidOperationException("Port属性未设置正确的端口号");
            }

            if (HasStarted)
            {
                return true;
            }

            return Sdk.Server.HP_Server_Start(SenderPtr, Address, Port);
        }

        /// <inheritdoc />
        public bool Stop() => HasStarted && Sdk.Server.HP_Server_Stop(SenderPtr);

        /// <inheritdoc />
        public bool Wait(int milliseconds = -1)
        {
            var ok = Sdk.Server.HP_Server_Wait(SenderPtr, milliseconds);
#if !NET20 && !NET30 && !NET35
            SysErrorCode.Value = ok ? 0 : Sys.SYS_GetLastError();
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
        public bool Send(IntPtr connId, byte[] bytes, int length)
        {
            var gch = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            var ok = Sdk.Server.HP_Server_Send(SenderPtr, connId, gch.AddrOfPinnedObject(), length);
#if !NET20 && !NET30 && !NET35
            SysErrorCode.Value = ok ? 0 : Sys.SYS_GetLastError();
#endif
            gch.Free();
            return ok;
        }

        /// <inheritdoc />
        public bool Send(IntPtr connId, byte[] bytes, int offset, int length)
        {
            var gch = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            var ok = Sdk.Server.HP_Server_SendPart(SenderPtr, connId, gch.AddrOfPinnedObject(), length, offset);
#if !NET20 && !NET30 && !NET35
            SysErrorCode.Value = ok ? 0 : Sys.SYS_GetLastError();
#endif
            gch.Free();
            return ok;
        }

        /// <inheritdoc />
        public bool SendPackets(IntPtr connId, Wsabuf[] buffers)
        {
            var ok = Sdk.Server.HP_Server_SendPackets(SenderPtr, connId, buffers, buffers.Length);
#if !NET20 && !NET30 && !NET35
            SysErrorCode.Value = ok ? 0 : Sys.SYS_GetLastError();
#endif
            return ok;
        }

        /// <inheritdoc />
        public bool Disconnect(IntPtr connId, bool force = true) => Sdk.Server.HP_Server_Disconnect(SenderPtr, connId, force);

        /// <inheritdoc />
        public bool DisconnectLongConnections(uint period, bool force = true) => Sdk.Server.HP_Server_DisconnectLongConnections(SenderPtr, period, force);

        /// <inheritdoc />
        public bool PauseReceive(IntPtr connId) => Sdk.Server.HP_Server_PauseReceive(SenderPtr, connId, true);

        /// <inheritdoc />
        public bool ResumeReceive(IntPtr connId) => Sdk.Server.HP_Server_PauseReceive(SenderPtr, connId, false);

        /// <inheritdoc />
        public ReceiveState GetReceiveState(IntPtr connId)
        {
            var state = -1;

            if (Sdk.Server.HP_Server_IsPauseReceive(SenderPtr, connId, ref state))
            {
                return (ReceiveState)state;
            }
            return ReceiveState.Unknown;
        }

        /// <inheritdoc />
        public bool DisconnectSilenceConnections(uint period, bool force = true) => Sdk.Server.HP_Server_DisconnectSilenceConnections(SenderPtr, period, force);

        /// <inheritdoc />
        public bool GetLocalAddress(IntPtr connId, out string ip, out ushort port)
        {
            var ipLength = 60;
            var sb = new StringBuilder(ipLength);
            port = 0;
            var ok = Sdk.Server.HP_Server_GetLocalAddress(SenderPtr, connId, sb, ref ipLength, ref port) && ipLength > 0;
            ip = ok ? sb.ToString() : string.Empty;
            return ok;
        }

        /// <inheritdoc />
        public bool GetRemoteAddress(IntPtr connId, out string ip, out ushort port)
        {
            var ipLength = 60;
            var sb = new StringBuilder(ipLength);
            port = 0;
            var ok = Sdk.Server.HP_Server_GetRemoteAddress(SenderPtr, connId, sb, ref ipLength, ref port) && ipLength > 0;
            ip = ok ? sb.ToString() : string.Empty;
            return ok;
        }

        /// <inheritdoc />
        public bool GetPendingDataLength(IntPtr connId, out int length)
        {
            length = 0;
            return Sdk.Server.HP_Server_GetPendingDataLength(SenderPtr, connId, ref length) && length > 0;
        }

        /// <inheritdoc />
        public bool GetListenAddress(out string ip, out ushort port)
        {
            var ipLength = 60;
            var sb = new StringBuilder(ipLength);
            port = 0;
            var ok = Sdk.Server.HP_Server_GetListenAddress(SenderPtr, sb, ref ipLength, ref port) && ipLength > 0;
            ip = ok ? sb.ToString() : string.Empty;
            return ok;
        }

        /// <inheritdoc />
        public bool GetConnectPeriod(IntPtr connId, out uint period)
        {
            period = 0;
            return Sdk.Server.HP_Server_GetConnectPeriod(SenderPtr, connId, ref period);
        }

        /// <inheritdoc />
        public bool GetSilencePeriod(IntPtr connId, out uint period)
        {
            period = 0;
            return Sdk.Server.HP_Server_GetSilencePeriod(SenderPtr, connId, ref period);
        }

        /// <inheritdoc />
        public List<IntPtr> GetAllConnectionIds()
        {
            var list = new List<IntPtr>();

            var count = ConnectionCount;
            if (count == 0)
            {
                return list;
            }

            var arr = new IntPtr[count];
            if (!Sdk.Server.HP_Server_GetAllConnectionIDs(SenderPtr, arr, ref count))
            {
                return list;
            }

            for (var i = 0; i < count; i++)
            {
                list.Add(arr[i]);
            }

            return list;
        }

        /// <inheritdoc />
        public bool IsConnected(IntPtr connId) => Sdk.Server.HP_Server_IsConnected(SenderPtr, connId);

        /// <inheritdoc />
        public bool NativeSetConnectionExtra(IntPtr connId, IntPtr extra) => Sdk.Server.HP_Server_SetConnectionExtra(SenderPtr, connId, extra);

        /// <inheritdoc />
        public bool NativeGetConnectionExtra(IntPtr connId, out IntPtr extra)
        {
            extra = IntPtr.Zero;
            return Sdk.Server.HP_Server_GetConnectionExtra(SenderPtr, connId, ref extra);
        }

        /// <inheritdoc />
        public bool SetExtra(IntPtr connId, object obj) => ExtraData.Set(connId, obj);

        /// <inheritdoc />
        public T GetExtra<T>(IntPtr connId)
        {
            var obj = ExtraData.Get(connId);
            return obj == null ? default : (T)obj;
        }

#if NET20 || NET30 || NET35
        /// <summary>
        /// 获取所有附加数据
        /// </summary>
        /// <returns></returns>
        public Dictionary<IntPtr, object> GetAllExtra()
        {
            return ExtraData.GetAll();
        }
#else
        /// <summary>
        /// 获取所有附加数据
        /// </summary>
        /// <returns></returns>
        public ConcurrentDictionary<IntPtr, object> GetAllExtra()
        {
            return ExtraData.GetAll();
        }
#endif

        /// <inheritdoc />
        public bool RemoveExtra(IntPtr connId) => ExtraData.Remove(connId);

        #region SDK事件

        #region SDK回调委托,防止GC

        private OnPrepareListen _onPrepareListen;
        private OnAccept _onAccept;
        private OnSend _onSend;
        private OnReceive _onReceive;
        private OnClose _onClose;
        private OnShutdown _onShutdown;
        private OnHandShake _onHandShake;

        #endregion

        /// <summary>
        /// 设置回调
        /// </summary>
        /// <returns></returns>
        protected virtual void SetCallback()
        {
            _onPrepareListen = SdkOnPrepareListen;
            _onAccept = SdkOnAccept;
            _onSend = SdkOnSend;
            _onReceive = SdkOnReceive;
            _onClose = SdkOnClose;
            _onShutdown = SdkOnShutdown;
            _onHandShake = SdkOnHandShake;

            Sdk.Server.HP_Set_FN_Server_OnPrepareListen(ListenerPtr, _onPrepareListen);
            Sdk.Server.HP_Set_FN_Server_OnAccept(ListenerPtr, _onAccept);
            Sdk.Server.HP_Set_FN_Server_OnSend(ListenerPtr, _onSend);
            Sdk.Server.HP_Set_FN_Server_OnReceive(ListenerPtr, _onReceive);
            Sdk.Server.HP_Set_FN_Server_OnClose(ListenerPtr, _onClose);
            Sdk.Server.HP_Set_FN_Server_OnShutdown(ListenerPtr, _onShutdown);
            Sdk.Server.HP_Set_FN_Server_OnHandShake(ListenerPtr, _onHandShake);

            GC.KeepAlive(_onPrepareListen);
            GC.KeepAlive(_onAccept);
            GC.KeepAlive(_onSend);
            GC.KeepAlive(_onReceive);
            GC.KeepAlive(_onClose);
            GC.KeepAlive(_onShutdown);
            GC.KeepAlive(_onHandShake);
        }

        protected HandleResult SdkOnPrepareListen(IntPtr sender, IntPtr socket)
        {
            // 直接发送策略同时设置NoDelay
            if (SendPolicy == SendPolicy.Direct)
            {
                Sys.SYS_SSO_NoDelay(socket, true);
            }

            return OnPrepareListen?.Invoke(this, socket) ?? HandleResult.Ignore;
        }

        protected HandleResult SdkOnAccept(IntPtr sender, IntPtr connId, IntPtr client) => OnAccept?.Invoke(this, connId, SenderPtr) ?? HandleResult.Ignore;

        protected HandleResult SdkOnReceive(IntPtr sender, IntPtr connId, IntPtr data, int length)
        {
            if (OnReceive == null) return HandleResult.Ignore;
            var bytes = new byte[length];
            if (bytes.Length > 0)
            {
                Marshal.Copy(data, bytes, 0, length);
            }
            return OnReceive(this, connId, bytes);
        }

        protected HandleResult SdkOnSend(IntPtr sender, IntPtr connId, IntPtr data, int length)
        {
            if (OnSend == null) return HandleResult.Ignore;
            var bytes = new byte[length];
            if (bytes.Length > 0)
            {
                Marshal.Copy(data, bytes, 0, length);
            }
            return OnSend(this, connId, bytes);
        }

        protected HandleResult SdkOnClose(IntPtr sender, IntPtr connId, SocketOperation socketOperation, int errorCode) => OnClose?.Invoke(this, connId, socketOperation, errorCode) ?? HandleResult.Ignore;

        protected HandleResult SdkOnShutdown(IntPtr sender) => OnShutdown?.Invoke(this) ?? HandleResult.Ignore;

        protected HandleResult SdkOnHandShake(IntPtr sender, IntPtr connId) => OnHandShake?.Invoke(this, connId) ?? HandleResult.Ignore;

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
