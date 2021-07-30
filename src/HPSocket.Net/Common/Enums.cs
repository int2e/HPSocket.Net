namespace HPSocket
{
    /// <summary>
    /// 通信组件服务状态,用程序可以通过通信组件的 GetState() 方法获取组件当前服务状态
    /// </summary>
    public enum ServiceState
    {
        /// <summary>
        /// 正在启动
        /// </summary>
        Starting = 0,
        /// <summary>
        /// 已经启动
        /// </summary>
        Started = 1,
        /// <summary>
        /// 正在停止
        /// </summary>
        Stopping = 2,
        /// <summary>
        /// 已经停止
        /// </summary>
        Stopped = 3,
    }

    /// <summary>
    /// Socket 操作类型,应用程序的 OnError() 事件中通过该参数标识是哪种操作导致的错误
    /// </summary>
    public enum SocketOperation
    {
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Accept
        /// </summary>
        Accept = 1,
        /// <summary>
        /// Connect
        /// </summary>
        Connect = 2,
        /// <summary>
        /// Text
        /// </summary>
        Send = 3,
        /// <summary>
        /// Receive
        /// </summary>
        Receive = 4,
        /// <summary>
        /// Receive
        /// </summary>
        Close = 5,

        /// <summary>
        /// Timed out
        /// </summary>
        TimedOut = 10060,
    }

    /// <summary>
    /// 事件通知处理结果,事件通知的返回值，不同的返回值会影响通信组件的后续行为
    /// </summary>
    public enum HandleResult
    {
        /// <summary>
        /// 成功
        /// </summary>
        Ok = 0,
        /// <summary>
        /// 忽略
        /// </summary>
        Ignore = 1,
        /// <summary>
        /// 错误
        /// </summary>
        Error = 2,
    }


    /// <summary>
    /// 操作结果代码, 组件 Start() / Stop() 方法执行失败时，可通过 ErrorCode() 获取错误代码
    /// </summary>
    public enum SocketError
    {
        /// <summary>
        /// 成功
        /// </summary>
        Ok = 0,
        /// <summary>
        /// 当前状态不允许操作
        /// </summary>
        IllegalState = 1,
        /// <summary>
        /// 非法参数
        /// </summary>
        InvalidParam = 2,
        /// <summary>
        /// 创建 SOCKET 失败
        /// </summary>
        SocketCreate = 3,
        /// <summary>
        /// 绑定 SOCKET 失败
        /// </summary>
        SocketBind = 4,
        /// <summary>
        /// 设置 SOCKET 失败
        /// </summary>
        SocketPrepare = 5,
        /// <summary>
        /// 监听 SOCKET 失败
        /// </summary>
        SocketListen = 6,
        /// <summary>
        /// 创建完成端口失败
        /// </summary>
        CpCreate = 7,
        /// <summary>
        /// 创建工作线程失败
        /// </summary>
        WorkerThreadCreate = 8,
        /// <summary>
        /// 创建监测线程失败
        /// </summary>
        DetectThreadCreate = 9,
        /// <summary>
        /// 绑定完成端口失败
        /// </summary>
        SocketAttachToCp = 10,
        /// <summary>
        /// 连接服务器失败
        /// </summary>
        ConnectServer = 11,
        /// <summary>
        /// 网络错误
        /// </summary>
        Network = 12,
        /// <summary>
        /// 数据处理错误
        /// </summary>
        DataProc = 13,
        /// <summary>
        /// 数据发送失败
        /// </summary>
        DataSend = 14,

        /***** SSL Socket 扩展操作结果代码 *****/
        /// <summary>
        /// SSL 环境未就绪
        /// </summary>
        SslEnvNotReady = 101,
    }

    /// <summary>
    /// 接收状态
    /// </summary>
    public enum ReceiveState
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unknown = -1,
        /// <summary>
        /// 唤醒状态
        /// </summary>
        Resume = 0,
        /// <summary>
        /// 暂停状态
        /// </summary>
        Pause = 1,
    }


    /// <summary>
    /// 发送策略
    /// </summary>
    public enum SendPolicy
    {
        /// <summary>
        /// 打包模式（默认）
        /// </summary>
        Pack = 0,
        /// <summary>
        /// 安全模式
        /// </summary>
        Safe = 1,
        /// <summary>
        /// 直接模式
        /// </summary>
        Direct = 2,
    }


    /// <summary>
    /// OnSend 事件同步策略
    /// <para>Server 组件和 Agent 组件的 OnSend 事件同步策略</para>
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public enum OnSendSyncPolicy
    {
        /// <summary>
        /// 不同步（默认）	：不同步 OnSend 事件，此时可能同时触发 OnReceive 和 OnClose 事件
        /// </summary>
        None = 0,
        /// <summary>
        /// 同步 OnClose	：只同步 OnClose 事件，此时可能同时触发 OnReceive 事件
        /// </summary>
        Close = 1,
        /// <summary>
        /// 同步 OnReceive	：（只用于 TCP 组件）同步 OnReceive 和 OnClose 事件，此处不可能同时触发 OnReceive 或 OnClose 事件
        /// </summary>
        Receive = 2,
    }

    /// <summary>
    /// 代理类型
    /// </summary>
    public enum ProxyType
    {
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// http
        /// </summary>
        Http,
        /// <summary>
        /// socks5
        /// </summary>
        Socks5,
    }

    /// <summary>
    /// 地址重用选项
    /// <para>通信组件底层 socket 的地址重用选项</para>
    /// </summary>
    public enum ReuseAddressPolicy
    {
        /// <summary>
        /// 不重用
        /// </summary>
        None = 0,
        /// <summary>
        /// 仅重用地址
        /// </summary>
        AddressOnly = 1,
        /// <summary>
        /// 重用地址和端口
        /// </summary>
        AddressAndPort,
    }
}
