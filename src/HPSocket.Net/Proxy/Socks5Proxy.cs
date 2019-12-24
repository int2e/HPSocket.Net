using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using HPSocket.Sdk;

namespace HPSocket.Proxy
{
    public class Socks5Proxy : Base.Proxy, ISocks5Proxy
    {
        private static readonly byte[] NoAuthBytes = { 0x05, 0x01, 0x00 };
        private static readonly byte[] AuthBytes = { 0x05, 0x01, 0x02 };

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
            /*
            0x05 SOCKS5协议版本
            0x02 支持的认证方法数量
            0x00 免认证
            0x02 账号密码认证
            */

            if (String.IsNullOrWhiteSpace(UserName) || String.IsNullOrWhiteSpace(Password))
            {
                return NoAuthBytes;
            }

            return AuthBytes;
        }

        /// <summary>
        /// 获取认证数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public bool GetAuthenticateData(byte[] data, out byte[] bytes)
        {
            bytes = null;
            if (data.Length != 2)
            {
                return false;
            }

            if (data[1] == 0x02) // 需要帐号密码
            {
                if (String.IsNullOrWhiteSpace(UserName) || String.IsNullOrWhiteSpace(Password))
                {
                    return false;
                }

                /*
                0x01 子协商版本
                0x04 用户名长度
                0x61 0x61 0x61 0x61 转换为ascii字符之后为"aaaa"
                0x04 密码长度
                0x61 0x61 0x61 0x61 转换为ascii字符之后"aaaa"
                */
                var sendBytes = new List<byte>
                {
                    0x01,
                    (byte) UserName.Length,
                };
                sendBytes.AddRange(Encoding.ASCII.GetBytes(UserName));
                sendBytes.Add((byte)Password.Length);
                sendBytes.AddRange(Encoding.ASCII.GetBytes(Password));

                bytes = sendBytes.ToArray();
            }
            else if (data[1] == 0x00) // 无需认证
            {

            }
            else
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// 检查子版本
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool CheckSubVersion(byte[] data)
        {
            if (data[0] != 0x01) // 子协商版本
            {
                return false;
            }
            if (data[1] != 0x00) // 验证不通过
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 获取连接目标服务器的数据
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public bool GetConnectRemoteServerData(out byte[] bytes)
        {
            /*
             0x05 SOCKS协议版本
             0x01 CONNECT命令
             0x01 RSV保留字段
             0x01 地址类型为IPV4; 0X03 该地址包含一个完全的域名; 0X04 该地址是IPv6地址
             0x7f 0x00 0x00 0x01 目标服务器IP为127.0.0.1
             0x00 0x50 目标服务器端口为80
            */
            bytes = null;
            if (!Sys.GetHostAddressType(RemoteAddress, out var ipAddressType) ||
                ipAddressType == IpAddressType.All)
            {
                return false;
            }

            var sendBytes = new List<byte>
            {
                0x05, 0x01, 0x01,
            };

            switch (ipAddressType)
            {
                case IpAddressType.Ipv4:
                    sendBytes.Add(0x01);
                    sendBytes.AddRange(IPAddress.Parse(RemoteAddress).GetAddressBytes());
                    break;
                case IpAddressType.Domain:
                    sendBytes.Add(0x03);
                    sendBytes.Add((byte)RemoteAddress.Length);
                    break;
                case IpAddressType.Ipv6:
                    sendBytes.Add(0x04);
                    sendBytes.AddRange(IPAddress.Parse(RemoteAddress).GetAddressBytes());
                    break;
                default:
                    return false;
            }

            sendBytes.AddRange(BitConverter.GetBytes((ushort)IPAddress.HostToNetworkOrder((short)RemotePort)));
            bytes = sendBytes.ToArray();
            return true;
        }

        /// <summary>
        /// 是否连接代理成功
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool IsConnected(byte[] data)
        {
            /*
            VERSION SOCKS协议版本，固定0x05
            RESPONSE 响应命令

            0x00 代理服务器连接目标服务器成功
            0x01 代理服务器故障
            0x02 代理服务器规则集不允许连接
            0x03 网络无法访问
            0x04 目标服务器无法访问（主机名无效）
            0x05 连接目标服务器被拒绝
            0x06 TTL已过期
            0x07 不支持的命令
            0x08 不支持的目标服务器地址类型
            0x09 - 0xFF 未分配
            RSV 保留字段
            BND.ADDR 代理服务器连接目标服务器成功后的代理服务器IP
            BND.PORT 代理服务器连接目标服务器成功后的代理服务器端口
            */
            return data[1] == 0x00;
        }
    }
}
