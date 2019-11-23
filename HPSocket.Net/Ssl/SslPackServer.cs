namespace HPSocket.Ssl
{
    public class SslPackServer : SslServer, ISslPackServer
    {
        public SslPackServer()
            : base(Sdk.Tcp.Create_HP_TcpPackServerListener,
                Sdk.Ssl.Create_HP_SSLPackServer,
                Sdk.Ssl.Destroy_HP_SSLPackServer,
                Sdk.Tcp.Destroy_HP_TcpPackServerListener)
        {
        }

        protected SslPackServer(Sdk.CreateListenerDelegate createListenerFunction, Sdk.CreateServiceDelegate createServiceFunction, Sdk.DestroyListenerDelegate destroyServiceFunction, Sdk.DestroyListenerDelegate destroyListenerFunction)
            : base(createListenerFunction, createServiceFunction, destroyServiceFunction, destroyListenerFunction)
        {
        }

        /// <inheritdoc />
        public uint MaxPackSize
        {
            get => Sdk.Tcp.HP_TcpPackServer_GetMaxPackSize(SenderPtr);
            set => Sdk.Tcp.HP_TcpPackServer_SetMaxPackSize(SenderPtr, value);
        }

        /// <inheritdoc />
        public ushort PackHeaderFlag
        {
            get => Sdk.Tcp.HP_TcpPackServer_GetPackHeaderFlag(SenderPtr);
            set => Sdk.Tcp.HP_TcpPackServer_SetPackHeaderFlag(SenderPtr, value);
        }
    }
}
