using System;
using System.Collections.Generic;

using HPSocket.WebSocket;

namespace HPSocket
{
    /// <summary>
    /// websocket agent
    /// </summary>
    public interface IWebSocketAgent : IWebSocket
    {
        #region 客户端属性

        /// <summary>
        /// 本地绑定地址, 默认0.0.0.0
        /// </summary>
        string BindAddress { get; set; }

        /// <summary>
        /// 默认掩码, 默认值: byte[] { 0x01, 0x02, 0x03, 0x04 }
        /// </summary>
        byte[] DefaultMask { get; set; }

        /// <summary>
        /// 浏览器 User-Agent, 默认 chrome78.0.3904.97 的 User-Agent
        /// </summary>
        string UserAgent { get; set; }

        /// <summary>
        /// cookie
        /// </summary>
        string Cookie { get; set; }
        
        /// <summary>
        /// 附加请求头, 除去UserAgent和Cookie头之外, 还想附加别的http请求头, 在此设置, 将在第一次访问, 升级协议的时候附加这些请求头
        /// </summary>
        List<NameValue> RequestHeaders { get; set; }

        /// <summary>
        /// 连接超时时间
        /// </summary>
        int ConnectionTimeout { get; set; }

        /// <summary>
        /// 获取是否启动
        /// </summary>
        bool HasStarted { get; }
        #endregion

        #region 客户端事件

        /// <summary>
        /// cont/text/binary 消息
        /// </summary>
        event MessageEventHandler OnMessage;

        /// <summary>
        /// 握手成功, 打开/进入 连接
        /// </summary>
        event OpenEventHandler OnOpen;

        /// <summary>
        /// 连接关闭
        /// </summary>
        event CloseEventHandler OnClose;

        /// <summary>
        /// ping消息
        /// </summary>
        event PingEventHandler OnPing;

        /// <summary>
        /// pong消息
        /// </summary>
        event PongEventHandler OnPong;

        #endregion

        #region 客户端方法

        /// <summary>
        /// 连接到目标 web socket 服务器
        /// </summary>
        /// <returns></returns>
        void Connect();

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="final"></param>
        /// <param name="opCode"></param>
        /// <param name="data"></param>
        /// <param name="mask"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        bool Send(IntPtr connId, bool final, OpCode opCode, byte[] mask, byte[] data, int length);

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="final"></param>
        /// <param name="opCode"></param>
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        new bool Send(IntPtr connId, bool final, OpCode opCode, byte[] data, int length);

        #endregion
    }
}
