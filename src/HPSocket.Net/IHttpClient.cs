using System;
using System.Collections.Generic;
using HPSocket.Http;
using HPSocket.WebSocket;

namespace HPSocket
{
    /// <summary>
    /// http client
    /// </summary>
    public interface IHttpClient : ITcpClient, IHttp
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
        /// 发送请求
        /// </summary>
        /// <param name="method">http method</param>
        /// <param name="path">请求路径</param>
        /// <param name="headers">请求头</param>
        /// <param name="body">请求体</param>
        /// <param name="length">请求体长度</param>
        /// <returns></returns>
        bool SendRequest(HttpMethod method, string path, List<NameValue> headers, byte[] body, int length);

        /// <summary>
        /// 发送请求
        /// </summary>
        /// <param name="method">http method</param>
        /// <param name="path">请求路径</param>
        /// <param name="headers">请求头</param>
        /// <returns></returns>
        bool SendRequest(HttpMethod method, string path, List<NameValue> headers);

        /// <summary>
        /// 发送本地小文件
        /// <para>向指定连接发送 4096 KB 以下的小文件</para>
        /// </summary>
        /// <param name="method"></param>
        /// <param name="path"></param>
        /// <param name="headers"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        bool SendSmallFile(HttpMethod method, string path, List<NameValue> headers, string filePath);

        /// <summary>
        /// 向对端发送 Chunked 数据分片
        /// </summary>
        /// <param name="data">Chunked 数据分片</param>
        /// <param name="length">数据分片长度（为 0 表示结束分片）</param>
        /// <param name="extensions">扩展属性（默认：null）</param>
        /// <returns></returns>
        bool SendChunkData(byte[] data, int length, string extensions = null);

        /// <summary>
        /// 发送web socket消息
        /// </summary>
        /// <param name="state"></param>
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        bool SendWsMessage(MessageState state, byte[] data, int length);

        /// <summary>
        /// 发送 POST 请求
        /// </summary>
        /// <param name="path">请求路径</param>
        /// <param name="headers">请求头</param>
        /// <param name="body">请求体</param>
        /// <param name="length">请求体长度</param>
        /// <returns></returns>
        [Obsolete("该方法已过期, 推荐使用body参数为byte[]类型的重载方法", false)]
        bool SendPost(string path, List<NameValue> headers, string body, int length);

        /// <summary>
        /// 发送 POST 请求
        /// </summary>
        /// <param name="path">请求路径</param>
        /// <param name="headers">请求头</param>
        /// <param name="body">请求体</param>
        /// <param name="length">请求体长度</param>
        /// <returns></returns>
        bool SendPost(string path, List<NameValue> headers, byte[] body, int length);

        /// <summary>
        /// 发送 PUT 请求
        /// </summary>
        /// <param name="path">请求路径</param>
        /// <param name="headers">请求头</param>
        /// <param name="body">请求体</param>
        /// <param name="length">请求体长度</param>
        /// <returns></returns>
        [Obsolete("该方法已过期, 推荐使用body参数为byte[]类型的重载方法", false)]
        bool SendPut(string path, List<NameValue> headers, string body, int length);

        /// <summary>
        /// 发送 PUT 请求
        /// </summary>
        /// <param name="path">请求路径</param>
        /// <param name="headers">请求头</param>
        /// <param name="body">请求体</param>
        /// <param name="length">请求体长度</param>
        /// <returns></returns>
        bool SendPut(string path, List<NameValue> headers, byte[] body, int length);

        /// <summary>
        /// 发送 PATCH 请求
        /// </summary>
        /// <param name="path">请求路径</param>
        /// <param name="headers">请求头</param>
        /// <param name="body">请求体</param>
        /// <param name="length">请求体长度</param>
        /// <returns></returns>
        [Obsolete("该方法已过期, 推荐使用body参数为byte[]类型的重载方法", false)]
        bool SendPatch(string path, List<NameValue> headers, string body, int length);

        /// <summary>
        /// 发送 PATCH 请求
        /// </summary>
        /// <param name="path">请求路径</param>
        /// <param name="headers">请求头</param>
        /// <param name="body">请求体</param>
        /// <param name="length">请求体长度</param>
        /// <returns></returns>
        bool SendPatch(string path, List<NameValue> headers, byte[] body, int length);

        /// <summary>
        /// 发送 GET 请求
        /// </summary>
        /// <param name="path">请求路径</param>
        /// <param name="headers">请求头</param>
        /// <returns></returns>
        bool SendGet(string path, List<NameValue> headers);

        /// <summary>
        /// 发送 DELETE 请求
        /// </summary>
        /// <param name="path">请求路径</param>
        /// <param name="headers">请求头</param>
        /// <returns></returns>
        bool SendDelete(string path, List<NameValue> headers);

        /// <summary>
        /// 发送 HEAD 请求
        /// </summary>
        /// <param name="path">请求路径</param>
        /// <param name="headers">请求头</param>
        /// <returns></returns>
        bool SendHead(string path, List<NameValue> headers);

        /// <summary>
        /// 发送 TRACE 请求
        /// </summary>
        /// <param name="path">请求路径</param>
        /// <param name="headers">请求头</param>
        /// <returns></returns>
        bool SendTrace(string path, List<NameValue> headers);

        /// <summary>
        /// 发送 OPTIONS 请求
        /// </summary>
        /// <param name="path">请求路径</param>
        /// <param name="headers">请求头</param>
        /// <returns></returns>
        bool SendOptions(string path, List<NameValue> headers);

        /// <summary>
        /// 发送 CONNECT 请求
        /// </summary>
        /// <param name="path">请求路径</param>
        /// <param name="headers">请求头</param>
        /// <returns></returns>
        bool SendConnect(string path, List<NameValue> headers);

        /// <summary>
        /// 获取当前 WebSocket 消息状态
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        bool GetWsMessageState(out MessageState state);

        /// <summary>
        /// 启动 HTTP 通信, 当通信组件设置为非自动启动 HTTP 通信时，需要调用本方法启动 HTTP 通信
        /// </summary>
        /// <returns></returns>
        bool StartHttp();

        /// <summary>
        /// 获取 HTTP 状态码
        /// </summary>
        /// <returns></returns>
        HttpStatusCode GetStatusCode();

        /// <summary>
        /// 检查是否升级协议
        /// </summary>
        /// <returns></returns>
        bool IsUpgrade();

        /// <summary>
        /// 检查是否有 Keep-Alive 标识
        /// </summary>
        /// <returns></returns>
        bool IsKeepAlive();

        /// <summary>
        /// 获取协议版本
        /// </summary>
        /// <returns></returns>
        HttpVersion GetVersion();

        /// <summary>
        /// 获取内容长度
        /// </summary>
        /// <returns></returns>
        long GetContentLength();

        /// <summary>
        /// 获取内容类型
        /// </summary>
        /// <returns></returns>
        string GetContentType();

        /// <summary>
        /// 获取内容类型
        /// </summary>
        /// <returns></returns>
        string GetContentEncoding();

        /// <summary>
        /// 获取传输编码
        /// </summary>
        /// <returns></returns>
        string GetTransferEncoding();

        /// <summary>
        /// 获取协议升级类型
        /// </summary>
        /// <returns></returns>
        HttpUpgradeType GetUpgradeType();

        /// <summary>
        /// 获取解析错误代码
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        int GetParseErrorInfo(out string errorMsg);

        /// <summary>
        /// 获取某个请求头（单值）
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string GetHeader(string name);

        /// <summary>
        /// 获取某个请求头（多值）
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        List<string> GetHeaders(string name);

        /// <summary>
        /// 获取所有请求头
        /// </summary>
        /// <returns></returns>
        List<NameValue> GetAllHeaders();

        /// <summary>
        /// 获取所有请求头名称
        /// </summary>
        /// <returns></returns>
        List<string> GetAllHeaderNames();

        /// <summary>
        /// 获取Cookie
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string GetCookie(string key);

        /// <summary>
        /// 获取所有 Cookie
        /// </summary>
        /// <returns></returns>
        List<NameValue> GetAllCookies();

        #endregion
    }
}
