using System;
using System.Collections.Generic;

namespace HPSocket.Tcp
{
    /// <summary>
    /// tcp pack agent
    /// </summary>
    public class TcpPackAgent : TcpAgent, ITcpPackAgent
    {
        public TcpPackAgent()
            : base(Sdk.Tcp.Create_HP_TcpPackAgentListener,
                Sdk.Tcp.Create_HP_TcpPackAgent,
                Sdk.Tcp.Destroy_HP_TcpPackAgent,
                Sdk.Tcp.Destroy_HP_TcpPackAgentListener)
        {
        }

        protected TcpPackAgent(Sdk.CreateListenerDelegate createListenerFunction, Sdk.CreateServiceDelegate createServiceFunction, Sdk.DestroyListenerDelegate destroyServiceFunction, Sdk.DestroyListenerDelegate destroyListenerFunction)
            : base(createListenerFunction, createServiceFunction, destroyServiceFunction, destroyListenerFunction)
        {
        }

        [Obsolete("tcp pack agent 不支持设置代理", true)]
        public new List<IProxy> ProxyList
        {
            get => null;
            set
            {

            }
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
