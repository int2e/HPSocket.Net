namespace Socks5Server
{
    /// <summary>
    /// 连接信息
    /// </summary>
    public class ClientInfo
    {
        /// <summary>
        /// 协议状态
        /// </summary>
        public ProtocolState State { get; set; }

        /// <summary>
        /// 帐号
        /// </summary>
        public Account? Account { get; set; }

        /// <summary>
        /// server 连接id
        /// </summary>
        public nint ServerConnId { get; set; }

        /// <summary>
        /// agent 连接id
        /// </summary>
        public nint AgentConnId { get; set; }

        /// <summary>
        /// 附加数据
        /// </summary>
        public object? ExtraData { get; set; }

    }
}
