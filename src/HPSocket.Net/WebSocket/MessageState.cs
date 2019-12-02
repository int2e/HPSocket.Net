namespace HPSocket.WebSocket
{
    /// <summary>
    /// web socket message state
    /// </summary>
    public class MessageState
    {
        /// <summary>
        /// 是否结束帧
        /// <para>是否是最后1帧，1个消息由1个或多个数据帧构成，若消息由1帧构成，起始帧就是结束帧。</para>
        /// </summary>
        public bool Final { get; set; }

        /// <summary>
        /// RSV1/RSV2/RSV3 各 1 位
        /// </summary>
        public Rsv Rsv { get; set; }

        /// <summary>
        /// 帧类型
        /// </summary>
        public OpCode OpCode { get; set; }

        /// <summary>
        /// 掩码
        /// </summary>
        public byte[] Mask { get; set; }

        /// <summary>
        /// 消息总长度
        /// <para>bodyLen 等于 0: 消息总长度为 length</para>
        /// <para>bodyLen 等于 length: 消息总长度为 bodyLen</para>
        /// <para>bodyLen 大于 0: 消息总长度为 bodyLen，后续消息体长度为 bodyLen - length，后续消息体通过底层方法 Text() / SendPackets() 发送</para>
        /// <para>bodyLen 小于 0: 错误参数，发送失败</para>
        /// </summary>
        public ulong BodyLength { get; set; }

        /// <summary>
        /// 剩余
        /// </summary>
        public ulong BodyRemain { get; set; }
    }
}
