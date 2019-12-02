using System;
using System.Runtime.InteropServices;

namespace HPSocket.Tcp
{
    /// <summary>
    /// tcp pull client
    /// </summary>
    public class TcpPullClient : TcpClient, ITcpPullClient
    {
        public TcpPullClient()
            : base(
                Sdk.Tcp.Create_HP_TcpPullClientListener,
                Sdk.Tcp.Create_HP_TcpPullClient,
                Sdk.Tcp.Destroy_HP_TcpPullClient,
                Sdk.Tcp.Destroy_HP_TcpPullClientListener)
        {
        }

        protected TcpPullClient(Sdk.CreateListenerDelegate createListenerFunction, Sdk.CreateServiceDelegate createServiceFunction, Sdk.DestroyListenerDelegate destroyServiceFunction, Sdk.DestroyListenerDelegate destroyListenerFunction)
            : base(createListenerFunction, createServiceFunction, destroyServiceFunction, destroyListenerFunction)
        {
        }

        /// <inheritdoc />
        public new event PullClientReceiveEventHandler OnReceive;

        /// <inheritdoc />
        public FetchResult Fetch(IntPtr buffer, int length) => Sdk.Tcp.HP_TcpPullClient_Fetch(SenderPtr, buffer, length);

        /// <inheritdoc />
        public FetchResult Peek(IntPtr buffer, int length) => Sdk.Tcp.HP_TcpPullClient_Peek(SenderPtr, buffer, length);

        /// <inheritdoc />
        public FetchResult Fetch(int length, out byte[] bytes)
        {
            var buffer = IntPtr.Zero;
            try
            {
                bytes = null;
                buffer = Marshal.AllocHGlobal(length);
                var fr = Fetch(buffer, length);
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
        public FetchResult Peek(int length, out byte[] bytes)
        {
            var buffer = IntPtr.Zero;
            try
            {
                bytes = null;
                buffer = Marshal.AllocHGlobal(length);
                var fr = Peek(buffer, length);
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

            Sdk.Client.HP_Set_FN_Client_OnPullReceive(ListenerPtr, _onPullReceive);

            GC.KeepAlive(_onPullReceive);

            base.SetCallback();
        }

        protected HandleResult SdkOnReceive(IntPtr sender, IntPtr client, int length) => OnReceive?.Invoke(this, length) ?? HandleResult.Ignore;

        #endregion
    }
}
