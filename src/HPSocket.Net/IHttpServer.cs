using System;
using System.Collections.Generic;
using HPSocket.Http;
using HPSocket.WebSocket;

namespace HPSocket
{
    /// <summary>
    /// http server
    /// </summary>
    public interface IHttpServer : ITcpServer, IHttpMultiId
    {
        #region Http服务器属性

        /// <summary>
        /// 获取或设置连接释放延时（默认：3000 毫秒）
        /// </summary>
        uint ReleaseDelay { get; set; }

        #endregion

        #region Http服务器事件

        /// <summary>
        /// 【可选】请求行解析完成（仅用于 HTTP 服务端）
        /// </summary>
        event RequestLineEventHandler OnRequestLine;

        #endregion

        #region Http服务器方法

        /// <summary>
        /// 发送请求
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="statusCode">http状态码</param>
        /// <param name="headers">响应头</param>
        /// <param name="body">响应体</param>
        /// <param name="length">响应体体长度</param>
        /// <returns></returns>
        bool SendResponse(IntPtr connId, HttpStatusCode statusCode, List<NameValue> headers, byte[] body, int length);

        /// <summary>
        /// 发送请求
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="statusCode">http状态码</param>
        /// <param name="desc">http响应描述 (一般根据状态码来)</param>
        /// <param name="headers">响应头</param>
        /// <param name="body">响应体</param>
        /// <param name="length">响应体体长度</param>
        /// <returns></returns>
        bool SendResponse(IntPtr connId, HttpStatusCode statusCode, string desc, List<NameValue> headers, byte[] body, int length);

        /// <summary>
        /// 发送本地小文件
        /// <para>向指定连接发送 4096 KB 以下的小文件</para>
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="statusCode"></param>
        /// <param name="filePath"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        bool SendSmallFile(IntPtr connId, HttpStatusCode statusCode, List<NameValue> headers, string filePath);

        /// <summary>
        /// 发送web socket消息
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="final"></param>
        /// <param name="rsv"></param>
        /// <param name="opCode"></param>
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        bool SendWsMessage(IntPtr connId, bool final, Rsv rsv, OpCode opCode, byte[] data, int length);

        /// <summary>
        /// 获取主机
        /// </summary>
        /// <param name="connId"></param>
        /// <returns></returns>
        string GetHost(IntPtr connId);

        /// <summary>
        /// 名称：释放连接
        /// 描述：把连接放入释放队列，等待某个时间（通过 SetReleaseDelay() 设置）关闭连接
        /// </summary>
        /// <param name="connId">连接 ID</param>
        /// <returns></returns>
        bool Release(IntPtr connId);

        /// <summary>
        /// 获取某个 URL 域值
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="urlField"></param>
        /// <returns></returns>
        string GetUrlField(IntPtr connId, HttpUrlField urlField);

        /// <summary>
        /// 获取请求行 URL 域掩码（URL 域参考：HttpUrlField）
        /// </summary>
        /// <param name="connId"></param>
        /// <returns></returns>
        ushort GetUrlFieldSet(IntPtr connId);

        /// <summary>
        /// 获取请求方法
        /// </summary>
        /// <param name="connId"></param>
        /// <returns></returns>
        string GetMethod(IntPtr connId);

        #endregion
    }
}
