using System;
using System.Collections.Generic;

namespace HPSocket.Tcp
{

    public sealed class TcpPortForwarding : ITcpPortForwarding
    {
        #region 私有成员

        /// <summary>
        /// 是否释放了
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// 是否初始化了
        /// </summary>
        private bool _init;

        /// <summary>
        /// server
        /// </summary>
        private TcpServer _server;

        /// <summary>
        /// agent
        /// </summary>
        private TcpAgent _agent;

        #endregion

        #region 公有成员

        #region 事件

        /// <inheritdoc />
        public event ServerAcceptEventHandler OnServerAccept;

        /// <inheritdoc />
        public event ServerReceiveEventHandler OnServerReceive;

        /// <inheritdoc />
        public event ServerCloseEventHandler OnServerClose;

        /// <inheritdoc />
        public event AgentConnectEventHandler OnAgentConnect;

        /// <inheritdoc />
        public event AgentReceiveEventHandler OnAgentReceive;

        /// <inheritdoc />
        public event AgentCloseEventHandler OnAgentClose;

        #endregion

        #region 属性

        /// <summary>
        /// 该属性不适用在当前组件
        /// </summary>
        public IntPtr SenderPtr => IntPtr.Zero;

        /// <summary>
        /// 当前组件版本
        /// </summary>
        public string Version => Sdk.Sys.GetVersion();

        /// <inheritdoc />
        public string LocalBindAddress { get; set; } = "0.0.0.0";

        /// <inheritdoc />
        public ushort LocalBindPort { get; set; }

        /// <inheritdoc />
        public string TargetAddress { get; set; }

        /// <inheritdoc />
        public ushort TargetPort { get; set; }

        /// <inheritdoc />
        public uint EachWorkThreadCount { get; set; } = (uint)Environment.ProcessorCount * 2 + 2;

        /// <inheritdoc />
        public uint MaxConnectionCount { get; set; } = 10000;

        /// <inheritdoc />
        public int ConnectionTimeout { get; set; } = 0;

        /// <inheritdoc />
        public List<IProxy> ProxyList { get; set; }

        /// <inheritdoc />
        public SocketError ErrorCode { get; set; }

        /// <inheritdoc />
        public string ErrorMessage { get; set; }

        #endregion

        #endregion

        #region 私有方法

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            lock (this)
            {
                if (_init)
                {
                    return;
                }

                _server = new TcpServer
                {
                    Address = LocalBindAddress,
                    Port = LocalBindPort,
                    MaxConnectionCount = MaxConnectionCount,
                    WorkerThreadCount = EachWorkThreadCount,
                    SendPolicy = SendPolicy.Direct,
                };
                _server.OnAccept += ServerAccept;
                _server.OnReceive += ServerReceive;
                _server.OnClose += ServerClose;

                _agent = new TcpAgent
                {
                    Async = true,
                    Address = LocalBindAddress,
                    ConnectionTimeout = ConnectionTimeout,
                    MaxConnectionCount = MaxConnectionCount,
                    WorkerThreadCount = EachWorkThreadCount,
                    SendPolicy = SendPolicy.Direct,
                    ProxyList = ProxyList,
                };
                _agent.OnConnect += AgentConnect;
                _agent.OnReceive += AgentReceive;
                _agent.OnClose += AgentClose;

                _init = true;
            }
        }


        /// <summary>
        /// 设置错误信息
        /// </summary>
        /// <param name="agent"></param>
        private void SetErrorInfo(IAgent agent)
        {
            ErrorCode = agent.ErrorCode;
            ErrorMessage = agent.ErrorMessage;
        }

        /// <summary>
        /// 设置错误信息
        /// </summary>
        /// <param name="server"></param>
        private void SetErrorInfo(IServer server)
        {
            ErrorCode = server.ErrorCode;
            ErrorMessage = server.ErrorMessage;
        }

        #region server组件回调

        private HandleResult ServerAccept(IServer sender, IntPtr connId, IntPtr client)
        {
            // 暂停接收数据
            if (!sender.PauseReceive(connId))
            {
                SetErrorInfo(sender);
                return HandleResult.Error;
            }

            // 设置附加数据到 server & agent
            var extra = new TcpPortForwardingExtra
            {
                ServerConnId = connId,
                Server = sender,
                Agent = _agent,
                ReleaseType = TcpPortForwardingReleaseType.None,
            };

            if (!sender.SetExtra(connId, extra))
            {
                return HandleResult.Error;
            }

            if (!_agent.Connect(TargetAddress, TargetPort, connId, out var agentConnId))
            {
                SetErrorInfo(sender);
                return HandleResult.Error;
            }

            extra.AgentConnId = agentConnId;
            if (!_agent.SetExtra(agentConnId, extra))
            {
                return HandleResult.Error;
            }

            return OnServerAccept?.Invoke(sender, connId, client) ?? HandleResult.Ok;
        }

        private HandleResult ServerReceive(IServer sender, IntPtr connId, byte[] data)
        {
            var extra = sender.GetExtra<TcpPortForwardingExtra>(connId);
            if (extra == null)
            {
                return HandleResult.Error;
            }

            if (!extra.Agent.Send(extra.AgentConnId, data, data.Length))
            {
                return HandleResult.Error;
            }

            return OnServerReceive?.Invoke(sender, connId, data) ?? HandleResult.Ok;
        }

        private HandleResult ServerClose(IServer sender, IntPtr connId, SocketOperation socketOperation, int errorCode)
        {
            var extra = sender.GetExtra<TcpPortForwardingExtra>(connId);
            if (extra != null)
            {
                sender.RemoveExtra(connId);

                if (extra.ReleaseType == TcpPortForwardingReleaseType.None)
                {
                    extra.ReleaseType = TcpPortForwardingReleaseType.Server;
                    extra.Agent.Disconnect(extra.AgentConnId);
                }
            }

            return OnServerClose?.Invoke(sender, connId, socketOperation, errorCode) ?? HandleResult.Ok;
        }

        #endregion

        #region agent组件回调

        private HandleResult AgentConnect(IAgent sender, IntPtr connId, IProxy proxy)
        {
            if (!sender.GetConnectionExtra(connId, out var serverConnId) || serverConnId == IntPtr.Zero)
            {
                SetErrorInfo(sender);
                return HandleResult.Error;
            }

            var extra = _server.GetExtra<TcpPortForwardingExtra>(serverConnId);
            if (extra == null)
            {
                return HandleResult.Error;
            }

            if (!extra.Server.ResumeReceive(extra.ServerConnId))
            {
                return HandleResult.Error;
            }

            return OnAgentConnect?.Invoke(sender, connId, proxy) ?? HandleResult.Ok;
        }

        private HandleResult AgentReceive(IAgent sender, IntPtr connId, byte[] data)
        {
            var extra = _server.GetExtra<TcpPortForwardingExtra>(connId);
            if (extra == null)
            {
                return HandleResult.Error;
            }

            if (!extra.Server.Send(extra.ServerConnId, data, data.Length))
            {
                return HandleResult.Error;
            }
            return OnAgentReceive?.Invoke(sender, connId, data) ?? HandleResult.Ok;
        }

        private HandleResult AgentClose(IAgent sender, IntPtr connId, SocketOperation socketOperation, int errorCode)
        {
            var extra = sender.GetExtra<TcpPortForwardingExtra>(connId);
            if (extra == null)
            {
                if (sender.GetConnectionExtra(connId, out var serverConnId) && serverConnId != IntPtr.Zero)
                {
                    _server.Disconnect(serverConnId);
                }

            }
            else
            {
                sender.RemoveExtra(connId);
                if (extra.ReleaseType == TcpPortForwardingReleaseType.None)
                {
                    extra.ReleaseType = TcpPortForwardingReleaseType.Agent;
                    extra.Server.Disconnect(extra.ServerConnId);
                }
            }

            return OnAgentClose?.Invoke(sender, connId, socketOperation, errorCode) ?? HandleResult.Ok;
        }

        #endregion

        #endregion

        #region 公有方法

        ~TcpPortForwarding() => Dispose(false);

        /// <inheritdoc />
        public bool Start()
        {
            Init();

            if (!_agent.Start())
            {
                ErrorCode = _agent.ErrorCode;
                ErrorMessage = _agent.ErrorMessage;
                return false;
            }

            if (!_server.Start())
            {
                ErrorCode = _server.ErrorCode;
                ErrorMessage = _server.ErrorMessage;
                _agent.Stop();
                return false;
            }

            return true;
        }

        /// <inheritdoc />
        public void Stop()
        {
            _agent.Stop();
            _server.Stop();
        }

        #endregion

        #region 释放资源

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                // 释放托管对象资源
                _agent.Dispose();
                _server.Dispose();
            }

            _disposed = true;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

    }
}
