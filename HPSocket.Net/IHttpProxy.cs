namespace HPSocket
{
    public interface IHttpProxy : IProxy
    {
        /// <summary>
        /// User-Agent, 只对 http 代理有效
        /// <para>默认HPSocket.net/2.0</para>
        /// </summary>
        string UserAgent { get; set; }

        /// <summary>
        /// 获取连接代理需要的数据
        /// </summary>
        /// <returns></returns>
        byte[] GetConnectData();

        /// <summary>
        /// 是否连接代理成功
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool IsConnected(byte[] data);
    }
}
