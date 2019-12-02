namespace HPSocket.Proxy
{
    /// <summary>
    /// 代理连接状态
    /// </summary>
    internal enum ProxyConnectionState
    {
        /// <summary>
        /// 正常收发
        /// </summary>
        Normal,
        /// <summary>
        /// socks5 获取受支持的认证方法; http 发送connect方法
        /// </summary>
        Step1,
        /// <summary>
        /// socks5 获取受支持的认证方法; http 判断连接是否成功
        /// </summary>
        Step2,
        /// <summary>
        /// socks5 连接目标服务器; http 未使用
        /// </summary>
        Step3,
    }
}
