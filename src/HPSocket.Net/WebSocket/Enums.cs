namespace HPSocket.WebSocket
{
    /// <summary>
    /// 指定压缩方法
    /// </summary>
    public enum CompressionMethod : byte
    {
        /// <summary>
        /// 不指定压缩
        /// </summary>
        None,
        /// <summary>
        /// 指定 DEFLATE
        /// </summary>
        Deflate
    }

    /// <summary>
    /// 帧类型
    /// </summary>
    public enum OpCode : byte
    {
        /// <summary>
        /// 延续帧 - 非控制帧
        /// </summary>
        Cont = 0x0,
        /// <summary>
        /// 文本帧 - 非控制帧
        /// </summary>
        Text = 0x1,
        /// <summary>
        /// 二进制帧 - 非控制帧
        /// </summary>
        Binary = 0x2,
        /// <summary>
        /// 关闭帧 - 控制帧
        /// </summary>
        Close = 0x8,
        /// <summary>
        /// ping帧 - 控制帧
        /// </summary>
        Ping = 0x9,
        /// <summary>
        /// pong帧 - 控制帧
        /// </summary>
        Pong = 0xa,
    }

    /// <summary>
    /// 指示 web socket 帧的每个RSV（RSV1、RSV2和RSV3）的值
    /// </summary>
    public enum Rsv : byte
    {
        /// <summary>
        /// 表示全部为0
        /// </summary>
        Off = 0x0,

        /// <summary>
        /// 表示 RSV1 = 1, 压缩位
        /// </summary>
        Compression = 0x4,
    }
}
