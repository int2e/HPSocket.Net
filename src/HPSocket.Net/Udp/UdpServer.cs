namespace HPSocket.Udp
{
    public class UdpServer : Base.Server, IUdpServer
    {
        public UdpServer()
            : base(Sdk.Udp.Create_HP_UdpServerListener,
                Sdk.Udp.Create_HP_UdpServer,
                Sdk.Udp.Destroy_HP_UdpServer,
                Sdk.Udp.Destroy_HP_UdpServerListener)
        {
        }

        protected UdpServer(Sdk.CreateListenerDelegate createListenerFunction, Sdk.CreateServiceDelegate createServiceFunction, Sdk.DestroyListenerDelegate destroyServiceFunction, Sdk.DestroyListenerDelegate destroyListenerFunction) 
            : base(createListenerFunction, createServiceFunction, destroyServiceFunction, destroyListenerFunction)
        {
        }

        /// <inheritdoc />
        public uint MaxDatagramSize
        {
            get => Sdk.Udp.HP_UdpServer_GetMaxDatagramSize(SenderPtr);
            set => Sdk.Udp.HP_UdpServer_SetMaxDatagramSize(SenderPtr, value);
        }

        /// <inheritdoc />
        public uint PostReceiveCount
        {
            get => Sdk.Udp.HP_UdpServer_GetPostReceiveCount(SenderPtr);
            set => Sdk.Udp.HP_UdpServer_SetPostReceiveCount(SenderPtr, value);
        }

        /// <inheritdoc />
        public uint DetectAttempts
        {
            get => Sdk.Udp.HP_UdpServer_GetDetectAttempts(SenderPtr);
            set => Sdk.Udp.HP_UdpServer_SetDetectAttempts(SenderPtr, value);
        }

        /// <inheritdoc />
        public uint DetectInterval
        {
            get => Sdk.Udp.HP_UdpServer_GetDetectInterval(SenderPtr);
            set => Sdk.Udp.HP_UdpServer_SetDetectInterval(SenderPtr, value);
        }
    }
}
