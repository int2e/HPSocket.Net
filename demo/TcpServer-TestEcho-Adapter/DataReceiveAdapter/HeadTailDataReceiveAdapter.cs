using System.Text;
using HPSocket.Adapter;

namespace TcpServerTestEchoAdapter.DataReceiveAdapter
{
    /// <summary>
    /// 区间数据接收适配器
    /// </summary>
    public class HeadTailDataReceiveAdapter : BetweenAndDataReceiveAdapter<string>
    {
        /// <summary>
        /// 构造函数调用父类构造, 指定区间起始和结束标志
        /// </summary>
        public HeadTailDataReceiveAdapter() 
            : base(// 例如我的数据的形式是"#*123456*#", 其中以#*开头, 以*#结尾, 中间123456部分是真实数据
                start : Encoding.UTF8.GetBytes("#*"),  // 区间起始标志, 这里以#*开始, 注意编码问题, 要两端保持一致
                end : Encoding.UTF8.GetBytes("*#")  // 区间结束标志, 这里以*#开始, 注意编码问题, 要两端保持一致
                )
        {
        }

        /// <summary>
        /// 解析请求体
        /// <remarks>子类必须覆盖此方法</remarks>
        /// </summary>
        /// <param name="data">父类处理好的不含区间起始结束标志的数据</param>
        /// <returns></returns>
        protected override string ParseRequestBody(byte[] data)
        {
            // 转换成请求对象, 注意编码问题, 要两端保持一致
            return Encoding.UTF8.GetString(data);
        }
    }
}
