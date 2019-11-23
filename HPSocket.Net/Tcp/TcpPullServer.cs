using System;
using System.Runtime.InteropServices;

namespace HPSocket.Tcp
{
    /// <summary>
    /// tcp pull server
    /// </summary>
    public class TcpPullServer : TcpServer, ITcpPullServer
    {
        public TcpPullServer()
            : base(Sdk.Tcp.Create_HP_TcpPullServerListener,
                Sdk.Tcp.Create_HP_TcpPullServer,
                Sdk.Tcp.Destroy_HP_TcpPullServer,
                Sdk.Tcp.Destroy_HP_TcpPullServerListener)
        {
        }

        protected TcpPullServer(Sdk.CreateListenerDelegate createListenerFunction, Sdk.CreateServiceDelegate createServiceFunction, Sdk.DestroyListenerDelegate destroyServiceFunction, Sdk.DestroyListenerDelegate destroyListenerFunction)
            : base(createListenerFunction, createServiceFunction, destroyServiceFunction, destroyListenerFunction)
        {
        }

        /// <inheritdoc />
        public new event PullServerReceiveEventHandler OnReceive;

        /// <inheritdoc />
        public FetchResult Fetch(IntPtr connId, IntPtr buffer, int length) => Sdk.Tcp.HP_TcpPullServer_Fetch(SenderPtr, connId, buffer, length);

        /// <inheritdoc />
        public FetchResult Peek(IntPtr connId, IntPtr buffer, int length) => Sdk.Tcp.HP_TcpPullServer_Peek(SenderPtr, connId, buffer, length);

        /// <inheritdoc />
        public FetchResult Fetch(IntPtr connId, int length, out byte[] bytes)
        {
            var buffer = IntPtr.Zero;
            try
            {
                bytes = null;
                buffer = Marshal.AllocHGlobal(length);
                var fr = Fetch(connId, buffer, length);
                if (fr != FetchResult.Ok) return fr;
                bytes = new byte[length];
                Marshal.Copy(buffer, bytes, 0, bytes.Length);
                return fr;
            }
            finally
            {
                if (buffer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(buffer);
                }
            }
        }

        /// <inheritdoc />
        public FetchResult Peek(IntPtr connId, int length, out byte[] bytes)
        {
            var buffer = IntPtr.Zero;
            try
            {
                bytes = null;
                buffer = Marshal.AllocHGlobal(length);
                var fr = Peek(connId, buffer, length);
                if (fr != FetchResult.Ok) return fr;
                bytes = new byte[length];
                Marshal.Copy(buffer, bytes, 0, bytes.Length);
                return fr;
            }
            finally
            {
                if (buffer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(buffer);
                }
            }
        }

        #region SDK事件

        #region SDK回调委托,防止GC

        private Sdk.OnPullReceive _onPullReceive;

        #endregion
        protected override void SetCallback()
        {
            _onPullReceive = SdkOnReceive;

            Sdk.Server.HP_Set_FN_Server_OnPullReceive(ListenerPtr, _onPullReceive);

            GC.KeepAlive(_onPullReceive);

            base.SetCallback();
        }
        protected HandleResult SdkOnReceive(IntPtr sender, IntPtr connId, int length) => OnReceive?.Invoke(this, connId, length) ?? HandleResult.Ignore;

        #endregion
    }
}
