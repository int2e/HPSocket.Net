using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using HPSocket.Proxy;
#if !NET20 && !NET30 && !NET35
using System.Threading.Tasks;
#endif
using HPSocket.Sdk;
using HPSocket.Tcp;
using System.Collections.Generic;

using Timer = System.Timers.Timer;

namespace HPSocket.Base
{
    public abstract class Client : IClient
    {
        #region 私有成员

        /// <summary>
        /// 是否释放了
        /// </summary>
        private volatile bool _disposed;

        /// <summary>
        /// 代理连接状态
        /// </summary>
        private int _proxyConnectionState;

        /// <summary>
        /// 连接超时时间
        /// </summary>
        private int _connectionTimeout;

        /// <summary>
        /// native data
        /// </summary>
        private NativeExtra _nativeExtra;

        /// <summary>
        /// 连接超时定时器
        /// </summary>
        private Timer _connectionTimer;


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
        /// 代理预连接
        /// </summary>
        protected event ClientProxyPrepareConnectEventHandler OnProxyPrepareConnect;

        /// <summary>
        /// 代理已连接
        /// </summary>
        protected event ClientProxyConnectedEventHandler OnProxyConnected;

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
        public int ConnectionTimeout
        {
            get => _connectionTimeout;

            // ReSharper disable once ValueParameterNotUsed
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "不能设置小于0的值");
                }

                var milliseconds = value;
                if (milliseconds != 0 && (milliseconds < 100 || milliseconds % 100 != 0))
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "超时时间毫秒数请设置为100的倍数, 如: [100,1100,1300]");
                }

                _connectionTimeout = value;
            }
        }

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

        /// <inheritdoc />
        public List<IProxy> ProxyList
        {
            get => _proxyList;
            set
            {
                _proxyList = value;
                _onConnect = SdkOnConnect;
                if (_proxyList?.Count > 0)
                {
                    _onReceive = SdkOnReceive;
                }
                else
                {
                    _onReceive = SdkOnReceiveNoProxy;
                }

                Sdk.Client.HP_Set_FN_Client_OnConnect(ListenerPtr, _onConnect);
                Sdk.Client.HP_Set_FN_Client_OnReceive(ListenerPtr, _onReceive);

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
            // 在释放本机资源前停止连接超时定时器,避免回调访问已释放的 SenderPtr
            var timer = Interlocked.Exchange(ref _connectionTimer, null);
            timer?.Dispose();

            Stop();

            _nativeExtra = null;

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
        public virtual bool Connect()
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

            _nativeExtra = new NativeExtra
            {
                TcpConnectionState = TcpConnectionState.Connecting,
            };

            var address = Address;
            var port = Port;

            if (_proxyList?.Count > 0)
            {
                _proxyConnectionState = 1; // 连接中

                var proxy = GetRandomProxyServer();
                if ((proxy is Socks5Proxy socks5Proxy))
                {
                    socks5Proxy.SetRemoteAddressPort(Address, Port);
                }
                else if ((proxy is HttpProxy httpProxy))
                {
                    httpProxy.SetRemoteAddressPort(Address, Port);
                }
                _nativeExtra.ProxyConnectionState = ProxyConnectionState.Step1;
                _nativeExtra.Proxy = proxy;
                address = proxy.Host;
                port = proxy.Port;
                OnProxyPrepareConnect?.Invoke(this, proxy);
            }
            else
            {
                _proxyConnectionState = 0; // 不使用代理
            }

            bool ok;
            if (!String.IsNullOrWhiteSpace(BindAddress) && BindPort > 0)
            {
                ok = Sdk.Client.HP_Client_StartWithBindAddressAndLocalPort(SenderPtr, address, port, Async, BindAddress, BindPort);
            }
            else if (!String.IsNullOrWhiteSpace(BindAddress))
            {
                ok = Sdk.Client.HP_Client_StartWithBindAddress(SenderPtr, address, port, Async, BindAddress);
            }
            else
            {
                ok = Sdk.Client.HP_Client_Start(SenderPtr, address, port, Async);
            }
            if (ok)
            {
#if !NET20 && !NET30 && !NET35
                SysErrorCode.Value = 0;
#endif
                DelayWaitConnectTimeout();
            }
            else
            {
#if !NET20 && !NET30 && !NET35
                SysErrorCode.Value = Sys.SYS_GetLastError();
#endif
            }

            return ok;
        }

        /// <inheritdoc />
        public virtual bool Connect(string address, ushort port)
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
            SysErrorCode.Value = ok ? 0 : Sys.SYS_GetLastError();
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
            SysErrorCode.Value = ok ? 0 : Sys.SYS_GetLastError();
#endif
            gch.Free();
            return ok;
        }

        /// <inheritdoc />
        public bool SendPackets(Wsabuf[] buffers, int count)
        {
            var ok = Sdk.Client.HP_Client_SendPackets(SenderPtr, buffers, count);
#if !NET20 && !NET30 && !NET35
            SysErrorCode.Value = ok ? 0 : Sys.SYS_GetLastError();
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

#if !NET20 && !NET30 && !NET35 && !NET40
        /// <inheritdoc />
        public async Task<bool> WaitProxyAsync()
        {
            do
            {
                await Task.Delay(10);
            } while (_proxyConnectionState != 3 && _proxyConnectionState != 4);

            return _proxyConnectionState == 3;
        }
#endif


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
            _onReceive = SdkOnReceiveNoProxy;
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

        private static bool TrySetTcpConnected(NativeExtra nativeExtra)
        {
            return nativeExtra != null &&
                   Interlocked.CompareExchange(
                       ref nativeExtra.TcpConnectionStateRef,
                       (int)TcpConnectionState.Connected,
                       (int)TcpConnectionState.Connecting) == (int)TcpConnectionState.Connecting;
        }

        protected HandleResult SdkOnPrepareConnect(IntPtr sender, IntPtr connId, IntPtr socket) => OnPrepareConnect?.Invoke(this, socket) ?? HandleResult.Ignore;

        protected HandleResult SdkOnConnect(IntPtr sender, IntPtr connId)
        {
            var hasProxy = _proxyList?.Count > 0;
            if (hasProxy)
            {
                if (_nativeExtra == null)
                {
                    return HandleResult.Error;
                }

                _proxyConnectionState = 2; // 协商中

                var proxy = _nativeExtra.Proxy;
                if (proxy == null)
                {
                    return HandleResult.Error;
                }

                switch (_nativeExtra.ProxyConnectionState)
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

                            if (!Send(data, data.Length))
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

            if (ConnectionTimeout > 0)
            {
                if (_nativeExtra == null)
                {
                    return HandleResult.Ignore;
                }

                // 仅当仍是 Connecting 时才触发 OnConnect, 防止与超时回调的 TimedOut 写入竞争
                if (!TrySetTcpConnected(_nativeExtra))
                {
                    return HandleResult.Ignore;
                }
                return OnConnect?.Invoke(this) ?? HandleResult.Ignore;
            }

            return OnConnect?.Invoke(this) ?? HandleResult.Ignore;
        }

        protected HandleResult SdkOnReceiveNoProxy(IntPtr sender, IntPtr connId, IntPtr data, int length)
        {
            if (OnReceive == null) return HandleResult.Ignore;
            var bytes = new byte[length];
            if (bytes.Length > 0)
            {
                Marshal.Copy(data, bytes, 0, length);
            }
            return OnReceive(this, bytes);
        }

        protected HandleResult SdkOnReceive(IntPtr sender, IntPtr connId, IntPtr data, int length)
        {
            var bytes = new byte[length];
            if (bytes.Length > 0)
            {
                Marshal.Copy(data, bytes, 0, length);
            }

            if (_proxyList?.Count > 0 && _nativeExtra != null)
            {
                if (_nativeExtra.ProxyConnectionState == ProxyConnectionState.Normal)
                {
                    _nativeExtra = null;
                    return OnReceive?.Invoke(this, bytes) ?? HandleResult.Ignore;
                }

                var proxy = _nativeExtra.Proxy;
                if (proxy == null)
                {
                    return HandleResult.Error;
                }
                switch (_nativeExtra.ProxyConnectionState)
                {
                    case ProxyConnectionState.Step1:
                    {
                        if (proxy is HttpProxy httpProxy)
                        {
                            if (!httpProxy.IsConnected(bytes))
                            {
                                return HandleResult.Error;
                            }

                            if (ConnectionTimeout > 0)
                            {
                                // 仅当仍是 Connecting 时才触发 OnConnect, 防止与超时回调的 TimedOut 写入竞争
                                if (!TrySetTcpConnected(_nativeExtra))
                                {
                                    return HandleResult.Ignore;
                                }
                            }

                            _nativeExtra.ProxyConnectionState = ProxyConnectionState.Normal;

                            OnProxyConnected?.Invoke(this, proxy);

                            _proxyConnectionState = 3; // 已连接

                            if (OnConnect?.Invoke(this) == HandleResult.Error)
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
                                if (!Send(sendBytes, sendBytes.Length))
                                {
                                    return HandleResult.Error;
                                }
                                _nativeExtra.ProxyConnectionState = ProxyConnectionState.Step2;
                            }
                            else
                            {
                                if (!socks5Proxy.GetConnectRemoteServerData(out sendBytes))
                                {
                                    return HandleResult.Error;
                                }

                                if (!Send(sendBytes, sendBytes.Length))
                                {
                                    return HandleResult.Error;
                                }

                                _nativeExtra.ProxyConnectionState = ProxyConnectionState.Step3;
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

                        if (!Send(sendBytes, sendBytes.Length))
                        {
                            return HandleResult.Error;
                        }

                        _nativeExtra.ProxyConnectionState = ProxyConnectionState.Step3;

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

                        if (ConnectionTimeout > 0)
                        {
                            // 仅当仍是 Connecting 时才触发 OnConnect, 防止与超时回调的 TimedOut 写入竞争
                            if (!TrySetTcpConnected(_nativeExtra))
                            {
                                return HandleResult.Ignore;
                            }
                        }

                        _nativeExtra.ProxyConnectionState = ProxyConnectionState.Normal;
                        _nativeExtra = null;
                        OnProxyConnected?.Invoke(this, proxy);

                        _proxyConnectionState = 3; // 已连接

                        if (OnConnect?.Invoke(this) == HandleResult.Error)
                        {
                            return HandleResult.Error;
                        }
                        break;
                    }
                }

                return HandleResult.Ok;
            }

            return OnReceive?.Invoke(this, bytes) ?? HandleResult.Ignore;
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

        protected HandleResult SdkOnClose(IntPtr sender, IntPtr connId, SocketOperation socketOperation, int errorCode)
        {
            if (_proxyConnectionState != 3)
            {
                _proxyConnectionState = 4;
            }

            if (_nativeExtra != null)
            {
                if (_nativeExtra.TcpConnectionState == TcpConnectionState.TimedOut)
                {
                    socketOperation = SocketOperation.TimedOut;
                    errorCode = (int)socketOperation;
                }
                _nativeExtra.TcpConnectionState = TcpConnectionState.Closed;
            }

            return OnClose?.Invoke(this, socketOperation, errorCode) ?? HandleResult.Ignore;
        }

        protected HandleResult SdkOnHandShake(IntPtr sender, IntPtr connId) => OnHandShake?.Invoke(this) ?? HandleResult.Ignore;

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
        private void DelayWaitConnectTimeout()
        {
            if (ConnectionTimeout == 0 || !Async) return;

            // 先清理可能残留的旧定时器(避免多次 Connect 叠加)
            var oldTimer = Interlocked.Exchange(ref _connectionTimer, null);
            oldTimer?.Dispose();

            // 得到设置的超时时间
            var timer = new Timer(_connectionTimeout)
            {
                AutoReset = false
            };
            _connectionTimer = timer;

            timer.Elapsed += (sender, args) =>
            {
                // 已释放或定时器已被替换,直接返回
                if (_disposed || !ReferenceEquals(_connectionTimer, timer)) return;

                if (_nativeExtra != null && _nativeExtra.TcpConnectionState == TcpConnectionState.Connecting)
                {
                    // CAS: 仅当仍是 Connecting 时才转为 TimedOut, 防止与 SdkOnConnect 的 Connected 写入竞争
                    if (Interlocked.CompareExchange(
                            ref _nativeExtra.TcpConnectionStateRef,
                            (int)TcpConnectionState.TimedOut,
                            (int)TcpConnectionState.Connecting) == (int)TcpConnectionState.Connecting)
                    {
                        Stop();
                    }
                }

                // 仅当本 timer 仍是当前定时器时才清理,避免与 Destroy 双重 Dispose
                if (ReferenceEquals(Interlocked.CompareExchange(ref _connectionTimer, null, timer), timer))
                {
                    timer.Stop();
                    timer.Dispose();
                }
            };
            timer.Start();
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
