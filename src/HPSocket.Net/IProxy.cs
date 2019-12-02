namespace HPSocket
{
    /// <summary>
    /// proxy
    /// </summary>
    public interface IProxy
    {
        /// <summary>
        /// 主机地址
        /// </summary>
        string Host { get; set; }
        /// <summary>
        /// 主机端口
        /// </summary>
        ushort Port { get; set; }
        /// <summary>
        /// 帐号
        /// </summary>
        string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        string Password { get; set; }

    }
}
