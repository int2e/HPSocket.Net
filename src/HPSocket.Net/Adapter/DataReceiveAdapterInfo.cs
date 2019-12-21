using System;
using System.Collections.Generic;

namespace HPSocket.Adapter
{
    /// <summary>
    /// 数据接收适配器信息
    /// </summary>
    public class DataReceiveAdapterInfo
    {
        /// <summary>
        /// 连接id
        /// </summary>
        internal IntPtr ConnId { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        internal List<byte> Data { get; set; }
    }
}
