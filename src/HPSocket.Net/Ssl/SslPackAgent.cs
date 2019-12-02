namespace HPSocket.Ssl
{
    public class SslPackAgent : SslAgent, ISslPackAgent
    {
        public SslPackAgent()
            : base(Sdk.Tcp.Create_HP_TcpPackAgentListener,
                Sdk.Ssl.Create_HP_SSLPackAgent,
                Sdk.Ssl.Destroy_HP_SSLPackAgent,
                Sdk.Tcp.Destroy_HP_TcpPackAgentListener)
        {
        }

        protected SslPackAgent(Sdk.CreateListenerDelegate createListenerFunction, Sdk.CreateServiceDelegate createServiceFunction, Sdk.DestroyListenerDelegate destroyServiceFunction, Sdk.DestroyListenerDelegate destroyListenerFunction)
            : base(createListenerFunction, createServiceFunction, destroyServiceFunction, destroyListenerFunction)
        {
        }


        /// <inheritdoc />
        public uint MaxPackSize
        {
            get => Sdk.Tcp.HP_TcpPackAgent_GetMaxPackSize(SenderPtr);
            set => Sdk.Tcp.HP_TcpPackAgent_SetMaxPackSize(SenderPtr, value);
        }

        /// <inheritdoc />
        public ushort PackHeaderFlag
        {
            get => Sdk.Tcp.HP_TcpPackAgent_GetPackHeaderFlag(SenderPtr);
            set => Sdk.Tcp.HP_TcpPackAgent_SetPackHeaderFlag(SenderPtr, value);
        }
    }
}
