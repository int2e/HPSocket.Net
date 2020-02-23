using System;
using System.Collections.Generic;
using HPSocket.WebSocket;

namespace HPSocket
{
    /// <summary>
    /// web socket server/agent 公共接口
    /// </summary>
    public interface IWebSocket : ISocket
    {
        #region 公共属性

        /// <summary>
        /// 忽略压缩扩展, 默认false
        /// <para>如果忽略, 则不支持压缩解压缩</para>
        /// </summary>
        bool IgnoreCompressionExtensions { get; set; }

        /// <summary>
        /// 开放式http server/agent对象, 对 http 连接有 cookie 或者 header 操作, 直接调用这个对象操作
        /// </summary>
        IHttpMultiId Http { get; }

        /// <summary>
        /// Uri
        /// </summary>
        Uri Uri { get; }

        /// <summary>
        /// 是否安全连接
        /// </summary>
        bool IsSecure { get; }

        /// <summary>
        /// 最大封包长度, 默认0, 不限制
        /// </summary>
        uint MaxPacketSize { get; set; }

        /// <summary>
        /// ssl环境配置
        /// </summary>
        SslConfiguration SslConfiguration { get; set; }

        /// <summary>
        /// 支持的子协议, 默认空, 不限制
        /// </summary>
        string SubProtocols { get; set; }

        #endregion

        #region 公共方法

        /// <summary>
        /// 启动服务
        /// </summary>
        bool Start();

        /// <summary>
        /// 停止服务
        /// </summary>
        bool Stop();

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="final"></param>
        /// <param name="opCode"></param>
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        bool Send(IntPtr connId, bool final, OpCode opCode, byte[] data, int length);

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="opCode"></param>
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        bool Send(IntPtr connId, OpCode opCode, byte[] data, int length);

        /// <summary>
        /// 发送文本消息
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        bool Text(IntPtr connId, string text);

        /// <summary>
        /// 发送ping消息
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="data"></param>
        /// <param name="length"></param>
        void Ping(IntPtr connId, byte[] data, int length);

        /// <summary>
        /// 发送pong消息
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="data"></param>
        /// <param name="length"></param>
        void Pong(IntPtr connId, byte[] data, int length);

        /// <summary>
        /// 发送关闭消息同时关闭连接
        /// </summary>
        /// <param name="connId"></param>
        void Close(IntPtr connId);

        /// <summary>
        /// 获取所有连接
        /// </summary>
        /// <returns></returns>
        List<IntPtr> GetAllConnectionIds();

        #endregion
    }
}
