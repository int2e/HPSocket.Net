namespace HPSocket
{
    /// <summary>
    /// udp client
    /// </summary>
    public interface IUdpClient : IClient
    {
        #region 客户端属性

        /// <summary>
        /// 获取或设置数据报文最大长度
        /// <para>建议在局域网环境下不超过 1472 字节，在广域网环境下不超过 548 字节</para>
        /// </summary>
        uint MaxDatagramSize { get; set; }
        
        /// <summary>
        /// 获取或设置监测包尝试次数
        /// <para>0 则不发送监测跳包，如果超过最大尝试次数则认为已断线</para>
        /// </summary>
        uint DetectAttempts { get; set; }

        /// <summary>
        /// 获取或设置心跳检查间隔
        /// </summary>
        uint DetectInterval { get; set; }

        #endregion
    }
}
