namespace HPSocket.Tcp
{
    /// <summary>
    /// tcp pack server
    /// </summary>
    public class TcpPackServer : TcpServer, ITcpPackServer
    {
        public TcpPackServer()
            : base(Sdk.Tcp.Create_HP_TcpPackServerListener,
                Sdk.Tcp.Create_HP_TcpPackServer,
                Sdk.Tcp.Destroy_HP_TcpPackServer,
                Sdk.Tcp.Destroy_HP_TcpPackServerListener)
        {
        }

        protected TcpPackServer(Sdk.CreateListenerDelegate createListenerFunction, Sdk.CreateServiceDelegate createServiceFunction, Sdk.DestroyListenerDelegate destroyServiceFunction, Sdk.DestroyListenerDelegate destroyListenerFunction)
            : base(createListenerFunction, createServiceFunction, destroyServiceFunction, destroyListenerFunction)
        {
        }

        /// <inheritdoc />
        public uint MaxPackSize
        {
            get => Sdk.Tcp.HP_TcpPackServer_GetMaxPackSize( SenderPtr);
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
