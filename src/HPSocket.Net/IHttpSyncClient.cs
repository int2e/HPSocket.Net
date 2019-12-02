using System.Collections.Generic;
using System.Text;
using HPSocket.Http;

namespace HPSocket
{
    /// <summary>
    /// http sync client
    /// </summary>
    public interface IHttpSyncClient : IHttpClient
    {
        #region Http同步客户端属性

        /// <summary>
        /// 获取或设置连接超时时间
        /// <para>（毫秒，0：系统默认超时，默认：5000）</para>
        /// </summary>
        uint ConnectTimeout { get; set; }

        /// <summary>
        /// 获取或设置请求超时时间
        /// <para>（毫秒，0：无限等待，默认：10000）</para>
        /// </summary>
        uint RequestTimeout { get; set; }

        /// <summary>
        /// 响应编码,
        /// <para>如果为空, 组件默认会根据响应头的Content-Type尝试寻找charset的值做解码编码</para>
        /// <para>如果Content-Type里没有charset, 则默认使用utf-8编码进行解码</para>
        /// </summary>
        Encoding ResponseEncoding { get; set; }

        #endregion

        #region Http同步客户端方法

        /// <summary>
        /// 发送 URL 请求
        /// <para>向服务端发送 HTTP URL 请求</para>
        /// </summary>
        /// <param name="method">http method</param>
        /// <param name="url">请求url</param>
        /// <param name="headers">请求头</param>
        /// <param name="body">请求体</param>
        /// <param name="length">请求体长度</param>
        /// <param name="forceReconnect">是否强制重新连接（默认：false，当请求 URL 的主机和端口与现有连接一致时，重用现有连接）</param>
        /// <returns></returns>
        bool OpenUrl(HttpMethod method, string url, List<NameValue> headers, byte[] body, int length, bool forceReconnect = false);

        /// <summary>
        /// 清除请求结果
        /// <para>清除上一次请求的响应头和响应体等结果信息（该方法会在每次发送请求前自动调用）</para>
        /// </summary>
        /// <returns></returns>
        bool CleanupRequestResult();

        /// <summary>
        /// 获取响应体
        /// </summary>
        /// <returns></returns>
        string GetResponseBody();

        #endregion
    }
}
