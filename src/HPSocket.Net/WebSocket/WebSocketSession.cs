using System.Collections.Generic;

namespace HPSocket.WebSocket
{
    /// <summary>
    /// web socket 连接对象的会话
    /// </summary>
    internal class WebSocketSession
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
        /// 访问路径
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 协议
        /// </summary>
        public string SecWebSocketProtocol { get; set; }

        /// <summary>
        /// 扩展
        /// </summary>
        public string SecWebSocketExtensions { get; set; }

        /// <summary>
        /// Key
        /// </summary>
        public string SecWebSocketKey { get; set; }

        /// <summary>
        /// 每帧压缩方法
        /// </summary>
        public CompressionMethod Compression { get; set; }

        /// <summary>
        /// 连接对象的完整数据
        /// </summary>
        public List<byte> Data { get; set; }
    }
}
