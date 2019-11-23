using HPSocket.WebSocket;

namespace HPSocket
{
    /// <summary>
    /// web socket server
    /// </summary>
    public interface IWebSocketServer : IWebSocket
    {
        #region 服务器属性
        
        /// <summary>
        /// 自动发送ping消息的时间间隔
        /// <para>毫秒，0不自动发送，默认不发送（多数分机房的防火墙都在1分钟检测空连接，超时无交互则被踢，如果间隔过长，可能被机房防火墙误杀）</para>
        /// <para>目前浏览器都不支持在客户端发送ping消息，所以一般在服务器发送ping，在客户端响应接收到ping消息之后再对服务器发送pong，或客户端主动pong，服务器响应pong再发送ping给客户端</para>
        /// </summary>
        uint PingInterval { get; set; }

        #endregion

        #region 服务器方法

        /// <summary>
        /// 对path注册特定服务
        /// <para>例如: AddHub&lt;Chat&gt;("/chat")</para>
        /// </summary>
        /// <typeparam name="TBehaviorWithNew">继承自WebSocketBehavior的行为类</typeparam>
        /// <param name="path">url path</param>
        void AddHub<TBehaviorWithNew>(string path) where TBehaviorWithNew : IHub, new();

        /// <summary>
        /// 移除已注册的服务
        /// </summary>
        /// <param name="path"></param>
        void RemoveHub(string path);

        #endregion
    }
}
