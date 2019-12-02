using System;
using System.Runtime.InteropServices;

namespace HPSocket.Tcp
{
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
        public bool SendSmallFile(IntPtr connId, string filePath, ref Wsabuf head, ref Wsabuf tail) => Sdk.Tcp.HP_TcpAgent_SendSmallFile(SenderPtr, connId, filePath, ref head, ref tail);


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
