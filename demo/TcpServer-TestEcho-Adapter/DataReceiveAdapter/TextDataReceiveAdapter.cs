using System.Text;
using HPSocket.Adapter;

namespace TcpServerTestEchoAdapter.DataReceiveAdapter
{
    /// <summary>
    /// 定长包数据接收适配器
    /// </summary>
    public class TextDataReceiveAdapter : TerminatorDataReceiveAdapter<string>
    {
        /// <summary>
        /// 构造函数调用父类构造, 指定定长包长度
        /// </summary>
        public TextDataReceiveAdapter()
            : base(
                terminator: Encoding.UTF8.GetBytes("\r\n") // 指定终止符为\r\n, 也就是说每条数据以\r\n结尾, 注意编码问题, 要两端保持一致
                )
        {
        }

        /// <summary>
        /// 解析请求体
        /// <remarks>子类必须覆盖此方法</remarks>
        /// </summary>
        /// <param name="data">父类处理好不含结束符的数据</param>
        /// <returns></returns>
        protected override string ParseRequestBody(byte[] data)
        {
            // 转换成请求对象, 注意编码问题, 要两端保持一致
            return Encoding.UTF8.GetString(data);
        }
    }
}
