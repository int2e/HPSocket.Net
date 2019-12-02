#if !NET20 && !NET30 && !NET35
using System.Threading.Tasks;
#endif
using HPSocket.Udp;

namespace HPSocket
{
    /// <summary>
    /// udp node
    /// </summary>
    public interface IUdpNode : ISocket
    {
        #region UdpNode基础属性

        /// <summary>
        /// 监听地址，默认0.0.0.0
        /// </summary>
        string Address { get; set; }

        /// <summary>
        /// 本地端口，默认0
        /// </summary>
        ushort Port { get; set; }

        #endregion

        #region UdpNode事件

        /// <summary>
        /// 准备监听了事件
        /// </summary>
        event UdpNodePrepareListenEventHandler OnPrepareListen;
        /// <summary>
        /// 数据包发送事件
        /// </summary>
        event UdpNodeSendEventHandler OnSend;
        /// <summary>
        /// 数据到达事件
        /// </summary>
        event UdpNodeReceiveEventHandler OnReceive;
        /// <summary>
        /// 发生了错误事件
        /// </summary>
        event UdpNodeErrorEventHandler OnError;
        /// <summary>
        /// 关闭服务事件
        /// </summary>
        event UdpNodeShutdownEventHandler OnShutdown;

        #endregion

        #region UdpNode属性

        /// <summary>
        /// 检查通信组件是否已启动
        /// </summary>
        bool HasStarted { get; }

        /// <summary>
        /// 查看通信组件当前状态
        /// </summary>
        /// <returns></returns>
        ServiceState State { get; }

        /// <summary>
        /// 获取传播模式
        /// </summary>
        CastMode CastMode { get; }

        /// <summary>
        /// 获取未发出数据的长度
        /// </summary>
        int PendingDataLength { get; }

        /// <summary>
        /// 获取或设置数据报文最大长度（建议在局域网环境下不超过 1432 字节，在广域网环境下不超过 548 字节）
        /// </summary>
        uint MaxDatagramSize { get; set; }

        /// <summary>
        /// 获取或设置是否使用地址重用机制（默认：不启用）
        /// </summary>
        bool IsReuseAddress { get; set; }

        /// <summary>
        /// 获取或设置组播报文的 TTL（0 - 255）
        /// </summary>
        int MultiCastTtl { get; set; }

        /// <summary>
        /// 获取或设置是否启用组播环路
        /// </summary>
        bool IsMultiCastLoop { get; set; }

        /// <summary>
        /// 获取或设置工作线程数量（通常设置为 2 * CPU + 2）
        /// </summary>
        uint WorkerThreadCount { get; set; }

        /// <summary>
        /// 获取或设置 Receive 预投递数量（根据负载调整设置，Receive 预投递数量越大则丢包概率越小）
        /// </summary>
        uint PostReceiveCount { get; set; }

        /// <summary>
        /// 获取或设置内存块缓存池大小
        /// </summary>
        /// <returns></returns>
        uint FreeBufferPoolSize { get; set; }

        /// <summary>
        /// 获取或设置内存块缓存池回收阀值
        /// </summary>
        uint FreeBufferPoolHold { get; set; }

        /// <summary>
        /// 获取或设置地址重用选项
        /// </summary>
        ReuseAddressPolicy ReuseAddressPolicy { get; set; }

        /// <summary>
        /// 获取错误码
        /// </summary>
        SocketError ErrorCode { get; }

        /// <summary>
        /// 获取错误信息
        /// </summary>
        string ErrorMessage { get; }

        /// <summary>
        /// 附加数据
        /// </summary>
        object ExtraData { get; set; }

        #endregion

        #region UdpNode方法

        /// <summary>
        /// 启动 UDP 节点通信组件，启动完成后可开始收发数据
        /// </summary>
        /// <returns></returns>
        bool Start();

        /// <summary>
        /// 启动 UDP 节点通信组件，启动完成后可开始收发数据
        /// </summary>
        /// <param name="castMode">传播模式（默认：UniCast）</param>
        /// <param name="castAddress">传播地址（默认：null，当 caseMode 为 Multicast 或 Broadcast 时有效）</param>
        /// <returns></returns>
        bool StartWithCast(CastMode castMode = CastMode.UniCast, string castAddress = null);

        /// <summary>
        /// 关闭通信组件
        /// </summary>
        /// <returns></returns>
        bool Stop();

        /// <summary>
        /// 向指定地址发送数据
        /// </summary>
        /// <param name="remoteAddress"></param>
        /// <param name="remotePort"></param>
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        bool Send(string remoteAddress, ushort remotePort, byte[] data, int length);

        /// <summary>
        /// 向指定地址发送数据
        /// </summary>
        /// <param name="remoteAddress"></param>
        /// <param name="remotePort"></param>
        /// <param name="data"></param>
        /// <param name="offset">发送缓冲区偏移量</param>
        /// <param name="length"></param>
        /// <returns></returns>
        bool Send(string remoteAddress, ushort remotePort, byte[] data, int offset, int length);

        /// <summary>
        /// 发送多组数据
        /// <para>向指定地址发送多组数据，把所有数据包组合成一个数据包发送（数据包的总长度不能大于设置的 UDP 包最大长度） </para>
        /// </summary>
        /// <param name="remoteAddress"></param>
        /// <param name="remotePort"></param>
        /// <param name="buffers">发送缓冲区数组</param>
        /// <param name="count">发送缓冲区数目</param>
        /// <returns></returns>
        bool Send(string remoteAddress, ushort remotePort, Wsabuf[] buffers, int count);

        /// <summary>
        /// 向传播地址发送数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        bool SendCast(byte[] data, int length);

        /// <summary>
        /// 向传播地址发送数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset">发送缓冲区偏移量</param>
        /// <param name="length"></param>
        /// <returns></returns>
        bool SendCast(byte[] data, int offset, int length);

        /// <summary>
        /// 发送多组数据
        /// <para>向传播地址发送多组数据，把所有数据包组合成一个数据包发送（数据包的总长度不能大于设置的 UDP 包最大长度） </para>
        /// </summary>
        /// <param name="buffers">发送缓冲区数组</param>
        /// <param name="count">发送缓冲区数目</param>
        /// <returns></returns>
        bool SendCast(Wsabuf[] buffers, int count);


        /**********************************************************************************/
        /****************************** UDP Node 属性访问方法 ******************************/

        /// <summary>
        /// 获取本节点地址
        /// </summary>
        /// <param name="address"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        bool GetLocalAddress(out string address, out ushort port);

        /// <summary>
        /// 获取本节点传播地址
        /// </summary>
        /// <param name="address"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        bool GetCastAddress(out string address, out ushort port);

        #endregion
    }
}
