using System;
using HPSocket.Adapter;
using HPSocket.Ssl;

namespace HPSocket
{
    /// <summary>
    /// ssl server
    /// </summary>
    /// <typeparam name="TRequestBodyType">包体解析对象类型</typeparam>
    public interface ISslServer<TRequestBodyType> : ISslServer
    {
        [Obsolete("adapter组件无需添加OnReceive事件, 请添加OnParseRequestBody事件", true)]
        new event ServerReceiveEventHandler OnReceive;

        /// <summary>
        /// 解析请求包体对象事件
        /// </summary>
        event ParseRequestBody<ITcpServer, TRequestBodyType> OnParseRequestBody;

        /// <summary>
        /// 数据接收适配器
        /// </summary>
        DataReceiveAdapter<TRequestBodyType> DataReceiveAdapter { get; set; }
    }


    /// <summary>
    /// ssl server
    /// </summary>
    public interface ISslServer : ISsl, ITcpServer
    {
        #region 服务器方法

        /// <summary>
        /// 增加 SNI 主机证书（只用于服务端）
        /// <para>SSL 服务端在 SetupSSLContext() 成功后可以调用本方法增加多个 SNI 主机证书</para>
        /// <remarks>
        /// 返回值：正数		-- 成功，并返回 SNI 主机证书对应的索引，该索引用于在 SNI 回调函数中定位 SNI 主机
        /// 返回值：负数		-- 失败，可通过 Sys.ErrorCode() 获取失败原因
        /// </remarks>
        /// </summary>
        /// <param name="verifyMode">SSL 验证模式（参考 SslVerifyMode）</param>
        /// <param name="pemCertFile">证书文件</param>
        /// <param name="pemKeyFile">私钥文件</param>
        /// <param name="keyPassword">私钥密码（没有密码则为空）</param>
        /// <param name="caPemCert">CA 证书文件或目录（单向验证可选）</param>
        /// <returns></returns>
        int AddContext(SslVerifyMode verifyMode, string pemCertFile, string pemKeyFile, string keyPassword, string caPemCert);

        /// <summary>
        /// 增加 SNI 主机证书（通过内存加载证书）
        /// <para>SSL 服务端在 SetupSSLContext() 成功后可以调用本方法增加多个 SNI 主机证书</para>
        /// <remarks>
        /// 返回值：正数		-- 成功，并返回 SNI 主机证书对应的索引，该索引用于在 SNI 回调函数中定位 SNI 主机
        /// 返回值：负数		-- 失败，可通过 Sys.ErrorCode() 获取失败原因
        /// </remarks>
        /// </summary>
        /// <param name="verifyMode">SSL 验证模式（参考 EnSSLVerifyMode）</param>
        /// <param name="pemCert">证书内容</param>
        /// <param name="pemKey">私钥内容</param>
        /// <param name="keyPassword">私钥密码（没有密码则为空）</param>
        /// <param name="caPemCert">CA 证书内容（单向验证可选）</param>
        /// <returns></returns>
        int AddContextByMemory(SslVerifyMode verifyMode, string pemCert, string pemKey, string keyPassword = null, string caPemCert = null);

        /// <summary>
        /// 绑定 SNI 主机域名
        /// <para>SSL 服务端在 AddSSLContext() 成功后可以调用本方法绑定主机域名到 SNI 主机证书</para>
        /// <remarks>
        /// 返回值：正数		-- 成功
        /// 返回值：负数		-- 失败，可通过 Sys.ErrorCode() 获取失败原因
        /// </remarks>
        /// </summary>
        /// <param name="serverName">主机域名</param>
        /// <param name="contextIndex">SNI 主机证书对应的索引</param>
        /// <returns></returns> 
        bool BindServerName(string serverName, int contextIndex);

        /// <summary>
        /// 启动 SSL 握手，当通信组件设置为非自动握手时，需要调用本方法启动 SSL 握手
        /// </summary>
        /// <param name="connId"></param>
        /// <returns></returns>
        bool StartHandShake(IntPtr connId);

        /// <summary>
        /// 获取指定类型的 SSL WebSocketSession 信息（输出类型参考：SslSessionInfo）
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="info"></param>
        /// <param name="sessionInfo"></param>
        /// <returns></returns>
        bool GetSessionInfo(IntPtr connId, SslSessionInfo info, out IntPtr sessionInfo);

        #endregion

    }
}
