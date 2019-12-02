using HPSocket.Udp;

namespace HPSocket
{
    /// <summary>
    /// udp cast
    /// </summary>
    public interface IUdpCast : IClient
    {
        #region 客户端属性

        /// <summary>
        /// 获取或设置数据报文最大长度
        /// <para>建议在局域网环境下不超过 1472 字节，在广域网环境下不超过 548 字节</para>
        /// </summary>
        uint MaxDatagramSize { get; set; }

        /// <summary>
        /// 获取或设置传播模式（组播或广播）
        /// </summary>
        CastMode CastMode { get; set; }

        /// <summary>
        /// 获取或设置组播报文的 TTL（0 - 255）
        /// </summary>
        int MultiCastTtl { get; set; }

        /// <summary>
        /// 获取或设置是否启用组播环路
        /// </summary>
        bool IsMultiCastLoop { get; set; }

        #endregion

        #region 客户端方法

        /// <summary>
        /// 获取当前数据报的远程地址信息（通常在 OnReceive 事件中调用）
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        bool GetRemoteAddress(out string ip, out ushort port);

        #endregion
    }
}
