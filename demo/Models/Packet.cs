namespace Models
{
    /// <summary>
    /// 封包
    /// </summary>
    public class Packet
    {
        /// <summary>
        /// 封包类型
        /// </summary>
        public PacketType Type { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public string Data { get; set; }
    }
}
