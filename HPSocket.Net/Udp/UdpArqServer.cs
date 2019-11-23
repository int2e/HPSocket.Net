using System;

namespace HPSocket.Udp
{
    public class UdpArqServer : UdpServer, IUdpArqServer
    {
        public UdpArqServer()
            : base(Sdk.Udp.Create_HP_UdpArqServerListener,
                Sdk.Udp.Create_HP_UdpArqServer,
                Sdk.Udp.Destroy_HP_UdpArqServer,
                Sdk.Udp.Destroy_HP_UdpArqServerListener)
        {
        }

        protected UdpArqServer(Sdk.CreateListenerDelegate createListenerFunction, Sdk.CreateServiceDelegate createServiceFunction, Sdk.DestroyListenerDelegate destroyServiceFunction, Sdk.DestroyListenerDelegate destroyListenerFunction)
            : base(createListenerFunction, createServiceFunction, destroyServiceFunction, destroyListenerFunction)
        {
        }

        /// <inheritdoc />
        public bool IsNoDelay
        {
            get => Sdk.Udp.HP_UdpArqServer_IsNoDelay(SenderPtr);
            set => Sdk.Udp.HP_UdpArqServer_SetNoDelay(SenderPtr, value);
        }

        /// <inheritdoc />
        public bool IsTurnoffCongestCtrl
        {
            get => Sdk.Udp.HP_UdpArqServer_IsTurnoffCongestCtrl(SenderPtr);
            set => Sdk.Udp.HP_UdpArqServer_SetTurnoffCongestCtrl(SenderPtr, value);
        }

        /// <inheritdoc />
        public uint FlushInterval
        {
            get => Sdk.Udp.HP_UdpArqServer_GetFlushInterval(SenderPtr);
            set => Sdk.Udp.HP_UdpArqServer_SetFlushInterval(SenderPtr, value);
        }

        /// <inheritdoc />
        public uint ResendByAcks
        {
            get => Sdk.Udp.HP_UdpArqServer_GetResendByAcks(SenderPtr);
            set => Sdk.Udp.HP_UdpArqServer_SetResendByAcks(SenderPtr, value);
        }

        /// <inheritdoc />
        public uint SendWndSize
        {
            get => Sdk.Udp.HP_UdpArqServer_GetSendWndSize(SenderPtr);
            set => Sdk.Udp.HP_UdpArqServer_SetSendWndSize(SenderPtr, value);
        }

        /// <inheritdoc />
        public uint RecvWndSize
        {
            get => Sdk.Udp.HP_UdpArqServer_GetRecvWndSize(SenderPtr);
            set => Sdk.Udp.HP_UdpArqServer_SetRecvWndSize(SenderPtr, value);
        }

        /// <inheritdoc />
        public uint MinRto
        {
            get => Sdk.Udp.HP_UdpArqServer_GetMinRto(SenderPtr);
            set => Sdk.Udp.HP_UdpArqServer_SetMinRto(SenderPtr, value);
        }

        /// <inheritdoc />
        public uint MaxTransUnit
        {
            get => Sdk.Udp.HP_UdpArqServer_GetMaxTransUnit(SenderPtr);
            set => Sdk.Udp.HP_UdpArqServer_SetMaxTransUnit(SenderPtr, value);
        }

        /// <inheritdoc />
        public uint MaxMessageSize
        {
            get => Sdk.Udp.HP_UdpArqServer_GetMaxMessageSize(SenderPtr);
            set => Sdk.Udp.HP_UdpArqServer_SetMaxMessageSize(SenderPtr, value);
        }

        /// <inheritdoc />
        public uint HandShakeTimeout
        {
            get => Sdk.Udp.HP_UdpArqServer_GetHandShakeTimeout(SenderPtr);
            set => Sdk.Udp.HP_UdpArqServer_SetHandShakeTimeout(SenderPtr, value);
        }

        /// <inheritdoc />
        public uint FastLimit
        {
            get => Sdk.Udp.HP_UdpArqServer_GetFastLimit(SenderPtr);
            set => Sdk.Udp.HP_UdpArqServer_SetFastLimit(SenderPtr, value);
        }

        /// <inheritdoc />
        public bool GetWaitingSendMessageCount(IntPtr connId, out int count)
        {
            count = 0;
            return Sdk.Udp.HP_UdpArqServer_GetWaitingSendMessageCount(SenderPtr, connId, ref count);
        }
    }
}
