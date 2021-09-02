namespace HPSocket.Ssl
{


    /// <summary>
    /// SSL WebSocketSession 信息类型，用于 GetSSLSessionInfo()，标识输出的 WebSocketSession 信息类型
    /// </summary>
    public enum SslSessionInfo
    {
        /// <summary>
        /// min
        /// </summary>
        Min = 0,
        /// <summary>
        ///  SSL CTX（输出类型：SSL_CTX*）
        /// </summary>
        Ctx = 0,
        /// <summary>
        /// SSL CTX Method （输出类型：SSL_METHOD*）
        /// </summary>
        CtxMethod = 1,
        /// <summary>
        /// SSL CTX Ciphers （输出类型：STACK_OF(SSL_CIPHER)*）
        /// </summary>
        CtxCiphers = 2,
        /// <summary>
        /// SSL CTX Cert Store （输出类型：X509_STORE*）
        /// </summary>
        CtxCertStore = 3,
        /// <summary>
        /// Server Name Type （输出类型：int）
        /// </summary>
        ServerNameType = 4,
        /// <summary>
        /// Server Name （输出类型：LPCSTR）
        /// </summary>
        ServerName = 5,
        /// <summary>
        /// SSL Version （输出类型：LPCSTR）
        /// </summary>
        Version = 6,
        /// <summary>
        /// SSL Method （输出类型：SSL_METHOD*）
        /// </summary>
        Method = 7,
        /// <summary>
        /// SSL Cert （输出类型：X509*）
        /// </summary>
        Cert = 8,
        /// <summary>
        /// SSL Private Name （输出类型：EVP_PKEY*）
        /// </summary>
        PrivateKey = 9,
        /// <summary>
        /// SSL Current Cipher （输出类型：SSL_CIPHER*）
        /// </summary>
        CurrentCipher = 10,
        /// <summary>
        /// SSL Available Ciphers（输出类型：STACK_OF(SSL_CIPHER)*）
        /// </summary>
        Ciphers = 11,
        /// <summary>
        /// SSL Client Ciphers （输出类型：STACK_OF(SSL_CIPHER)*）
        /// </summary>
        ClientCiphers = 12,
        /// <summary>
        /// SSL Peer Cert （输出类型：X509*）
        /// </summary>
        PeerCert = 13,
        /// <summary>
        /// SSL Peer Cert Chain （输出类型：STACK_OF(X509)*）
        /// </summary>
        PeerCertChain = 14,
        /// <summary>
        /// SSL Verified Chain （输出类型：STACK_OF(X509)*）
        /// </summary>
        VerifiedChain = 15,
        /// <summary>
        /// max
        /// </summary>
        Max = 15,
    }

    /// <summary>
    /// 标识 SSL 的工作模式，客户端模式或服务端模式
    /// </summary>
    public enum SslSessionMode
    {
        /// <summary>
        /// 客户端模式
        /// </summary>
        Client = 0,
        /// <summary>
        /// 服务端模式
        /// </summary>
        Server = 1,
    }

    /// <summary>
    /// SSL 验证模式选项，SSL_VM_PEER 可以和后面两个选项组合一起
    /// </summary>
    public enum SslVerifyMode
    {
        /// <summary>
        /// SSL_VERIFY_NONE
        /// </summary>
        None = 0x00,
        /// <summary>
        /// SSL_VERIFY_PEER
        /// </summary>
        Peer = 0x01,
        /// <summary>
        /// SSL_VERIFY_FAIL_IF_NO_PEER_CERT
        /// </summary>
        FailIfNoPeerCert = 0x02,
        /// <summary>
        /// SSL_VERIFY_CLIENT_ONCE
        /// </summary>
        ClientOnce = 0x04,
    }

    public enum SslCipherList
    {
        SSL_CTX_set_cipher_list
    }
}
