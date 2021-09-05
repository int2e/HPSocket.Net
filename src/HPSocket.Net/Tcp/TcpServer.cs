using System;
using System.Runtime.InteropServices;
using HPSocket.Adapter;


namespace HPSocket.Tcp
{
    /// <summary>
    /// tcp server
    /// </summary>
    /// <typeparam name="TRequestBodyType">包体解析对象类型</typeparam>
    public class TcpServer<TRequestBodyType> : TcpServer, ITcpServer<TRequestBodyType>
    {
        public TcpServer()
        {
            OnAccept += TcpServer_OnAccept;
            base.OnReceive += TcpServer_OnReceive;
            OnClose += TcpServer_OnClose;
        }

#pragma warning disable 0067
        [Obsolete("adapter组件无需添加OnReceive事件, 请添加OnParseRequestBody事件", true)]
        public new event ServerReceiveEventHandler OnReceive;
#pragma warning restore

        /// <inheritdoc />
        public event ParseRequestBody<ITcpServer, TRequestBodyType> OnParseRequestBody;

        /// <inheritdoc />
        public DataReceiveAdapter<TRequestBodyType> DataReceiveAdapter { get; set; }

        private HandleResult TcpServer_OnAccept(IServer sender, IntPtr connId, IntPtr client)
        {
            DataReceiveAdapter.OnOpen(connId);
            return HandleResult.Ok;
        }

        private HandleResult TcpServer_OnReceive(IServer sender, IntPtr connId, byte[] data)
        {
            return DataReceiveAdapter.OnReceive(this, connId, data, OnParseRequestBody);
        }

        private HandleResult TcpServer_OnClose(IServer sender, IntPtr connId, SocketOperation socketOperation, int errorCode)
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
    /// tcp server
    /// </summary>
    public class TcpServer : Base.Server, ITcpServer
    {
        public TcpServer()
            : base(Sdk.Tcp.Create_HP_TcpServerListener,
                Sdk.Tcp.Create_HP_TcpServer,
                Sdk.Tcp.Destroy_HP_TcpServer,
                Sdk.Tcp.Destroy_HP_TcpServerListener)
        {
        }

        protected TcpServer(Sdk.CreateListenerDelegate createListenerFunction, Sdk.CreateServiceDelegate createServiceFunction, Sdk.DestroyListenerDelegate destroyServiceFunction, Sdk.DestroyListenerDelegate destroyListenerFunction)
            : base(createListenerFunction, createServiceFunction, destroyServiceFunction, destroyListenerFunction)
        {
        }

        /// <inheritdoc />
        public uint AcceptSocketCount
        {
            get => Sdk.Tcp.HP_TcpServer_GetAcceptSocketCount(SenderPtr);
            set => Sdk.Tcp.HP_TcpServer_SetAcceptSocketCount(SenderPtr, value);
        }

        /// <inheritdoc />
        public uint SocketBufferSize
        {
            get => Sdk.Tcp.HP_TcpServer_GetSocketBufferSize(SenderPtr);
            set => Sdk.Tcp.HP_TcpServer_SetSocketBufferSize(SenderPtr, value);
        }

        /// <inheritdoc />
        public uint SocketListenQueue
        {
            get => Sdk.Tcp.HP_TcpServer_GetSocketListenQueue(SenderPtr);
            set => Sdk.Tcp.HP_TcpServer_SetSocketListenQueue(SenderPtr, value);
        }

        /// <inheritdoc />
        public uint KeepAliveTime
        {
            get => Sdk.Tcp.HP_TcpServer_GetKeepAliveTime(SenderPtr);
            set => Sdk.Tcp.HP_TcpServer_SetKeepAliveTime(SenderPtr, value);
        }

        /// <inheritdoc />
        public uint KeepAliveInterval
        {
            get => Sdk.Tcp.HP_TcpServer_GetKeepAliveInterval(SenderPtr);
            set => Sdk.Tcp.HP_TcpServer_SetKeepAliveInterval(SenderPtr, value);
        }

        /// <inheritdoc />
        public bool NoDelay
        {
            get => Sdk.Tcp.HP_TcpServer_IsNoDelay(SenderPtr);
            set => Sdk.Tcp.HP_TcpServer_SetNoDelay(SenderPtr, value);
        }

        /// <inheritdoc />
        public bool SendSmallFile(IntPtr connId, string filePath, ref Wsabuf head, ref Wsabuf tail)
        {
            var ok = Sdk.Tcp.HP_TcpServer_SendSmallFile(SenderPtr, connId, filePath, ref head, ref tail);
#if !NET20 && !NET30 && !NET35
            SysErrorCode.Value = ok ? 0 : Sdk.Sys.SYS_GetLastError();
#endif
            return ok;
        }

        /// <inheritdoc />
        public bool SendSmallFile(IntPtr connId, string filePath, byte[] head, byte[] tail)
        {
            var wsaHead = new Wsabuf() { Length = 0, Buffer = IntPtr.Zero };
            var wsaTail = new Wsabuf() { Length = 0, Buffer = IntPtr.Zero };

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
