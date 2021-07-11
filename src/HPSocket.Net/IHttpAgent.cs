using System;
using System.Collections.Generic;
using HPSocket.Http;
using HPSocket.WebSocket;

namespace HPSocket
{
    /// <summary>
    /// http agent
    /// </summary>
    public interface IHttpAgent : ITcpAgent, IHttpMultiId
    {
        #region Http客户端属性

        /// <summary>
        /// 获取或设置是否使用 Cookie
        /// </summary>
        bool IsUseCookie { get; set; }

        #endregion

        #region Http客户端事件

        /// <summary>
        /// 【可选】状态行解析完成（仅用于 HTTP 客户端）
        /// </summary>
        event StatusLineEventHandler OnStatusLine;

        #endregion

        #region Http客户端方法

        /// <summary>
        /// 发送web socket消息
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="final"></param>
        /// <param name="rsv"></param>
        /// <param name="opCode"></param>
        /// <param name="mask"></param>
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        bool SendWsMessage(IntPtr connId, bool final, Rsv rsv, OpCode opCode, byte[] mask, byte[] data, int length);

        /// <summary>
        /// 发送请求
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="method">http method</param>
        /// <param name="path">请求路径</param>
        /// <param name="headers">请求头</param>
        /// <param name="body">请求体</param>
        /// <param name="length">请求体长度</param>
        /// <returns></returns>
        bool SendRequest(IntPtr connId, HttpMethod method, string path, List<NameValue> headers, byte[] body, int length);

        /// <summary>
        /// 发送请求
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="method">http method</param>
        /// <param name="path">请求路径</param>
        /// <param name="headers">请求头</param>
        /// <returns></returns>
        bool SendRequest(IntPtr connId, HttpMethod method, string path, List<NameValue> headers);

        /// <summary>
        /// 发送本地小文件
        /// <para>向指定连接发送 4096 KB 以下的小文件</para>
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="method"></param>
        /// <param name="path"></param>
        /// <param name="headers"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        bool SendSmallFile(IntPtr connId, HttpMethod method, string path, List<NameValue> headers, string filePath);


        /// <summary>
        /// 发送 POST 请求
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="path">请求路径</param>
        /// <param name="headers">请求头</param>
        /// <param name="body">请求体</param>
        /// <param name="length">请求体长度</param>
        /// <returns></returns>
        [Obsolete("该方法已过期, 推荐使用body参数为byte[]类型的重载方法", false)]
        bool SendPost(IntPtr connId, string path, List<NameValue> headers, string body, int length);

        /// <summary>
        /// 发送 POST 请求
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="path">请求路径</param>
        /// <param name="headers">请求头</param>
        /// <param name="body">请求体</param>
        /// <param name="length">请求体长度</param>
        /// <returns></returns>
        bool SendPost(IntPtr connId, string path, List<NameValue> headers, byte[] body, int length);

        /// <summary>
        /// 发送 PUT 请求
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="path">请求路径</param>
        /// <param name="headers">请求头</param>
        /// <param name="body">请求体</param>
        /// <param name="length">请求体长度</param>
        /// <returns></returns>
        [Obsolete("该方法已过期, 推荐使用body参数为byte[]类型的重载方法", false)]
        bool SendPut(IntPtr connId, string path, List<NameValue> headers, string body, int length);

        /// <summary>
        /// 发送 PUT 请求
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="path">请求路径</param>
        /// <param name="headers">请求头</param>
        /// <param name="body">请求体</param>
        /// <param name="length">请求体长度</param>
        /// <returns></returns>
        bool SendPut(IntPtr connId, string path, List<NameValue> headers, byte[] body, int length);

        /// <summary>
        /// 发送 PATCH 请求
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="path">请求路径</param>
        /// <param name="headers">请求头</param>
        /// <param name="body">请求体</param>
        /// <param name="length">请求体长度</param>
        /// <returns></returns>
        [Obsolete("该方法已过期, 推荐使用body参数为byte[]类型的重载方法", false)]
        bool SendPatch(IntPtr connId, string path, List<NameValue> headers, string body, int length);

        /// <summary>
        /// 发送 PATCH 请求
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="path">请求路径</param>
        /// <param name="headers">请求头</param>
        /// <param name="body">请求体</param>
        /// <param name="length">请求体长度</param>
        /// <returns></returns>
        bool SendPatch(IntPtr connId, string path, List<NameValue> headers, byte[] body, int length);

        /// <summary>
        /// 发送 GET 请求
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="path">请求路径</param>
        /// <param name="headers">请求头</param>
        /// <returns></returns>
        bool SendGet(IntPtr connId, string path, List<NameValue> headers);

        /// <summary>
        /// 发送 DELETE 请求
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="path">请求路径</param>
        /// <param name="headers">请求头</param>
        /// <returns></returns>
        bool SendDelete(IntPtr connId, string path, List<NameValue> headers);

        /// <summary>
        /// 发送 HEAD 请求
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="path">请求路径</param>
        /// <param name="headers">请求头</param>
        /// <returns></returns>
        bool SendHead(IntPtr connId, string path, List<NameValue> headers);

        /// <summary>
        /// 发送 TRACE 请求
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="path">请求路径</param>
        /// <param name="headers">请求头</param>
        /// <returns></returns>
        bool SendTrace(IntPtr connId, string path, List<NameValue> headers);

        /// <summary>
        /// 发送 OPTIONS 请求
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="path">请求路径</param>
        /// <param name="headers">请求头</param>
        /// <returns></returns>
        bool SendOptions(IntPtr connId, string path, List<NameValue> headers);

        /// <summary>
        /// 发送 CONNECT 请求
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="path">请求路径</param>
        /// <param name="headers">请求头</param>
        /// <returns></returns>
        bool SendConnect(IntPtr connId, string path, List<NameValue> headers);

        /// <summary>
        /// 获取 HTTP 状态码
        /// </summary>
        /// <param name="connId"></param>
        /// <returns></returns>
        HttpStatusCode GetStatusCode(IntPtr connId);

        #endregion
    }
}
