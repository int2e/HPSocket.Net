using System.Collections.Generic;

namespace HPSocket
{
    /// <summary>
    /// http sync client
    /// </summary>
    public interface IHttpSyncClientEx : IHttpSyncClient
    {
        #region Http同步客户端属性

        /// <summary>
        /// http请求头
        /// </summary>
        List<NameValue> RequestHeaders { get; set; }
        
        /// <summary>
        /// http响应头
        /// </summary>
        List<NameValue> ResponseHeaders { get; set; }

        #endregion

        #region Http同步客户端方法

        /// <summary>
        /// 发送 GET 请求
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="forceReconnect">是否强制重新连接（默认：false，当请求 URL 的主机和端口与现有连接一致时，重用现有连接）</param>
        /// <returns></returns>
        bool Get(string url, bool forceReconnect = false);

        /// <summary>
        /// 发送 POST 请求
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="body">请求体</param>
        /// <param name="length">请求体长度</param>
        /// <param name="forceReconnect">是否强制重新连接（默认：false，当请求 URL 的主机和端口与现有连接一致时，重用现有连接）</param>
        /// <returns></returns>
        bool Post(string url, byte[] body, int length, bool forceReconnect = false);

        /// <summary>
        /// 发送 PUT 请求
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="body">请求体</param>
        /// <param name="length">请求体长度</param>
        /// <param name="forceReconnect">是否强制重新连接（默认：false，当请求 URL 的主机和端口与现有连接一致时，重用现有连接）</param>
        /// <returns></returns>
        bool Put(string url, byte[] body, int length, bool forceReconnect = false);

        /// <summary>
        /// 发送 DELETE 请求
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="forceReconnect">是否强制重新连接（默认：false，当请求 URL 的主机和端口与现有连接一致时，重用现有连接）</param>
        /// <returns></returns>
        bool Delete(string url, bool forceReconnect = false);

        #endregion
    }
}
