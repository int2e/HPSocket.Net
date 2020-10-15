using System;
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

        /// <summary>
        /// 获取是否启动
        /// </summary>
        bool HasStarted { get; }

        #endregion

        #region 服务器方法

        /// <summary>
        /// 获取指定连接的子协议
        /// </summary>
        /// <param name="connId"></param>
        /// <returns></returns>
        string GetSubProtocol(IntPtr connId);

        /// <summary>
        /// 获取连接的http会话信息
        /// </summary>
        /// <param name="connId"></param>
        /// <returns></returns>
        HttpSession GetHttpSession(IntPtr connId);

        /// <summary>
        /// 对path注册特定服务
        /// <para>例如: AddHub&lt;Chat&gt;("/chat")</para>
        /// </summary>
        /// <typeparam name="THub">继承自THub的类</typeparam>
        /// <param name="path">url path</param>
        void AddHub<THub>(string path) where THub : IHub, new();

        /// <summary>
        /// 对path注册特定服务且绑定到已有对象实例
        /// <para>例如: AddHub&lt;Chat&gt;("/chat", form1)</para>
        /// </summary>
        /// <typeparam name="THub">继承自THub的类</typeparam>
        /// <param name="path">url path</param>
        /// <param name="obj">已THub类型的对象实例，例如：form1</param>
        void AddHub<THub>(string path, THub obj) where THub : IHub;

        /// <summary>
        /// 获取指定类型的已注册过的服务
        /// </summary>
        /// <typeparam name="THub">继承自THub的类</typeparam>
        /// <param name="path"></param>
        /// <returns>path有效返回对象，path无效返回T的默认类型</returns>
        THub GetHub<THub>(string path) where THub : IHub;

        /// <summary>
        /// 移除已注册的服务
        /// </summary>
        /// <param name="path"></param>
        void RemoveHub(string path);

        #endregion
    }
}
