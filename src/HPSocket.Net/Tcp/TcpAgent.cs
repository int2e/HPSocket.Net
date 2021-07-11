using System;
using System.Runtime.InteropServices;

using HPSocket.Adapter;

namespace HPSocket.Tcp
{
    /// <summary>
    /// tcp agent
    /// </summary>
    /// <typeparam name="TRequestBodyType">包体解析对象类型</typeparam>
    public class TcpAgent<TRequestBodyType> : TcpAgent, ITcpAgent<TRequestBodyType>
    {
        public TcpAgent()
        {
            OnConnect += TcpAgent_OnConnect;
            base.OnReceive += TcpAgent_OnReceive;
            OnClose += TcpAgent_OnClose;
        }

#pragma warning disable 0067
        [Obsolete("adapter组件无需添加OnReceive事件, 请添加OnParseRequestBody事件", true)]
        public new event AgentReceiveEventHandler OnReceive;
#pragma warning restore

        /// <inheritdoc />
        public event ParseRequestBody<ITcpAgent, TRequestBodyType> OnParseRequestBody;

        /// <inheritdoc />
        public DataReceiveAdapter<TRequestBodyType> DataReceiveAdapter { get; set; }

        private HandleResult TcpAgent_OnConnect(IAgent sender, IntPtr connId, IProxy proxy)
        {
            DataReceiveAdapter.OnOpen(connId);
            return HandleResult.Ok;
        }

        private HandleResult TcpAgent_OnReceive(IAgent sender, IntPtr connId, byte[] data)
        {
            return DataReceiveAdapter.OnReceive(this, connId, data, OnParseRequestBody);
        }

        private HandleResult TcpAgent_OnClose(IAgent sender, IntPtr connId, SocketOperation socketOperation, int errorCode)
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


    /// <summary>
    /// tcp agent
    /// </summary>
    public class TcpAgent : Base.Agent, ITcpAgent
    {
        public TcpAgent()
            : base(Sdk.Tcp.Create_HP_TcpAgentListener,
                Sdk.Tcp.Create_HP_TcpAgent,
                Sdk.Tcp.Destroy_HP_TcpAgent,
                Sdk.Tcp.Destroy_HP_TcpAgentListener)
        {
        }

        protected TcpAgent(Sdk.CreateListenerDelegate createListenerFunction, Sdk.CreateServiceDelegate createServiceFunction, Sdk.DestroyListenerDelegate destroyServiceFunction, Sdk.DestroyListenerDelegate destroyListenerFunction)
            : base(createListenerFunction, createServiceFunction, destroyServiceFunction, destroyListenerFunction)
        {
        }

        /// <inheritdoc />
        public uint SocketBufferSize
        {
            get => Sdk.Tcp.HP_TcpAgent_GetSocketBufferSize(SenderPtr);
            set => Sdk.Tcp.HP_TcpAgent_SetSocketBufferSize(SenderPtr, value);
        }

        /// <inheritdoc />
        public uint KeepAliveTime
        {
            get => Sdk.Tcp.HP_TcpAgent_GetKeepAliveTime(SenderPtr);
            set => Sdk.Tcp.HP_TcpAgent_SetKeepAliveTime(SenderPtr, value);
        }

        /// <inheritdoc />
        public uint KeepAliveInterval
        {
            get => Sdk.Tcp.HP_TcpAgent_GetKeepAliveInterval(SenderPtr);
            set => Sdk.Tcp.HP_TcpAgent_SetKeepAliveInterval(SenderPtr, value);
        }

        /// <inheritdoc />
        public bool NoDelay 
        {
            get => Sdk.Tcp.HP_TcpAgent_IsNoDelay(SenderPtr);
            set => Sdk.Tcp.HP_TcpAgent_SetNoDelay(SenderPtr, value);
        }

        /// <inheritdoc />
        public bool SendSmallFile(IntPtr connId, string filePath, ref Wsabuf head, ref Wsabuf tail)
        {
            var ok = Sdk.Tcp.HP_TcpAgent_SendSmallFile(SenderPtr, connId, filePath, ref head, ref tail);
#if !NET20 && !NET30 && !NET35
            SysErrorCode.Value = ok ? 0 : Sdk.Sys.SYS_GetLastError();
#endif
            return ok;
        }


        /// <inheritdoc />
        public bool SendSmallFile(IntPtr connId, string filePath, byte[] head, byte[] tail)
        {
            var wsaHead = new Wsabuf { Length = 0, Buffer = IntPtr.Zero };
            var wsaTail = new Wsabuf { Length = 0, Buffer = IntPtr.Zero };
            var gchHead = GCHandle.Alloc(head, GCHandleType.Pinned);
            var gchTail = GCHandle.Alloc(tail, GCHandleType.Pinned);
            if (head != null)
            {
                wsaHead.Length = head.Length;
                wsaHead.Buffer = gchHead.AddrOfPinnedObject();
            }

            if (tail != null)
            {
                wsaTail.Length = tail.Length;
                wsaTail.Buffer = gchTail.AddrOfPinnedObject();
            }

            var ok = SendSmallFile(connId, filePath, ref wsaHead, ref wsaTail);
            gchHead.Free();
            gchTail.Free();
            return ok;
        }
    }
}
