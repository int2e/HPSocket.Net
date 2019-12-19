namespace HPSocket.Ssl
{
    /// <summary>
    /// ssl 基础接口
    /// </summary>
    public interface ISsl
    {
        #region SSL属性

        /// <summary>
        /// 获取或设置 SSL 加密算法列表
        /// <remarks>使用方法请参阅:<see href="https://www.openssl.org/docs/manmaster/man3/SSL_CTX_set_cipher_list.html"/>  和 <see href="https://www.openssl.org/docs/manmaster/man1/openssl-ciphers.html"/></remarks>
        /// </summary>
        string CipherList { get; set; }

        /// <summary>
        /// 获取或设置通信组件握手方式（默认：true，自动握手)
        /// </summary>
        bool AutoHandShake { get; set; }

        /// <summary>
        /// 验证模式
        /// </summary>
        SslVerifyMode VerifyMode { get; set; }

        /// <summary>
        /// 证书文件（客户端可选）
        /// </summary>
        string PemCertFile { get; set; }

        /// <summary>
        /// 私钥文件（客户端可选）
        /// </summary>
        string PemKeyFile { get; set; }

        /// <summary>
        /// 私钥密码（没有密码则为空）
        /// </summary>
        string KeyPassword { get; set; }

        /// <summary>
        /// CA 证书文件或目录（单向验证或客户端可选）
        /// </summary>
        string CaPemCertFileOrPath { get; set; }


        #endregion


        #region SSL环境

        /// <summary>
        /// 初始化ssl环境
        /// </summary>
        /// <param name="memory">是否从内存加载证书，如果为false，[PemCertFile,PemKeyFile,CaPemCertFileOrPath]这些属性应该是文件路径;如果为true，[PemCertFile,PemKeyFile,CaPemCertFileOrPath]这些属性应该是证书文件的内容，而不是文件路径</param>
        /// <returns></returns>
        bool Initialize(bool memory);

        /// <summary>
        /// 卸载ssl环境
        /// </summary>
        void UnInitialize();

        #endregion
    }
}
