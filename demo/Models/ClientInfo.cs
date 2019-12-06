using System;
using System.Collections.Generic;

namespace Models
{
    /// <summary>
    /// 客户信息
    /// </summary>
    public class ClientInfo
    {
        /// <summary>
        /// 连接id
        /// </summary>
        public IntPtr ConnId { get; set; }

        /// <summary>
        /// 封包数据
        /// </summary>
        public List<byte> PacketData { get; set; }

    }


    /// <summary>
    /// 客户信息
    /// </summary>
    public class ClientInfo<TDataType> : ClientInfo
    {
        /// <summary>
        /// 封包数据
        /// </summary>
        public new TDataType PacketData { get; set; }
    }
}
