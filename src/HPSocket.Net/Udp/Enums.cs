namespace HPSocket.Udp
{
    /// <summary>
    /// 播送模式  UDP 组件的播送模式（组播或广播）
    /// </summary>
    public enum CastMode
    {
        /// <summary>
        /// 单播
        /// </summary>
        UniCast = -1,
        /// <summary>
        /// 组播
        /// </summary>
        // ReSharper disable once IdentifierTypo
        Multicast = 0,
        /// <summary>
        /// 广播
        /// </summary>
        Broadcast = 1,

    }
}
