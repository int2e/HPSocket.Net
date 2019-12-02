using System;
using System.Runtime.InteropServices;

namespace HPSocket.Tcp
{
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
        public bool SendSmallFile(IntPtr connId, string filePath, ref Wsabuf head, ref Wsabuf tail) => Sdk.Tcp.HP_TcpClient_SendSmallFile(SenderPtr, filePath, ref head, ref tail);

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
                wsaHead.Length = tail.Length;
                wsaHead.Buffer = gchTail.AddrOfPinnedObject();
            }

            var ok = SendSmallFile(connId, filePath, ref wsaHead, ref wsaTail);
            gchHead.Free();
            gchTail.Free();

            return ok;
        }
    }
}
