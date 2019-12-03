using System;

namespace HPSocket
{
    /// <summary>
    /// client 基础接口
    /// </summary>
    public interface IClient : ISocket
    {
        #region 基础属性

        /// <summary>
        /// 远程服务器地址
        /// </summary>
        string Address { get; set; }

        /// <summary>
        /// 远程服务器端口
        /// </summary>
        ushort Port { get; set; }

        /// <summary>
        /// 本地绑定到哪个ip
        /// </summary>
        string BindAddress { get; set; }

        /// <summary>
        /// 本地绑定到哪个端口
        /// </summary>
        ushort BindPort { get; set; }

        /// <summary>
        /// 是否异步连接，默认为真
        /// </summary>
        bool Async { get; set; }

        /// <summary>
        /// 附加数据
        /// <para>赋值：client.ExtraData = myObj;</para>
        /// <para>取值：var data = ExtraData as MyData;</para>
        /// </summary>
        object ExtraData { get; set; }

        #endregion

        #region 客户端事件

        /// <summary>
        /// 准备连接了事件
        /// </summary>
        event ClientPrepareConnectEventHandler OnPrepareConnect;

        /// <summary>
        /// 连接事件
        /// </summary>
        event ClientConnectEventHandler OnConnect;

        /// <summary>
        /// 数据发送事件
        /// </summary>
        event ClientSendEventHandler OnSend;

        /// <summary>
        /// 数据到达事件
        /// </summary>
        event ClientReceiveEventHandler OnReceive;

        /// <summary>
        /// 连接关闭事件
        /// </summary>
        event ClientCloseEventHandler OnClose;

        /// <summary>
        /// 握手事件
        /// </summary>
        event ClientHandShakeEventHandler OnHandShake;

        #endregion

        #region 客户端属性
        /// <summary>
        /// 读取或设置内存块缓存池大小（通常设置为 -> PUSH 模型：5 - 10；PULL 模型：10 - 20 ）
        /// </summary>
        uint FreeBufferPoolSize { get; set; }

        /// <summary>
        ///  读取或设置内存块缓存池回收阀值（通常设置为内存块缓存池大小的 3 倍）
        /// </summary>
        uint FreeBufferPoolHold { get; set; }

        /// <summary>
        /// 检查通信组件是否已启动
        /// </summary>
        bool HasStarted { get; }

        /// <summary>
        /// 是否已连接
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// 状态
        /// </summary>
        ServiceState State { get; }

        /// <summary>
        /// 获取该组件对象的连接Id
        /// </summary>
        IntPtr ConnectionId { get; }

        /// <summary>
        /// 是否为安全连接（SSL/HTTPS）
        /// </summary>
        bool IsSecure { get; }

        /// <summary>
        /// 获取或设置暂停接收状态，设置状态时，不允许设置为ReceiveState.Unknown，
        /// </summary>
        ReceiveState PauseReceive { get; set; }

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

        #endregion

        #region 客户端方法

        /// <summary>
        /// 启动通讯组件并连接到服务器
        /// </summary>
        /// <returns></returns>
        bool Connect();

        /// <summary>
        /// 启动通讯组件并连接到服务器
        /// </summary>
        /// <param name="address">远程服务器地址</param>
        /// <param name="port">远程服务器端口</param>
        /// <returns></returns>
        bool Connect(string address, ushort port);

        /// <summary>
        /// 停止服务
        /// </summary>
        /// <returns></returns>
        bool Stop();

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        bool Send(byte[] bytes, int length);

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="offset">针对bytes的偏移</param>
        /// <param name="length">发多大</param>
        /// <returns></returns>
        bool Send(byte[] bytes, int offset, int length);

        /// <summary>
        /// 发送多组数据
        /// 向指定连接发送多组数据
        /// TCP - 顺序发送所有数据包
        /// </summary>
        /// <param name="buffers">发送缓冲区数组</param>
        /// <param name="count">发送缓冲区数目</param>
        /// <returns>true.成功,false.失败，可通过 SYSGetLastError() 获取 Windows 错误代码</returns>
        bool SendPackets(Wsabuf[] buffers, int count);

        /// <summary>
        /// 获取连接中未发出数据的长度
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        bool GetPendingDataLength(out int length);

        /// <summary>
        /// 获取监听socket的地址信息
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        bool GetListenAddress(out string host, out ushort port);

        /// <summary>
        /// 获取连接的远程主机信息
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        bool GetRemoteHost(out string host, out ushort port);

        #endregion
    }
}
