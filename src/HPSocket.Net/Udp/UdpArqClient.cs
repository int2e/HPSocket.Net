namespace HPSocket.Udp
{
    public class UdpArqClient : UdpClient, IUdpArqClient
    {
        public UdpArqClient()
            : base(Sdk.Udp.Create_HP_UdpArqClientListener,
                Sdk.Udp.Create_HP_UdpArqClient,
                Sdk.Udp.Destroy_HP_UdpArqClient,
                Sdk.Udp.Destroy_HP_UdpArqClientListener)
        {
        }

        protected UdpArqClient(Sdk.CreateListenerDelegate createListenerFunction, Sdk.CreateServiceDelegate createServiceFunction, Sdk.DestroyListenerDelegate destroyServiceFunction, Sdk.DestroyListenerDelegate destroyListenerFunction)
            : base(createListenerFunction, createServiceFunction, destroyServiceFunction, destroyListenerFunction)
        {
        }

        /// <inheritdoc />
        public bool IsNoDelay
        {
            get => Sdk.Udp.HP_UdpArqClient_IsNoDelay(SenderPtr);
            set => Sdk.Udp.HP_UdpArqClient_SetNoDelay(SenderPtr, value);
        }

        /// <inheritdoc />
        public bool IsTurnoffCongestCtrl
        {
            get => Sdk.Udp.HP_UdpArqClient_IsTurnoffCongestCtrl(SenderPtr);
            set => Sdk.Udp.HP_UdpArqClient_SetTurnoffCongestCtrl(SenderPtr, value);
        }

        /// <inheritdoc />
        public uint FlushInterval
        {
            get => Sdk.Udp.HP_UdpArqClient_GetFlushInterval(SenderPtr);
            set => Sdk.Udp.HP_UdpArqClient_SetFlushInterval(SenderPtr, value);
        }

        /// <inheritdoc />
        public uint ResendByAcks
        {
            get => Sdk.Udp.HP_UdpArqClient_GetResendByAcks(SenderPtr);
            set => Sdk.Udp.HP_UdpArqClient_SetResendByAcks(SenderPtr, value);
        }

        /// <inheritdoc />
        public uint SendWndSize
        {
            get => Sdk.Udp.HP_UdpArqClient_GetSendWndSize(SenderPtr);
            set => Sdk.Udp.HP_UdpArqClient_SetSendWndSize(SenderPtr, value);
        }

        /// <inheritdoc />
        public uint RecvWndSize
        {
            get => Sdk.Udp.HP_UdpArqClient_GetRecvWndSize(SenderPtr);
            set => Sdk.Udp.HP_UdpArqClient_SetRecvWndSize(SenderPtr, value);
        }

        /// <inheritdoc />
        public uint MinRto
        {
            get => Sdk.Udp.HP_UdpArqClient_GetMinRto(SenderPtr);
            set => Sdk.Udp.HP_UdpArqClient_SetMinRto(SenderPtr, value);
        }

        /// <inheritdoc />
        public uint MaxTransUnit
        {
            get => Sdk.Udp.HP_UdpArqClient_GetMaxTransUnit(SenderPtr);
            set => Sdk.Udp.HP_UdpArqClient_SetMaxTransUnit(SenderPtr, value);
        }

        /// <inheritdoc />
        public uint MaxMessageSize
        {
            get => Sdk.Udp.HP_UdpArqClient_GetMaxMessageSize(SenderPtr);
            set => Sdk.Udp.HP_UdpArqClient_SetMaxMessageSize(SenderPtr, value);
        }

        /// <inheritdoc />
        public uint HandShakeTimeout
        {
            get => Sdk.Udp.HP_UdpArqClient_GetHandShakeTimeout(SenderPtr);
            set => Sdk.Udp.HP_UdpArqClient_SetHandShakeTimeout(SenderPtr, value);
        }

        /// <inheritdoc />
        public uint FastLimit
        {
            get => Sdk.Udp.HP_UdpArqClient_GetFastLimit(SenderPtr);
            set => Sdk.Udp.HP_UdpArqClient_SetFastLimit(SenderPtr, value);
        }

        /// <inheritdoc />
        public bool GetWaitingSendMessageCount(out int count)
        {
            count = 0;
            return Sdk.Udp.HP_UdpArqClient_GetWaitingSendMessageCount(SenderPtr, ref count);
        }
    }
}
