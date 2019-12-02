namespace HPSocket.Udp
{
    public class UdpClient : Base.Client, IUdpClient
    {
        public UdpClient()
            : base(Sdk.Udp.Create_HP_UdpClientListener,
                Sdk.Udp.Create_HP_UdpClient,
                Sdk.Udp.Destroy_HP_UdpClient,
                Sdk.Udp.Destroy_HP_UdpClientListener)
        {
        }

        protected UdpClient(Sdk.CreateListenerDelegate createListenerFunction, Sdk.CreateServiceDelegate createServiceFunction, Sdk.DestroyListenerDelegate destroyServiceFunction, Sdk.DestroyListenerDelegate destroyListenerFunction) 
            : base(createListenerFunction, createServiceFunction, destroyServiceFunction, destroyListenerFunction)
        {
        }

        /// <inheritdoc />
        public uint MaxDatagramSize
        {
            get => Sdk.Udp.HP_UdpClient_GetMaxDatagramSize(SenderPtr);
            set => Sdk.Udp.HP_UdpClient_SetMaxDatagramSize(SenderPtr, value);
        }
        
        /// <inheritdoc />
        public uint DetectAttempts
        {
            get => Sdk.Udp.HP_UdpClient_GetDetectAttempts(SenderPtr);
            set => Sdk.Udp.HP_UdpClient_SetDetectAttempts(SenderPtr, value);
        }

        /// <inheritdoc />
        public uint DetectInterval
        {
            get => Sdk.Udp.HP_UdpClient_GetDetectInterval(SenderPtr);
            set => Sdk.Udp.HP_UdpClient_SetDetectInterval(SenderPtr, value);
        }
    }
}
