using System;
using System.Collections.Generic;
using HPSocket.Http;
using HPSocket.WebSocket;

namespace HPSocket
{
    /// <summary>
    /// http server/agent 公共接口
    /// </summary>
    public interface IHttpMultiId : IHttp
    {
        #region Http多Id公共方法

        /// <summary>
        /// 向对端发送 Chunked 数据分片
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="data">Chunked 数据分片</param>
        /// <param name="length">数据分片长度（为 0 表示结束分片）</param>
        /// <param name="extensions">扩展属性（默认：null）</param>
        /// <returns></returns>
        bool SendChunkData(IntPtr connId, byte[] data, int length, string extensions = null);

        /// <summary>
        /// 发送web socket消息
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="state"></param>
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        bool SendWsMessage(IntPtr connId, MessageState state, byte[] data, int length);

        /// <summary>
        /// 启动 HTTP 通信, 当通信组件设置为非自动启动 HTTP 通信时，需要调用本方法启动 HTTP 通信
        /// </summary>
        /// <param name="connId"></param>
        /// <returns></returns>
        bool StartHttp(IntPtr connId);

        /// <summary>
        /// 获取当前 WebSocket 消息状态
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        bool GetWsMessageState(IntPtr connId, out MessageState state);

        /// <summary>
        /// 检查是否升级协议
        /// </summary>
        /// <param name="connId"></param>
        /// <returns></returns>
        bool IsUpgrade(IntPtr connId);

        /// <summary>
        /// 检查是否有 Keep-Alive 标识
        /// </summary>
        /// <param name="connId"></param>
        /// <returns></returns>
        bool IsKeepAlive(IntPtr connId);

        /// <summary>
        /// 获取协议版本
        /// </summary>
        /// <param name="connId"></param>
        /// <returns></returns>
        HttpVersion GetVersion(IntPtr connId);

        /// <summary>
        /// 获取内容长度
        /// </summary>
        /// <param name="connId"></param>
        /// <returns></returns>
        long GetContentLength(IntPtr connId);

        /// <summary>
        /// 获取内容类型
        /// </summary>
        /// <param name="connId"></param>
        /// <returns></returns>
        string GetContentType(IntPtr connId);

        /// <summary>
        /// 获取内容类型
        /// </summary>
        /// <returns></returns>
        string GetContentEncoding(IntPtr connId);

        /// <summary>
        /// 获取传输编码
        /// </summary>
        /// <returns></returns>
        string GetTransferEncoding(IntPtr connId);

        /// <summary>
        /// 获取协议升级类型
        /// </summary>
        /// <param name="connId"></param>
        /// <returns></returns>
        HttpUpgradeType GetUpgradeType(IntPtr connId);

        /// <summary>
        /// 获取解析错误代码
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        int GetParseErrorInfo(IntPtr connId, out string errorMsg);

        /// <summary>
        /// 获取某个请求头（单值）
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        string GetHeader(IntPtr connId, string name);

        /// <summary>
        /// 获取某个请求头（多值）
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        List<string> GetHeaders(IntPtr connId, string name);

        /// <summary>
        /// 获取所有请求头
        /// </summary>
        /// <param name="connId"></param>
        /// <returns></returns>
        List<NameValue> GetAllHeaders(IntPtr connId);

        /// <summary>
        /// 获取所有请求头名称
        /// </summary>
        /// <param name="connId"></param>
        /// <returns></returns>
        List<string> GetAllHeaderNames(IntPtr connId);

        /// <summary>
        /// 获取Cookie
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        string GetCookie(IntPtr connId, string key);

        /// <summary>
        /// 获取所有 Cookie
        /// </summary>
        /// <param name="connId"></param>
        /// <returns></returns>
        List<NameValue> GetAllCookies(IntPtr connId);

        #endregion
    }
}
