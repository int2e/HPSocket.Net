using HPSocket.Adapter;
using System;
using System.Runtime.InteropServices;

namespace HPSocket.Tcp
{
    /// <summary>
    /// tcp client
    /// </summary>
    /// <typeparam name="TRequestBodyType">包体解析对象类型</typeparam>
    public class TcpClient<TRequestBodyType> : TcpClient, ITcpClient<TRequestBodyType>
    {
        public TcpClient()
        {
            OnConnect += TcpClient_OnConnect;
            base.OnReceive += TcpClient_OnReceive;
            OnClose += TcpClient_OnClose;
        }
        
#pragma warning disable 0067
        [Obsolete("adapter组件无需添加OnReceive事件, 请添加OnParseRequestBody事件", true)]
        public new event ClientReceiveEventHandler OnReceive;
#pragma warning restore

        /// <inheritdoc />
        public event ParseRequestBody<ITcpClient, TRequestBodyType> OnParseRequestBody;

        /// <inheritdoc />
        public DataReceiveAdapter<TRequestBodyType> DataReceiveAdapter { get; set; }

        private HandleResult TcpClient_OnConnect(IClient sender)
        {
            DataReceiveAdapter.OnOpen(ConnectionId);
            return HandleResult.Ok;
        }

        private HandleResult TcpClient_OnReceive(IClient sender, byte[] data)
        {
            return DataReceiveAdapter.OnReceive(this, ConnectionId, data, OnParseRequestBody);
        }

        private HandleResult TcpClient_OnClose(IClient sender, SocketOperation socketOperation, int errorCode)
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


    /// <summary>
    /// tcp client
    /// </summary>
    public class TcpClient : Base.Client, ITcpClient
    {
        public TcpClient()
            : base(Sdk.Tcp.Create_HP_TcpClientListener,
                Sdk.Tcp.Create_HP_TcpClient,
                Sdk.Tcp.Destroy_HP_TcpClient,
                Sdk.Tcp.Destroy_HP_TcpClientListener)
        {
        }

        protected TcpClient(Sdk.CreateListenerDelegate createListenerFunction, Sdk.CreateServiceDelegate createServiceFunction, Sdk.DestroyListenerDelegate destroyServiceFunction, Sdk.DestroyListenerDelegate destroyListenerFunction)
            : base(createListenerFunction, createServiceFunction, destroyServiceFunction, destroyListenerFunction)
        {
        }

        /// <inheritdoc />
        public uint SocketBufferSize
        {
            get => Sdk.Tcp.HP_TcpClient_GetSocketBufferSize(SenderPtr);
            set => Sdk.Tcp.HP_TcpClient_SetSocketBufferSize(SenderPtr, value);
        }

        /// <inheritdoc />
        public uint KeepAliveTime
        {
            get => Sdk.Tcp.HP_TcpClient_GetKeepAliveTime(SenderPtr);
            set => Sdk.Tcp.HP_TcpClient_SetKeepAliveTime(SenderPtr, value);
        }

        /// <inheritdoc />
        public uint KeepAliveInterval
        {
            get => Sdk.Tcp.HP_TcpClient_GetKeepAliveInterval(SenderPtr);
            set => Sdk.Tcp.HP_TcpClient_SetKeepAliveInterval(SenderPtr, value);
        }

        /// <inheritdoc />
        public bool NoDelay
        {
            get => Sdk.Tcp.HP_TcpClient_IsNoDelay(SenderPtr);
            set => Sdk.Tcp.HP_TcpClient_SetNoDelay(SenderPtr, value);
        }

        /// <inheritdoc />
        public bool SendSmallFile(string filePath, ref Wsabuf head, ref Wsabuf tail)
        {
            var ok = Sdk.Tcp.HP_TcpClient_SendSmallFile(SenderPtr, filePath, ref head, ref tail);
#if !NET20 && !NET30 && !NET35
            SysErrorCode.Value = ok ? 0 : Sdk.Sys.SYS_GetLastError();
#endif
            return ok;
        }

        /// <inheritdoc />
        public bool SendSmallFile(string filePath, byte[] head, byte[] tail)
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

            var ok = SendSmallFile(filePath, ref wsaHead, ref wsaTail);
            gchHead.Free();
            gchTail.Free();

            return ok;
        }
    }
}
