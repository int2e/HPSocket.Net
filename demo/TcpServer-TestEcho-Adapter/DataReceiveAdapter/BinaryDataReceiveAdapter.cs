using HPSocket.Adapter;

namespace TcpServerTestEchoAdapter.DataReceiveAdapter
{
    /// <summary>
    /// 定长包数据接收适配器
    /// </summary>
    public class BinaryDataReceiveAdapter : FixedSizeDataReceiveAdapter<byte[]>
    {
        /// <summary>
        /// 构造函数调用父类构造, 指定定长包长度
        /// </summary>
        public BinaryDataReceiveAdapter()
            : base(
                packetSize: 4 // 固定包长1K字节
            )
        {
        }

        /// <summary>
        /// 解析请求体
        /// <remarks>子类必须重写此方法</remarks>
        /// </summary>
        /// <param name="data">父类处理好的定长数据</param>
        /// <returns></returns>
        protected override byte[] ParseRequestBody(byte[] data)
        {
            // 因为继承自FixedSizeDataReceiveAdapter<byte[]>, 所以这里直接返回了, 如果是其他类型, 请做完转换在返回
            return data;
        }
    }
}
