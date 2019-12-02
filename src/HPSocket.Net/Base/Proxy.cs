namespace HPSocket.Base
{
    /// <summary>
    /// proxy base
    /// </summary>
    public abstract class Proxy : IProxy
    {
        /// <summary>
        /// 主机地址
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// 主机端口
        /// </summary>
        public ushort Port { get; set; }
        /// <summary>
        /// 帐号
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 远程服务器地址
        /// </summary>
        protected string RemoteAddress { get; set; }
        /// <summary>
        /// 远程服务器端口
        /// </summary>
        protected ushort RemotePort { get; set; }

        /// <summary>
        /// User-Agent, 只对 http 代理有效
        /// <para>默认HPSocket.net/2.0</para>
        /// </summary>
        public string UserAgent { get; set; } = "HPSocket.net/2.0";

    }
}
