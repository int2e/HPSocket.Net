using System.Text;

namespace HPSocket.Udp
{
    public class UdpCast : Base.Client, IUdpCast
    {
        public UdpCast()
            : base(Sdk.Udp.Create_HP_UdpCastListener,
                Sdk.Udp.Create_HP_UdpCast,
                Sdk.Udp.Destroy_HP_UdpCast,
                Sdk.Udp.Destroy_HP_UdpCastListener)
        {
        }

        protected UdpCast(Sdk.CreateListenerDelegate createListenerFunction, Sdk.CreateServiceDelegate createServiceFunction, Sdk.DestroyListenerDelegate destroyServiceFunction, Sdk.DestroyListenerDelegate destroyListenerFunction)
            : base(createListenerFunction, createServiceFunction, destroyServiceFunction, destroyListenerFunction)
        {
        }

        /// <inheritdoc />
        public uint MaxDatagramSize
        {
            get => Sdk.Udp.HP_UdpCast_GetMaxDatagramSize(SenderPtr);
            set => Sdk.Udp.HP_UdpCast_SetMaxDatagramSize(SenderPtr, value);
        }
        
        /// <inheritdoc />
        public CastMode CastMode
        {
            get => Sdk.Udp.HP_UdpCast_GetCastMode(SenderPtr);
            set => Sdk.Udp.HP_UdpCast_SetCastMode(SenderPtr, value);
        }

        /// <inheritdoc />
        public int MultiCastTtl
        {
            get => Sdk.Udp.HP_UdpCast_GetMultiCastTtl(SenderPtr);
            set => Sdk.Udp.HP_UdpCast_SetMultiCastTtl(SenderPtr, value);
        }

        /// <inheritdoc />
        public bool IsMultiCastLoop
        {
            get => Sdk.Udp.HP_UdpCast_IsMultiCastLoop(SenderPtr);
            set => Sdk.Udp.HP_UdpCast_SetMultiCastLoop(SenderPtr, value);
        }

        /// <inheritdoc />
        public bool GetRemoteAddress(out string ip, out ushort port)
        {
            var ipLength = 60;
            var sb = new StringBuilder(ipLength);
            port = 0;
            var ok = Sdk.Udp.HP_UdpCast_GetRemoteAddress(SenderPtr, sb, ref ipLength, ref port) && ipLength > 0;
            ip = ok ? sb.ToString() : string.Empty;
            return ok;
        }
    }
}
