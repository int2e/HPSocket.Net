using HPSocket.Ssl;

namespace HPSocket.WebSocket
{
    /// <summary>
    /// ssl configuration
    /// </summary>
    public class SslConfiguration
    {
        /// <summary>
        /// 验证模式
        /// </summary>
        public SslVerifyMode VerifyMode { get; set; }

        /// <summary>
        /// 证书文件（客户端可选）
        /// </summary>
        public string PemCertFile { get; set; }

        /// <summary>
        /// 私钥文件（客户端可选）
        /// </summary>
        public string PemKeyFile { get; set; }

        /// <summary>
        /// 私钥密码（没有密码则为空）
        /// </summary>
        public string KeyPassword { get; set; }

        /// <summary>
        /// CA 证书文件或目录（单向验证或客户端可选）
        /// </summary>
        public string CaPemCertFileOrPath { get; set; }

        /// <summary>
        /// 从内存加载证书
        /// </summary>
        public bool FromMemory { get; set; }
    }
}
