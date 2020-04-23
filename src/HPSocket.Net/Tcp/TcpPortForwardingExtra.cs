using System;

namespace HPSocket.Tcp
{
    /// <summary>
    /// tcp 端口转发附加数据
    /// </summary>
    internal class TcpPortForwardingExtra
    {
        /// <summary>
        /// server 对应的 conn id
        /// </summary>
        public IntPtr ServerConnId { get; set; }

        /// <summary>
        /// agent 对应的 conn id
        /// </summary>
        public IntPtr AgentConnId { get; set; }

        /// <summary>
        /// server 对象
        /// </summary>
        public IServer Server { get; set; }

        /// <summary>
        /// agent 对象
        /// </summary>
        public IAgent Agent { get; set; }

        /// <summary>
        /// 释放方式
        /// </summary>
        public TcpPortForwardingReleaseType ReleaseType { get; set; }

        /// <summary>
        /// 附加数据
        /// </summary>
        public object ExtraData { get; set; }
    }
}
