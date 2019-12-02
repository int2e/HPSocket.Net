namespace HPSocket.Ssl
{
    public class SslPackClient : SslClient, ISslPackClient
    {
        public SslPackClient()
            : base(Sdk.Tcp.Create_HP_TcpPackClientListener,
                Sdk.Ssl.Create_HP_SSLPackClient,
                Sdk.Ssl.Destroy_HP_SSLPackClient,
                Sdk.Tcp.Destroy_HP_TcpPackClientListener)
        {
        }

        protected SslPackClient(Sdk.CreateListenerDelegate createListenerFunction, Sdk.CreateServiceDelegate createServiceFunction, Sdk.DestroyListenerDelegate destroyServiceFunction, Sdk.DestroyListenerDelegate destroyListenerFunction)
            : base(createListenerFunction, createServiceFunction, destroyServiceFunction, destroyListenerFunction)
        {
        }

        /// <inheritdoc />
        public uint MaxPackSize
        {
            get => Sdk.Tcp.HP_TcpPackClient_GetMaxPackSize(SenderPtr);
            set => Sdk.Tcp.HP_TcpPackClient_SetMaxPackSize(SenderPtr, value);
        }

        /// <inheritdoc />
        public ushort PackHeaderFlag
        {
            get => Sdk.Tcp.HP_TcpPackClient_GetPackHeaderFlag(SenderPtr);
            set => Sdk.Tcp.HP_TcpPackClient_SetPackHeaderFlag(SenderPtr, value);
        }
    }
}
