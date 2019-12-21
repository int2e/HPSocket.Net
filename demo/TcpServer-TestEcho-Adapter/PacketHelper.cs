using System;
using System.Collections.Generic;
using System.Text;
using Models;
using Newtonsoft.Json;

namespace TcpServerTestEchoAdapter
{
    /// <summary>
    /// 文本数据发送帮助类
    /// </summary>
    public static class PacketHelper
    {
        /// <summary>
        /// packet对象转含包头的完整bytes
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public static byte[] ToSendBytes(Packet packet)
        {
            // 对象转json
            var json = JsonConvert.SerializeObject(packet);

            // json到bytes
            var data = Encoding.UTF8.GetBytes(json);

            // 包头转字节数组
            var header = BitConverter.GetBytes(data.Length);

            // 两端字节序要保持一致
            // 如果当前环境不是小端字节序
            if (!BitConverter.IsLittleEndian)
            {
                // 翻转字节数组, 变为小端字节序
                Array.Reverse(header);
            }

            var bytes = new List<byte>();
            // 加入包头
            bytes.AddRange(header);
            // 加入真实数据
            bytes.AddRange(data);

            return bytes.ToArray();
        }


    }
}
