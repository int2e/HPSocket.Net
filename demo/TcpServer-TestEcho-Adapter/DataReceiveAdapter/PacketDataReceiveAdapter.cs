using System;
using System.Text;
using HPSocket.Adapter;
using Models;
using Newtonsoft.Json;

namespace TcpServerTestEchoAdapter.DataReceiveAdapter
{
    /// <summary>
    /// Packet类型数据接收适配器
    /// </summary>
    public class PacketDataReceiveAdapter : FixedHeaderDataReceiveAdapter<Packet>
    {
        /// <summary>
        /// 构造函数调用父类构造, 指定固定包头的长度及最大封包长度
        /// </summary>
        public PacketDataReceiveAdapter()
            : base(
                headerSize: 4,// 这里指定4字节包头
                maxPacketSize: 0x1000 // 这里指定最大封包长度不能超过4K
                )
        {

        }

        /// <summary>
        /// 获取请求体长度方法
        /// <remarks>子类必须覆盖此方法</remarks>
        /// </summary>
        /// <param name="header">包头, header的长度是构造函数里指定的长度, 构造函数里指定包头长度多少, 父类就等到了指定长度在调用此方法</param>
        /// <returns>返回真实长度</returns>
        protected override int GetBodySize(byte[] header)
        {
            // 适配大小端请根据自己的业务场景来, 两端字节序要保持一致

            // 如果当前环境不是小端字节序
            if (!BitConverter.IsLittleEndian)
            {
                // 翻转字节数组, 变为小端字节序
                Array.Reverse(header);
            }

            // 因为我是4字节包头, 所以直接转int并返回
            return BitConverter.ToInt32(header, 0);
        }

        /// <summary>
        /// 解析请求体
        /// <remarks>子类必须重写此方法</remarks>
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected override Packet ParseRequestBody(byte[] data)
        {
            // 将data转换为泛型传入的类型对象
            // 我这里是Packet类的对象
            return JsonConvert.DeserializeObject<Packet>(Encoding.UTF8.GetString(data));
        }
    }
}
