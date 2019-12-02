namespace HPSocket.Tcp
{
    /// <summary>
    /// 数据抓取结果,数据抓取操作的返回值
    /// </summary>
    public enum FetchResult
    {
        /// <summary>
        /// 成功
        /// </summary>
        Ok = 0,
        /// <summary>
        /// 抓取长度过大
        /// </summary>
        LengthTooLong = 1,
        /// <summary>
        /// 找不到 ConnID 对应的数据
        /// </summary>
        DataNotFound = 2,
    }

    /// <summary>
    /// 连接状态
    /// </summary>
    public enum TcpConnectionState
    {
        /// <summary>
        /// 连接中
        /// </summary>
        Connecting,
        /// <summary>
        /// 已连接
        /// </summary>
        Connected,
        /// <summary>
        /// 连接超时
        /// </summary>
        TimedOut,
        /// <summary>
        /// 已关闭
        /// </summary>
        Closed,
    }


    /// <summary>
    /// 释放方式
    /// </summary>
    internal enum TcpPortForwardingReleaseType
    {
        None,
        Server,
        Agent,
    }

}
