using HPSocket.Http;

namespace HPSocket
{
    /// <summary>
    /// http server/agent/client 的公共接口
    /// </summary>
    public interface IHttp : ISocket
    {
        #region Http属性

        /// <summary>
        /// 获取或设置 HTTP 启动方式, 默认为true
        /// </summary>
        bool HttpAutoStart { get; set; }

        /// <summary>
        /// 获取或设置本地协议版本
        /// </summary>
        HttpVersion LocalVersion { get; set; }

        #endregion

        #region Http事件

        /// <summary>
        /// 【可选】开始解析
        /// </summary>
        event MessageBeginEventHandler OnMessageBegin;
        /// <summary>
        /// 【可选】请求头通知
        /// </summary>
        event HeaderEventHandler OnHeader;
        /// <summary>
        /// 【可选】Chunked 报文头通知
        /// </summary>
        event ChunkHeaderEventHandler OnChunkHeader;
        /// <summary>
        /// 【可选】Chunked 报文结束通知
        /// </summary>
        event ChunkCompleteEventHandler OnChunkComplete;
        /// <summary>
        /// 【可选】升级协议通知
        /// </summary>
        event UpgradeEventHandler OnUpgrade;
        /// <summary>
        /// 【必须】请求头完成通知
        /// </summary>
        event HeadersCompleteEventHandler OnHeadersComplete;
        /// <summary>
        /// 【必须】请求体报文通知
        /// </summary>
        event BodyEventHandler OnBody;
        /// <summary>
        /// 【必须】完成解析通知
        /// </summary>
        event MessageCompleteEventHandler OnMessageComplete;
        /// <summary>
        /// 【必须】解析错误通知
        /// </summary>
        event ParseErrorEventHandler OnParseError;

        #endregion

        #region WS事件

        /// <summary>
        /// 【必须】WebSocket数据头通知
        /// </summary>
        event WsMessageHeaderEventHandler OnWsMessageHeader;
        /// <summary>
        /// 【必须】WebSocket数据包体通知(byte)
        /// </summary>
        event WsMessageBodyEventHandler OnWsMessageBody;
        /// <summary>
        /// 【必须】WebSocket数据完成解析通知
        /// </summary>
        event WsMessageCompleteEventHandler OnWsMessageComplete;

        #endregion

    }
}
