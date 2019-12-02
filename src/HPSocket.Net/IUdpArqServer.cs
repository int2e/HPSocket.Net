using System;

namespace HPSocket
{
    /// <summary>
    /// udp arq server
    /// </summary>
    public interface IUdpArqServer : IUdpServer
    {
        #region 服务器属性

        /// <summary>
        /// 获取或设置是否开启 NoDelay 模式 
        /// </summary>
        bool IsNoDelay { get; set; }

        /// <summary>
        /// 获取或设置是否关闭拥塞控制（默认：false，不关闭）
        /// </summary>
        bool IsTurnoffCongestCtrl { get; set; }

        /// <summary>
        /// 获取或设置数据刷新间隔（毫秒，默认：20）
        /// </summary>
        uint FlushInterval { get; set; }

        /// <summary>
        /// 获取或设置快速重传 ACK 跨越次数（默认：0，关闭快速重传）
        /// </summary>
        uint ResendByAcks { get; set; }

        /// <summary>
        /// 获取或设置发送窗口大小（数据包数量，默认：128）
        /// </summary>
        uint SendWndSize { get; set; }

        /// <summary>
        /// 获取或设置接收窗口大小（数据包数量，默认：512）
        /// </summary>
        uint RecvWndSize { get; set; }

        /// <summary>
        /// 获取或设置最小重传超时时间（毫秒，默认：30）
        /// </summary>
        uint MinRto { get; set; }

        /// <summary>
        /// 获取或设置最大传输单元（默认：0，与 MaxDataGramSize 一致）
        /// </summary>
        uint MaxTransUnit { get; set; }

        /// <summary>
        /// 获取或设置最大数据包大小（默认：4096）
        /// </summary>
        uint MaxMessageSize { get; set; }

        /// <summary>
        /// 获取或设置握手超时时间（毫秒，默认：5000）
        /// </summary>
        uint HandShakeTimeout { get; set; }

        /// <summary>
        /// 获取或设置快速握手次数限制（默认：5，如果为 0 则不限制
        /// </summary>
        uint FastLimit { get; set; }

        #endregion

        #region 服务器方法

        /// <summary>
        /// 获取等待发送包数量
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        bool GetWaitingSendMessageCount(IntPtr connId, out int count);

        #endregion
    }
}
