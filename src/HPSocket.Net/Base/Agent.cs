using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
#if !NET20 && !NET30 && !NET35
using System.Threading.Tasks;
#endif
using HPSocket.Proxy;
using HPSocket.Sdk;
using HPSocket.Tcp;

namespace HPSocket.Base
{
    public abstract class Agent : IAgent
    {
        #region 私有成员

        /// <summary>
        /// 是否释放了
        /// </summary>
        private volatile bool _disposed;

        /// <summary>
        /// 连接超时时间
        /// </summary>
        private int _connectionTimeout = 10000;

        /// <summary>
        /// 同步接收超时时间
        /// </summary>
        private int _syncRecvTimeout;

        /// <summary>
        /// 异步连接
        /// </summary>
        private bool _async = true;

        /// <summary>
        /// native id
        /// </summary>
        private int _connNativeId;

        /// <summary>
        /// native data
        /// </summary>
        private readonly ExtraData<int, NativeExtra> _connNativeData = new ExtraData<int, NativeExtra>();

        /// <summary>
        /// 连接超时定时器集合
        /// </summary>
        private readonly ExtraData<IntPtr, ConnectionTimeoutTimer> _connectionTimers =
            new ExtraData<IntPtr, ConnectionTimeoutTimer>();

        #endregion

        #region 保护成员

        /// <summary>
        /// 代理列表
        /// </summary>
        // ReSharper disable once InconsistentNaming
        protected List<IProxy> _proxyList;

        /// <summary>
        /// 监听对象指针
        /// </summary>
        protected IntPtr ListenerPtr = IntPtr.Zero;

        /// <summary>
        /// 附加数据
        /// </summary>
        protected ExtraData<IntPtr, object> ExtraData = new ExtraData<IntPtr, object>();

        /// <summary>
        /// 代理预连接
        /// </summary>
        protected event AgentProxyPrepareConnectEventHandler OnProxyPrepareConnect;

        /// <summary>
        /// 代理已连接
        /// </summary>
        protected event AgentProxyConnectedEventHandler OnProxyConnected;

        protected CreateListenerDelegate CreateListenerFunction;
        protected CreateServiceDelegate CreateServiceFunction;
        protected DestroyListenerDelegate DestroyListenerFunction;
        protected DestroyListenerDelegate DestroyServiceFunction;

        #endregion

        protected Agent(CreateListenerDelegate createListenerFunction, CreateServiceDelegate createServiceFunction,
            DestroyListenerDelegate destroyServiceFunction, DestroyListenerDelegate destroyListenerFunction)
        {
            _connNativeId = 0;
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
        public bool Async
        {
            get => _async;
            set
            {
                if (HasStarted)
                {
                    throw new InvalidOperationException("比如在启动服务之前设置当前属性的值");
                }

                _async = value;
            }
        }

        /// <inheritdoc />
        public virtual int ConnectionTimeout
        {
            get => _connectionTimeout;

            // ReSharper disable once ValueParameterNotUsed
            set
            {
                if (value < 100 || value % 100 != 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "超时时间毫秒数不能小于100且必须是100的倍数, 如: [100,1100,1300]");
                }

                _connectionTimeout = value;
            }
        }

        /// <inheritdoc />
        public int SyncRecvTimeout
        {
            get => _syncRecvTimeout;
            set
            {
                if (Async)
                {
                    _syncRecvTimeout = 0;
                    throw new InvalidOperationException("不支持设置异步接收超时时间");
                }

                if (value < 0)
                {
                    throw new InvalidOperationException("不能设置小于0的值");
                }

                _syncRecvTimeout = value;
            }
        }

        /// <inheritdoc />
        public event AgentConnectEventHandler OnConnect;

        /// <inheritdoc />
        public event AgentSendEventHandler OnSend;

        /// <inheritdoc />
        public event AgentPrepareConnectEventHandler OnPrepareConnect;

        /// <inheritdoc />
        public event AgentReceiveEventHandler OnReceive;

        /// <inheritdoc />
        public event AgentCloseEventHandler OnClose;

        /// <inheritdoc />
        public event AgentShutdownEventHandler OnShutdown;

        /// <inheritdoc />
        public event AgentHandShakeEventHandler OnHandShake;

        /// <inheritdoc />
        public bool HasStarted => SenderPtr != IntPtr.Zero && Sdk.Agent.HP_Agent_HasStarted(SenderPtr);

        /// <inheritdoc />
        public ServiceState State => Sdk.Agent.HP_Agent_GetState(SenderPtr);

        /// <inheritdoc />
        public uint ConnectionCount => Sdk.Agent.HP_Agent_GetConnectionCount(SenderPtr);

        /// <inheritdoc />
        public bool IsSecure => Sdk.Agent.HP_Agent_IsSecure(SenderPtr);

        /// <inheritdoc />
        public uint MaxConnectionCount
        {
            get => Sdk.Agent.HP_Agent_GetMaxConnectionCount(SenderPtr);
            set => Sdk.Agent.HP_Agent_SetMaxConnectionCount(SenderPtr, value);
        }

        /// <inheritdoc />
        public uint WorkerThreadCount
        {
            get => Sdk.Agent.HP_Agent_GetWorkerThreadCount(SenderPtr);
            set => Sdk.Agent.HP_Agent_SetWorkerThreadCount(SenderPtr, value);
        }

        /// <inheritdoc />
        public uint FreeSocketObjLockTime
        {
            get => Sdk.Agent.HP_Agent_GetFreeSocketObjLockTime(SenderPtr);
            set => Sdk.Agent.HP_Agent_SetFreeSocketObjLockTime(SenderPtr, value);
        }

        /// <inheritdoc />
        public uint FreeSocketObjPool
        {
            get => Sdk.Agent.HP_Agent_GetFreeSocketObjPool(SenderPtr);
            set => Sdk.Agent.HP_Agent_SetFreeSocketObjPool(SenderPtr, value);
        }

        /// <inheritdoc />
        public uint FreeBufferObjPool
        {
            get => Sdk.Agent.HP_Agent_GetFreeBufferObjPool(SenderPtr);
            set => Sdk.Agent.HP_Agent_SetFreeBufferObjPool(SenderPtr, value);
        }

        /// <inheritdoc />
        public uint FreeSocketObjHold
        {
            get => Sdk.Agent.HP_Agent_GetFreeSocketObjHold(SenderPtr);
            set => Sdk.Agent.HP_Agent_SetFreeSocketObjHold(SenderPtr, value);
        }

        /// <inheritdoc />
        public uint FreeBufferObjHold
        {
            get => Sdk.Agent.HP_Agent_GetFreeBufferObjHold(SenderPtr);
            set => Sdk.Agent.HP_Agent_SetFreeBufferObjHold(SenderPtr, value);
        }

        /// <inheritdoc />
        public bool IsMarkSilence
        {
            get => Sdk.Agent.HP_Agent_IsMarkSilence(SenderPtr);
            set => Sdk.Agent.HP_Agent_SetMarkSilence(SenderPtr, value);
        }

        /// <inheritdoc />
        public SendPolicy SendPolicy
        {
            get => Sdk.Agent.HP_Agent_GetSendPolicy(SenderPtr);
            set => Sdk.Agent.HP_Agent_SetSendPolicy(SenderPtr, value);
        }

        /// <inheritdoc />
        public OnSendSyncPolicy OnSendSyncPolicy
        {
            get => Sdk.Agent.HP_Agent_GetOnSendSyncPolicy(SenderPtr);
            set => Sdk.Agent.HP_Agent_SetOnSendSyncPolicy(SenderPtr, value);
        }

        /// <inheritdoc />
        public ReuseAddressPolicy ReuseAddressPolicy
        {
            get => Sdk.Tcp.HP_Agent_GetReuseAddressPolicy(SenderPtr);
            set => Sdk.Tcp.HP_Agent_SetReuseAddressPolicy(SenderPtr, value);
        }

        /// <inheritdoc />
        public SocketError ErrorCode => Sdk.Agent.HP_Agent_GetLastError(SenderPtr);

        /// <inheritdoc />
        public string Version => Sys.GetVersion();

#if !NET20 && !NET30 && !NET35
        /// <inheritdoc />
        public ThreadLocal<int> SysErrorCode { get; protected set; }
#endif

        /// <inheritdoc />
        public string ErrorMessage => Sdk.Agent.HP_Agent_GetLastErrorDesc(SenderPtr).PtrToAnsiString();

        /// <inheritdoc />
        public List<IProxy> ProxyList
        {
            get => _proxyList;
            set
            {
                _proxyList = value;
                if (_proxyList?.Count > 0)
                {
                    _onConnect = SdkOnConnect;
                    _onReceive = SdkOnReceive;
                }
                else
                {
                    _onConnect = SdkOnConnectNoProxy;
                    _onReceive = SdkOnReceiveNoProxy;
                }

                Sdk.Agent.HP_Set_FN_Agent_OnConnect(ListenerPtr, _onConnect);
                Sdk.Agent.HP_Set_FN_Agent_OnReceive(ListenerPtr, _onReceive);

                GC.KeepAlive(_onConnect);
                GC.KeepAlive(_onReceive);
            }
        }

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
            // 在释放本机资源前停止所有连接超时定时器,避免回调访问已释放的 SenderPtr
            foreach (var connectionTimer in _connectionTimers.GetAll().Values)
            {
                connectionTimer?.Dispose();
            }
            _connectionTimers.Clear();

            Stop();

            _connNativeId = 0;
            _connNativeData.Clear();

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
                throw new InvalidOperationException("BindAddress属性未设置正确的本机IP地址");
            }

            if (HasStarted)
            {
                return true;
            }

            return Sdk.Agent.HP_Agent_Start(SenderPtr, Address, Async);
        }

        /// <inheritdoc />
        public bool Stop() => HasStarted && State != ServiceState.Stopped && State != ServiceState.Stopping && Sdk.Agent.HP_Agent_Stop(SenderPtr);

        /// <inheritdoc />
        public bool Wait(int milliseconds = -1)
        {
            var ok = Sdk.Agent.HP_Agent_Wait(SenderPtr, milliseconds);
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
        public bool Connect(string address, ushort port)
        {
            return Connect(address, port, out _);
        }

        /// <inheritdoc />
        public bool Connect(string address, ushort port, out IntPtr connId)
        {
            return Connect(address, port, IntPtr.Zero, out connId);
        }

        /// <inheritdoc />
        public bool Connect(string address, ushort port, IntPtr extra, out IntPtr connId, string localAddress = "", ushort localPort = 0)
        {
            var nativeExtra = new NativeExtra
            {
                TcpConnectionState = TcpConnectionState.Connecting,
                UserExtra = extra,
            };
            var nativeId = Interlocked.Increment(ref _connNativeId);
            if (_proxyList?.Count > 0)
            {
                var proxy = GetRandomProxyServer();
                if ((proxy is Socks5Proxy socks5Proxy))
                {
                    socks5Proxy.SetRemoteAddressPort(address, port);
                }
                else if ((proxy is HttpProxy httpProxy))
                {
                    httpProxy.SetRemoteAddressPort(address, port);
                }
                nativeExtra.ProxyConnectionState = ProxyConnectionState.Step1;
                nativeExtra.Proxy = proxy;
                address = proxy.Host;
                port = proxy.Port;
                OnProxyPrepareConnect?.Invoke(this, proxy);
            }

            _connNativeData.Set(nativeId, nativeExtra);

            connId = IntPtr.Zero;
            bool ok;
            if (!String.IsNullOrEmpty(localAddress))
            {
                ok = Sdk.Agent.HP_Agent_ConnectWithExtraAndLocalAddressPort(SenderPtr,
                    address,
                    port,
                    ref connId,
                    (IntPtr)nativeId,
                    localPort,
                    localAddress);
            }
            else if (localPort != 0)
            {
                ok = Sdk.Agent.HP_Agent_ConnectWithExtraAndLocalPort(SenderPtr,
                    address,
                    port,
                    ref connId,
                    (IntPtr)nativeId,
                    localPort);
            }
            else
            {
                ok = Sdk.Agent.HP_Agent_ConnectWithExtra(SenderPtr,
                    address,
                    port,
                    ref connId,
                    (IntPtr)nativeId);
            }

            if (ok)
            {
#if !NET20 && !NET30 && !NET35
                SysErrorCode.Value = 0;
#endif
                DelayWaitConnectTimeout(connId, nativeExtra);
            }
            else
            {
                nativeExtra.TcpConnectionState = TcpConnectionState.Closed;
                _connNativeData.Remove(nativeId);
#if !NET20 && !NET30 && !NET35
                SysErrorCode.Value = Sys.SYS_GetLastError();
#endif
            }
            return ok;
        }

        /// <inheritdoc />
        public bool Connect(string address, ushort port, IntPtr extra) => Connect(address, port, extra, out _);

        /// <summary>
        /// 设置附加数据-非托管版本
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="extra"></param>
        /// <returns></returns>
        protected bool NativeSetConnectionExtra(IntPtr connId, IntPtr extra) => Sdk.Agent.HP_Agent_SetConnectionExtra(SenderPtr, connId, extra);

        /// <summary>
        /// 获取附加数据--非托管版本
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="extra"></param>
        /// <returns></returns>
        protected bool NativeGetConnectionExtra(IntPtr connId, out IntPtr extra)
        {
            extra = IntPtr.Zero;
            return Sdk.Agent.HP_Agent_GetConnectionExtra(SenderPtr, connId, ref extra);
        }

        /// <summary>
        /// 设置附加数据-非托管版本
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="extra"></param>
        /// <returns></returns>
        public bool SetConnectionExtra(IntPtr connId, IntPtr extra)
        {
            NativeExtra nativeExtra;
            if (NativeGetConnectionExtra(connId, out var nativeId))
            {
                nativeExtra = _connNativeData.Get(nativeId.ToInt32());
                if (nativeExtra != null)
                {
                    nativeExtra.UserExtra = extra;
                    return true;
                }
            }

            nativeExtra = new NativeExtra
            {
                UserExtra = extra,
            };
            var newNativeId = Interlocked.Increment(ref _connNativeId);
            _connNativeData.Set(newNativeId, nativeExtra);
            return NativeSetConnectionExtra(connId, (IntPtr)newNativeId);

        }

        /// <summary>
        /// 获取附加数据--非托管版本
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="extra"></param>
        /// <returns></returns>
        public bool GetConnectionExtra(IntPtr connId, out IntPtr extra)
        {
            extra = IntPtr.Zero;
            if (NativeGetConnectionExtra(connId, out var nativeId))
            {
                var nativeExtra = _connNativeData.Get(nativeId.ToInt32());
                if (nativeExtra != null)
                {
                    extra = nativeExtra.UserExtra;

                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc />
        public bool Send(IntPtr connId, byte[] bytes, int length)
        {
            var gch = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            var ok = Sdk.Agent.HP_Agent_Send(SenderPtr, connId, gch.AddrOfPinnedObject(), length);
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
            var ok = Sdk.Agent.HP_Agent_SendPart(SenderPtr, connId, gch.AddrOfPinnedObject(), length, offset);
#if !NET20 && !NET30 && !NET35
            SysErrorCode.Value = ok ? 0 : Sys.SYS_GetLastError();
#endif
            gch.Free();
            return ok;
        }

        /// <inheritdoc />
        public bool SendPackets(IntPtr connId, Wsabuf[] buffers)
        {
            var ok = Sdk.Agent.HP_Agent_SendPackets(SenderPtr, connId, buffers, buffers.Length);
#if !NET20 && !NET30 && !NET35
            SysErrorCode.Value = ok ? 0 : Sys.SYS_GetLastError();
#endif
            return ok;
        }

        /// <inheritdoc />
        public bool Disconnect(IntPtr connId, bool force = true) => Sdk.Agent.HP_Agent_Disconnect(SenderPtr, connId, force);

        /// <inheritdoc />
        public bool DisconnectLongConnections(uint period, bool force = true) => Sdk.Agent.HP_Agent_DisconnectLongConnections(SenderPtr, period, force);

        /// <inheritdoc />
        public bool PauseReceive(IntPtr connId) => Sdk.Agent.HP_Agent_PauseReceive(SenderPtr, connId, true);

        /// <inheritdoc />
        public bool ResumeReceive(IntPtr connId) => Sdk.Agent.HP_Agent_PauseReceive(SenderPtr, connId, false);

        /// <inheritdoc />
        public ReceiveState GetReceiveState(IntPtr connId)
        {
            var state = -1;

            if (Sdk.Agent.HP_Agent_IsPauseReceive(SenderPtr, connId, ref state))
            {
                return (ReceiveState)state;
            }

            return ReceiveState.Unknown;
        }

        /// <inheritdoc />
        public bool DisconnectSilenceConnections(uint period, bool force = true) =>
            Sdk.Agent.HP_Agent_DisconnectSilenceConnections(SenderPtr, period, force);

        /// <inheritdoc />
        public bool GetLocalAddress(IntPtr connId, out string ip, out ushort port)
        {
            var ipLength = 60;
            var sb = new StringBuilder(ipLength);
            port = 0;
            var ok = Sdk.Agent.HP_Agent_GetLocalAddress(SenderPtr, connId, sb, ref ipLength, ref port) && ipLength > 0;
            ip = ok ? sb.ToString() : string.Empty;
            return ok;
        }

        /// <inheritdoc />
        public bool GetRemoteAddress(IntPtr connId, out string ip, out ushort port)
        {
            var ipLength = 60;
            var sb = new StringBuilder(ipLength);
            port = 0;
            var ok = Sdk.Agent.HP_Agent_GetRemoteAddress(SenderPtr, connId, sb, ref ipLength, ref port) && ipLength > 0;
            ip = ok ? sb.ToString() : string.Empty;
            return ok;
        }

        /// <inheritdoc />
        public bool GetPendingDataLength(IntPtr connId, out int length)
        {
            length = 0;
            return Sdk.Agent.HP_Agent_GetPendingDataLength(SenderPtr, connId, ref length) && length > 0;

        }

        /// <inheritdoc />
        public bool GetConnectPeriod(IntPtr connId, out uint period)
        {
            period = 0;
            return Sdk.Agent.HP_Agent_GetConnectPeriod(SenderPtr, connId, ref period);

        }

        /// <inheritdoc />
        public bool GetSilencePeriod(IntPtr connId, out uint period)
        {
            period = 0;
            return Sdk.Agent.HP_Agent_GetSilencePeriod(SenderPtr, connId, ref period);
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
            if (!Sdk.Agent.HP_Agent_GetAllConnectionIDs(SenderPtr, arr, ref count))
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
        public bool GetRemoteHost(IntPtr connId, out string ip, out ushort port)
        {
            var ipLength = 60;
            var sb = new StringBuilder(ipLength);
            port = 0;
            var ok = Sdk.Agent.HP_Agent_GetRemoteHost(SenderPtr, connId, sb, ref ipLength, ref port) && ipLength > 0;
            ip = ok ? sb.ToString() : string.Empty;
            return ok;
        }

        /// <inheritdoc />
        public bool IsConnected(IntPtr connId) => Sdk.Agent.HP_Agent_IsConnected(SenderPtr, connId);

        /// <inheritdoc />
        public bool SetExtra(IntPtr connId, object obj) => ExtraData.Set(connId, obj);

        /// <inheritdoc />
        public T GetExtra<T>(IntPtr connId)
        {
            var obj = ExtraData.Get(connId);
            return obj == null ? default : (T)obj;
        }

        /// <inheritdoc />
        public bool RemoveExtra(IntPtr connId) => ExtraData.Remove(connId);

        /// <inheritdoc />
        public TcpConnectionState GetConnectionState(IntPtr connId)
        {
            if (!NativeGetConnectionExtra(connId, out var nativeId))
            {
                return TcpConnectionState.Closed;
            }

            var nativeExtra = _connNativeData.Get(nativeId.ToInt32());
            if (nativeExtra == null)
            {
                return TcpConnectionState.Closed;
            }

            return nativeExtra.TcpConnectionState;
        }


        #region SDK事件

        #region SDK回调委托,防止GC

        private OnPrepareConnect _onPrepareConnect;
        private OnConnect _onConnect;
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
            _onPrepareConnect = SdkOnPrepareConnect;
            // _onConnect = SdkOnConnect;
            _onConnect = SdkOnConnectNoProxy;
            _onSend = SdkOnSend;
            // _onReceive = SdkOnReceive;
            _onReceive = SdkOnReceiveNoProxy;
            _onClose = SdkOnClose;
            _onShutdown = SdkOnShutdown;
            _onHandShake = SdkOnHandShake;

            Sdk.Agent.HP_Set_FN_Agent_OnPrepareConnect(ListenerPtr, _onPrepareConnect);
            Sdk.Agent.HP_Set_FN_Agent_OnConnect(ListenerPtr, _onConnect);
            Sdk.Agent.HP_Set_FN_Agent_OnSend(ListenerPtr, _onSend);
            Sdk.Agent.HP_Set_FN_Agent_OnReceive(ListenerPtr, _onReceive);
            Sdk.Agent.HP_Set_FN_Agent_OnClose(ListenerPtr, _onClose);
            Sdk.Agent.HP_Set_FN_Agent_OnShutdown(ListenerPtr, _onShutdown);
            Sdk.Agent.HP_Set_FN_Agent_OnHandShake(ListenerPtr, _onHandShake);

            GC.KeepAlive(_onPrepareConnect);
            GC.KeepAlive(_onConnect);
            GC.KeepAlive(_onSend);
            GC.KeepAlive(_onReceive);
            GC.KeepAlive(_onClose);
            GC.KeepAlive(_onShutdown);
            GC.KeepAlive(_onHandShake);
        }

        private static bool TrySetTcpConnected(NativeExtra nativeExtra)
        {
            return nativeExtra != null &&
                   Interlocked.CompareExchange(
                       ref nativeExtra.TcpConnectionStateRef,
                       (int)TcpConnectionState.Connected,
                       (int)TcpConnectionState.Connecting) == (int)TcpConnectionState.Connecting;
        }

        protected HandleResult SdkOnPrepareConnect(IntPtr sender, IntPtr connId, IntPtr socket)
        {
            // 同步接收超时
            if (SyncRecvTimeout > 0 && !Async)
            {
                Sys.SYS_SSO_RecvTimeOut(socket, SyncRecvTimeout);
            }

            // 直接发送策略同时设置NoDelay
            if (SendPolicy == SendPolicy.Direct)
            {
                Sys.SYS_SSO_NoDelay(socket, true);
            }

            return OnPrepareConnect?.Invoke(this, connId, socket) ?? HandleResult.Ignore;
        }

        protected HandleResult SdkOnConnectNoProxy(IntPtr sender, IntPtr connId)
        {
            if (!NativeGetConnectionExtra(connId, out var noProxyNativeId))
            {
                return HandleResult.Ignore;
            }

            var noProxyNativeExtra = _connNativeData.Get(noProxyNativeId.ToInt32());
            // 仅当仍是 Connecting 时才触发 OnConnect, 防止与超时回调的 TimedOut 写入竞争
            if (!TrySetTcpConnected(noProxyNativeExtra))
            {
                return HandleResult.Ignore;
            }

            CancelConnectionTimer(connId, noProxyNativeExtra);
            return OnConnect?.Invoke(this, connId, null) ?? HandleResult.Ignore;
        }

        protected HandleResult SdkOnConnect(IntPtr sender, IntPtr connId)
        {
            var hasProxy = _proxyList?.Count > 0;
            if (hasProxy)
            {
                if (!NativeGetConnectionExtra(connId, out var nativeId))
                {
                    return HandleResult.Error;
                }

                var nativeExtra = _connNativeData.Get(nativeId.ToInt32());
                if (nativeExtra == null)
                {
                    return HandleResult.Error;
                }

                var proxy = nativeExtra.Proxy;
                if (proxy == null)
                {
                    return HandleResult.Error;
                }

                switch (nativeExtra.ProxyConnectionState)
                {
                    case ProxyConnectionState.Step1:
                        {
                            byte[] data;
                            switch (proxy)
                            {
                                case IHttpProxy httpProxy:
                                    data = httpProxy.GetConnectData();
                                    break;
                                case ISocks5Proxy socks5Proxy:
                                    data = socks5Proxy.GetConnectData();
                                    break;
                                default:
                                    return HandleResult.Error;
                            }

                            if (!Send(connId, data, data.Length))
                            {
                                return HandleResult.Error;
                            }

                            break;
                        }
                    default:
                        {
                            return HandleResult.Error;
                        }
                }

                return HandleResult.Ok;
            }

            if (!NativeGetConnectionExtra(connId, out var noProxyNativeId))
            {
                return HandleResult.Ignore;
            }

            var noProxyNativeExtra = _connNativeData.Get(noProxyNativeId.ToInt32());
            // 仅当仍是 Connecting 时才触发 OnConnect, 防止与超时回调的 TimedOut 写入竞争
            if (!TrySetTcpConnected(noProxyNativeExtra))
            {
                return HandleResult.Ignore;
            }

            CancelConnectionTimer(connId, noProxyNativeExtra);
            return OnConnect?.Invoke(this, connId, null) ?? HandleResult.Ignore;
        }

        protected HandleResult SdkOnReceiveNoProxy(IntPtr sender, IntPtr connId, IntPtr data, int length)
        {
            var bytes = new byte[length];
            if (bytes.Length > 0)
            {
                Marshal.Copy(data, bytes, 0, length);
            }

            return OnReceive?.Invoke(this, connId, bytes) ?? HandleResult.Ignore;
        }

        protected HandleResult SdkOnReceive(IntPtr sender, IntPtr connId, IntPtr data, int length)
        {
            var bytes = new byte[length];
            if (bytes.Length > 0)
            {
                Marshal.Copy(data, bytes, 0, length);
            }

            if (_proxyList?.Count > 0 && NativeGetConnectionExtra(connId, out var nativeId))
            {
                var nativeExtra = _connNativeData.Get(nativeId.ToInt32());
                if (nativeExtra == null)
                {
                    return HandleResult.Error;
                }

                if (nativeExtra.ProxyConnectionState == ProxyConnectionState.Normal)
                {
                    return OnReceive?.Invoke(this, connId, bytes) ?? HandleResult.Ignore;
                }

                var proxy = nativeExtra.Proxy;
                if (proxy == null)
                {
                    return HandleResult.Error;
                }
                switch (nativeExtra.ProxyConnectionState)
                {
                    case ProxyConnectionState.Step1:
                        {
                            if (proxy is HttpProxy httpProxy)
                            {
                                if (!httpProxy.IsConnected(bytes))
                                {
                                    return HandleResult.Error;
                                }

                                // 仅当仍是 Connecting 时才触发 OnConnect, 防止与超时回调的 TimedOut 写入竞争
                                if (!TrySetTcpConnected(nativeExtra))
                                {
                                    return HandleResult.Ignore;
                                }

                                CancelConnectionTimer(connId, nativeExtra);
                                nativeExtra.ProxyConnectionState = ProxyConnectionState.Normal;
                                // _connNativeData.Remove(nativeId.ToInt32());
                                // NativeSetConnectionExtra(connId, IntPtr.Zero);

                                OnProxyConnected?.Invoke(this, connId, proxy);

                                if (OnConnect?.Invoke(this, connId, proxy) == HandleResult.Error)
                                {
                                    return HandleResult.Error;
                                }
                            }
                            else
                            {
                                if (!(proxy is ISocks5Proxy socks5Proxy))
                                {
                                    return HandleResult.Error;
                                }

                                if (!socks5Proxy.GetAuthenticateData(bytes, out var sendBytes))
                                {
                                    return HandleResult.Error;
                                }

                                if (sendBytes?.Length > 0)
                                {
                                    if (!Send(connId, sendBytes, sendBytes.Length))
                                    {
                                        return HandleResult.Error;
                                    }

                                    nativeExtra.ProxyConnectionState = ProxyConnectionState.Step2;
                                }
                                else
                                {
                                    if (!socks5Proxy.GetConnectRemoteServerData(out sendBytes))
                                    {
                                        return HandleResult.Error;
                                    }

                                    if (!Send(connId, sendBytes, sendBytes.Length))
                                    {
                                        return HandleResult.Error;
                                    }

                                    nativeExtra.ProxyConnectionState = ProxyConnectionState.Step3;
                                }
                            }

                            break;
                        }
                    case ProxyConnectionState.Step2:
                        {
                            if (!(proxy is ISocks5Proxy socks5Proxy))
                            {
                                return HandleResult.Error;
                            }
                            if (!socks5Proxy.CheckSubVersion(bytes))
                            {
                                return HandleResult.Error;
                            }

                            if (!socks5Proxy.GetConnectRemoteServerData(out var sendBytes))
                            {
                                return HandleResult.Error;
                            }

                            if (!Send(connId, sendBytes, sendBytes.Length))
                            {
                                return HandleResult.Error;
                            }

                            nativeExtra.ProxyConnectionState = ProxyConnectionState.Step3;

                            break;
                        }
                    case ProxyConnectionState.Step3:
                        {
                            if (!(proxy is ISocks5Proxy socks5Proxy))
                            {
                                return HandleResult.Error;
                            }
                            if (!socks5Proxy.IsConnected(bytes))
                            {
                                return HandleResult.Error;
                            }

                            // 仅当仍是 Connecting 时才触发 OnConnect, 防止与超时回调的 TimedOut 写入竞争
                            if (!TrySetTcpConnected(nativeExtra))
                            {
                                return HandleResult.Ignore;
                            }

                            CancelConnectionTimer(connId, nativeExtra);
                            nativeExtra.ProxyConnectionState = ProxyConnectionState.Normal;
                            // _connNativeData.Remove(nativeId.ToInt32());
                            // NativeSetConnectionExtra(connId, IntPtr.Zero);

                            OnProxyConnected?.Invoke(this, connId, proxy);

                            if (OnConnect?.Invoke(this, connId, proxy) == HandleResult.Error)
                            {
                                return HandleResult.Error;
                            }
                            break;
                        }
                }

                return HandleResult.Ok;
            }


            return OnReceive?.Invoke(this, connId, bytes) ?? HandleResult.Ignore;
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

        protected HandleResult SdkOnClose(IntPtr sender, IntPtr connId, SocketOperation socketOperation, int errorCode)
        {
            if (NativeGetConnectionExtra(connId, out var nativeId))
            {
                var nativeExtra = _connNativeData.Get(nativeId.ToInt32());
                if (nativeExtra != null)
                {
                    var previousState = (TcpConnectionState)Interlocked.Exchange(
                        ref nativeExtra.TcpConnectionStateRef,
                        (int)TcpConnectionState.Closed);
                    if (previousState == TcpConnectionState.TimedOut)
                    {
                        socketOperation = SocketOperation.TimedOut;
                        errorCode = (int)socketOperation;
                    }
                }
                _connNativeData.Remove(nativeId.ToInt32());
                NativeSetConnectionExtra(connId, IntPtr.Zero);
            }

            CancelConnectionTimer(connId);

            return OnClose?.Invoke(this, connId, socketOperation, errorCode) ?? HandleResult.Ignore;

        }

        protected HandleResult SdkOnShutdown(IntPtr sender) => OnShutdown?.Invoke(this) ?? HandleResult.Ignore;

        protected HandleResult SdkOnHandShake(IntPtr sender, IntPtr connId) => OnHandShake?.Invoke(this, connId) ?? HandleResult.Ignore;


        #endregion

        #region 代理获取方法

        /// <summary>
        /// 随机获取代理服务器
        /// </summary>
        /// <returns></returns>
        private IProxy GetRandomProxyServer()
        {
            return _proxyList?[new Random(Guid.NewGuid().GetHashCode()).Next(0, _proxyList.Count)];
        }

        #endregion

        /// <summary>
        /// 延迟等待连接超时
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="nativeExtra"></param>
        private void DelayWaitConnectTimeout(IntPtr connId, NativeExtra nativeExtra)
        {
            if (!Async) return;

            // 清理该 connId 可能残留的旧定时器
            if (_connectionTimers.Remove(connId, out var oldTimer) && oldTimer != null)
            {
                oldTimer.Dispose();
            }

            // 得到设置的超时时间
            var connectionTimer = new ConnectionTimeoutTimer(nativeExtra, _connectionTimeout);
            var timer = connectionTimer.Timer;

            timer.Elapsed += (sender, args) =>
            {
                // 已释放或定时器已被替换,直接返回
                if (_disposed || !ReferenceEquals(_connectionTimers.Get(connId), connectionTimer)) return;

                // CAS: 仅当仍是 Connecting 时才转为 TimedOut, 防止与 SdkOnConnect 的 Connected 写入竞争
                if (Interlocked.CompareExchange(
                        ref nativeExtra.TcpConnectionStateRef,
                        (int)TcpConnectionState.TimedOut,
                        (int)TcpConnectionState.Connecting) == (int)TcpConnectionState.Connecting)
                {
                    // 仅当本 timer 仍归该 connId 和 NativeExtra 所有时才能断开连接
                    if (CancelConnectionTimer(connId, connectionTimer))
                    {
                        Disconnect(connId);
                    }
                }
                else
                {
                    CancelConnectionTimer(connId, connectionTimer);
                }
            };

            if (!_connectionTimers.Set(connId, connectionTimer))
            {
                connectionTimer.Dispose();
                return;
            }

            if (nativeExtra.TcpConnectionState != TcpConnectionState.Connecting)
            {
                CancelConnectionTimer(connId, connectionTimer);
                return;
            }

            connectionTimer.Start();
        }

        private void CancelConnectionTimer(IntPtr connId)
        {
            if (_connectionTimers.Remove(connId, out var connectionTimer))
            {
                connectionTimer?.Dispose();
            }
        }

        private bool CancelConnectionTimer(IntPtr connId, ConnectionTimeoutTimer connectionTimer)
        {
            if (!_connectionTimers.Remove(connId, connectionTimer))
            {
                return false;
            }

            connectionTimer.Dispose();
            return true;
        }

        private void CancelConnectionTimer(IntPtr connId, NativeExtra nativeExtra)
        {
            while (true)
            {
                var connectionTimer = _connectionTimers.Get(connId);
                if (connectionTimer == null || !ReferenceEquals(connectionTimer.NativeExtra, nativeExtra))
                {
                    return;
                }

                if (CancelConnectionTimer(connId, connectionTimer))
                {
                    return;
                }
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            // 提前置位,让连接超时定时器回调能在本机资源释放前及时退出
            _disposed = true;

            if (disposing)
            {
                // 释放托管对象资源
#if !NET20 && !NET30 && !NET35
                SysErrorCode?.Dispose();
#endif
            }

            Destroy();
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
