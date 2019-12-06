using System;
using HPSocket.Http;

namespace HPSocket
{
    /// <summary>
    /// http easy agent
    /// </summary>
    public interface IHttpEasyServer : IHttpServer, IHttpEasyData
    {
        #region Easy事件

        /// <summary>
        /// 每次到一个完整的chunk data数据包, 事件到达同时表示chunk data接收完成
        /// </summary>
        event HttpServerEasyDataEventHandler OnEasyChunkData;
        /// <summary>
        /// 每次到一个完整的http get/post数据包 事件到达同时表示message接收完成
        /// </summary>
        event HttpServerEasyDataEventHandler OnEasyMessageData;
        /// <summary>
        /// 每次到一个完整的web socket数据包 事件到达同时表示web socket message接收完成
        /// </summary>
        event HttpServerWebSocketEasyDataEventHandler OnEasyWebSocketMessageData;


        [Obsolete("无需添加此事件, 接收到完整数据后一次性触发OnEasyChunkData事件", true)]
        new event ChunkHeaderEventHandler OnChunkHeader;
        [Obsolete("无需添加此事件, 接收到完整数据后一次性触发OnEasyChunkData事件", true)]
        new event ChunkCompleteEventHandler OnChunkComplete;
        [Obsolete("无需添加此事件, 接收到完整数据后一次性触发OnEasyMessageData事件", true)]
        new event HeadersCompleteEventHandler OnHeadersComplete;
        [Obsolete("无需添加此事件, 接收到完整数据后一次性触发OnEasyMessageData事件", true)]
        new event BodyEventHandler OnBody;
        [Obsolete("无需添加此事件, 接收到完整数据后一次性触发OnEasyMessageData事件", true)]
        new event MessageCompleteEventHandler OnMessageComplete;
        [Obsolete("无需添加此事件, 接收到完整数据后一次性触发OnEasyWebSocketMessageData事件", true)]
        new event WsMessageHeaderEventHandler OnWsMessageHeader;
        [Obsolete("无需添加此事件, 接收到完整数据后一次性触发OnEasyWebSocketMessageData事件", true)]
        new event WsMessageBodyEventHandler OnWsMessageBody;
        [Obsolete("无需添加此事件, 接收到完整数据后一次性触发OnEasyWebSocketMessageData事件", true)]
        new event WsMessageCompleteEventHandler OnWsMessageComplete;

        #endregion
    }
}
