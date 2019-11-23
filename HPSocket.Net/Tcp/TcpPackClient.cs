namespace HPSocket.Tcp
{
    /// <summary>
    /// tcp pack client
    /// </summary>
    public class TcpPackClient : TcpClient, ITcpPackClient
    {
        public TcpPackClient()
            : base(
                Sdk.Tcp.Create_HP_TcpPackClientListener,
                Sdk.Tcp.Create_HP_TcpPackClient,
                Sdk.Tcp.Destroy_HP_TcpPackClient,
                Sdk.Tcp.Destroy_HP_TcpPackClientListener)
        {
        }

        protected TcpPackClient(Sdk.CreateListenerDelegate createListenerFunction, Sdk.CreateServiceDelegate createServiceFunction, Sdk.DestroyListenerDelegate destroyServiceFunction, Sdk.DestroyListenerDelegate destroyListenerFunction)
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
