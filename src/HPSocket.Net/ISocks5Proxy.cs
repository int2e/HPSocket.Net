namespace HPSocket
{
    public interface ISocks5Proxy : IProxy
    {
        /// <summary>
        /// 获取连接代理需要的数据
        /// </summary>
        /// <returns></returns>
        byte[] GetConnectData();

        /// <summary>
        /// 获取认证数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        bool GetAuthenticateData(byte[] data, out byte[] bytes);

        /// <summary>
        /// 检查子版本
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool CheckSubVersion(byte[] data);

        /// <summary>
        /// 获取连接目标服务器的数据
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        bool GetConnectRemoteServerData(out byte[] bytes);

        /// <summary>
        /// 是否连接代理成功
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool IsConnected(byte[] data);
    }
}
