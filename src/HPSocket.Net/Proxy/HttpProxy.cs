using System;
using System.Text;

namespace HPSocket.Proxy
{
    public class HttpProxy : Base.Proxy, IHttpProxy
    {
        /// <summary>
        /// 设置远程地址端口
        /// </summary>
        /// <param name="remoteAddress"></param>
        /// <param name="remotePort"></param>
        public void SetRemoteAddressPort(string remoteAddress, ushort remotePort)
        {
            RemoteAddress = remoteAddress;
            RemotePort = remotePort;
        }

        /// <summary>
        /// 获取连接代理需要的数据
        /// </summary>
        /// <returns></returns>
        public byte[] GetConnectData()
        {
            var authorization = "";
            if (!String.IsNullOrWhiteSpace(UserName) && !String.IsNullOrWhiteSpace(Password))
            {
                authorization = $"Proxy-Authorization: Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{UserName}:{Password}"))}\r\n";
            }

            var userAgent = "";
            if (!String.IsNullOrWhiteSpace(UserAgent))
            {
                userAgent = $"\r\nUser-Agent: {UserAgent}";
            }
            var connect = $@"CONNECT {RemoteAddress}:{RemotePort} HTTP/1.1
Host: {RemoteAddress}{userAgent}
Proxy-Connection: Keep-Alive
{authorization}
";
            return Encoding.UTF8.GetBytes(connect);
        }

        /// <summary>
        /// 是否连接代理成功
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool IsConnected(byte[] data)
        {
            var response = Encoding.UTF8.GetString(data);
            if (!response.StartsWith("HTTP/1.1 200 Connection Established"))
            {
                if (!response.Replace(" ", "").ToLower().StartsWith("http/1.1 200 connection established"))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
