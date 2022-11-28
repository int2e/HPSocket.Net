namespace HPSocket
{
    /// <summary>
    /// https agent
    /// </summary>
    public interface IHttpsSyncClient : IHttpsClient, IHttpSyncClient
    {
        /// <summary>
        /// 忽略证书验证
        /// </summary>
        bool IgnoreCertificateValidation { get; set; }
    }
}
