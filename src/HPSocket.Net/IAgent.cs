using System;
using System.Collections.Generic;
using HPSocket.Tcp;

namespace HPSocket
{
    /// <summary>
    /// agent 基础接口
    /// </summary>
    public interface IAgent : ISocket
    {
        #region 基础属性

        /// <summary>
        /// 监听地址，默认0.0.0.0
        /// </summary>
        string Address { get; set; }

        /// <summary>
        /// 是否异步连接，默认为真
        /// <exception cref="InvalidOperationException">启动服务后设置此属性会引发此异常</exception>
        /// </summary>
        bool Async { get; set; }

        /// <summary>
        /// 连接超时时间, 默认操作系统默认值
        /// <para>单位: 毫秒</para>
        /// </summary>
        /// <exception cref="InvalidOperationException">同步连接、.NET Framework2.0以及设置小于100毫秒会引发此异常</exception>
        int ConnectionTimeout { get; set; }

        /// <summary>
        /// 同步接收超时, 默认操作系统默认值
        /// <para>只对同步连接有用</para>
        /// <para>单位: 毫秒</para>
        /// </summary>
        /// <exception cref="InvalidOperationException">异步连接、小于100毫秒会引发此异常</exception>
        int SyncRecvTimeout { get; set; }
        
        #endregion

        #region 客户端事件

        /// <summary>
        /// 连接到达事件
        /// </summary>
        event AgentConnectEventHandler OnConnect;

        /// <summary>
        /// 数据包发送事件
        /// </summary>
        event AgentSendEventHandler OnSend;
        /// <summary>
        /// 准备监听了事件
        /// </summary>
        event AgentPrepareConnectEventHandler OnPrepareConnect;

        /// <summary>
        /// 数据到达事件
        /// </summary>
        event AgentReceiveEventHandler OnReceive;

        /// <summary>
        /// 连接关闭事件
        /// </summary>
        event AgentCloseEventHandler OnClose;

        /// <summary>
        /// 客户端停止事件
        /// </summary>
        event AgentShutdownEventHandler OnShutdown;

        /// <summary>
        /// 握手事件
        /// </summary>
        event AgentHandShakeEventHandler OnHandShake;

        #endregion

        #region 客户端属性

        /// <summary>
        /// 获取是否启动
        /// </summary>
        bool HasStarted { get; }

        /// <summary>
        /// 获取状态
        /// </summary>
        ServiceState State { get; }

        /// <summary>
        /// 获取连接数
        /// </summary>
        uint ConnectionCount { get; }

        /// <summary>
        /// 是否为安全连接（SSL/HTTPS）
        /// </summary>
        bool IsSecure { get; }

        /// <summary>
        /// 设置最大连接数（组件会根据设置值预分配内存，因此需要根据实际情况设置，不宜过大）
        /// </summary>
        uint MaxConnectionCount { get; set; }

        /// <summary>
        /// 读取或设置工作线程数量（通常设置为 2 * CPU + 2）
        /// </summary>
        uint WorkerThreadCount { get; set; }

        /// <summary>
        /// 读取或设置 Socket 缓存对象锁定时间（毫秒，在锁定期间该 Socket 缓存对象不能被获取使用）
        /// </summary>
        uint FreeSocketObjLockTime { get; set; }

        /// <summary>
        /// 读取或设置 Socket 缓存池大小（通常设置为平均并发连接数量的 1/3 - 1/2）
        /// </summary>
        uint FreeSocketObjPool { get; set; }

        /// <summary>
        /// 读取或设置内存块缓存池大小（通常设置为 Socket 缓存池大小的 2 - 3 倍）
        /// </summary>
        uint FreeBufferObjPool { get; set; }

        /// <summary>
        /// 读取或设置内存块缓存池大小（通常设置为 Socket 缓存池大小的 2 - 3 倍）
        /// </summary>
        uint FreeSocketObjHold { get; set; }

        /// <summary>
        /// 读取或设置内存块缓存池回收阀值（通常设置为内存块缓存池大小的 3 倍）
        /// </summary>
        uint FreeBufferObjHold { get; set; }

        /// <summary>
        /// 读取或设置是否标记静默时间（设置为 true 时 DisconnectSilenceConnections() 和 GetSilencePeriod() 才有效，默认：false）
        /// </summary>
        bool IsMarkSilence { get; set; }

        /// <summary>
        /// 获取或设置数据发送策略
        /// </summary>
        SendPolicy SendPolicy { get; set; }

        /// <summary>
        /// 获取或设置 OnSend 事件同步策略
        /// </summary>
        // ReSharper disable once InconsistentNaming
        OnSendSyncPolicy OnSendSyncPolicy { get; set; }

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
        /// socks5 代理列表
        /// </summary>
        List<IProxy> ProxyList { get; set; }

        #endregion

        #region 客户端方法

        /// <summary>
        /// 启动服务
        /// <exception cref="InvalidOperationException">BindAddress未设置会引发此异常</exception>
        /// </summary>
        /// <returns></returns>
        bool Start();

        /// <summary>
        /// 停止服务
        /// </summary>
        /// <returns></returns>
        bool Stop();

        /// <summary>
        /// 连接到远程服务器
        /// </summary>
        /// <param name="address">远程服务器地址</param>
        /// <param name="port">远程服务器端口</param>
        /// <returns></returns>
        bool Connect(string address, ushort port);

        /// <summary>
        /// 连接到远程服务器
        /// </summary>
        /// <param name="address">远程服务器地址</param>
        /// <param name="port">远程服务器端口</param>
        /// <param name="connId">连接id</param>
        /// <returns></returns>
        bool Connect(string address, ushort port, out IntPtr connId);

        /// <summary>
        /// 连接到远程服务器并附带附加数据, 另可附带本地地址及端口, 默认为空不带
        /// </summary>
        /// <param name="address">远程服务器地址</param>
        /// <param name="port">远程服务器端口</param>
        /// <param name="extra">附加数据, 在回调事件中使用GetConnectionExtra()获取
        /// <para>附加托管对象时候可能需使用GCHandle固定托管对象地址, 使用方法参考微软官方文档:
        /// <see>
        ///     <cref>https://docs.microsoft.com/zh-cn/dotnet/api/system.runtime.interopservices.gchandle.addrofpinnedobject?view=netframework-4.8#System_Runtime_InteropServices_GCHandle_AddrOfPinnedObject</cref>
        /// </see>
        /// </para>
        /// </param>
        /// <param name="connId">连接id</param>
        /// <param name="localAddress">要绑定的本地地址</param>
        /// <param name="localPort">要绑定的本地端口</param>
        /// <returns></returns>
        bool Connect(string address, ushort port, IntPtr extra, out IntPtr connId, string localAddress = "", ushort localPort = 0);

        /// <summary>
        /// 连接到远程服务器并附带附加数据
        /// </summary>
        /// <param name="address"></param>
        /// <param name="port"></param>
        /// <param name="extra">附加数据, 在回调事件中使用GetConnectionExtra()获取
        /// <para>附加托管对象时候可能需使用GCHandle固定托管对象地址, 使用方法参考微软官方文档:
        /// <see>
        ///     <cref>https://docs.microsoft.com/zh-cn/dotnet/api/system.runtime.interopservices.gchandle.addrofpinnedobject?view=netframework-4.8#System_Runtime_InteropServices_GCHandle_AddrOfPinnedObject</cref>
        /// </see>
        /// </para>
        /// </param>
        /// <returns></returns>
        bool Connect(string address, ushort port, IntPtr extra);

        /// <summary>
        /// 设置连接附加数据, 非托管版本, hp-socket自带方法；【使用此方法不支持异步连接超时时间，且不支持连接状态获取】；非特殊需求不要使用这个方法, 请直接使用 SetExtra();
        /// <para>附加托管对象时候可能需使用GCHandle固定托管对象地址, 使用方法参考微软官方文档:
        /// <see>
        ///     <cref>https://docs.microsoft.com/zh-cn/dotnet/api/system.runtime.interopservices.gchandle.addrofpinnedobject?view=netframework-4.8#System_Runtime_InteropServices_GCHandle_AddrOfPinnedObject</cref>
        /// </see>
        /// </para>
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="extra"></param>
        /// <returns></returns>
        bool SetConnectionExtra(IntPtr connId, IntPtr extra);

        /// <summary>
        /// 获取连接附加数据, 非托管版本, hp-socket自带方法；【使用此方法不支持异步连接超时时间，且不支持连接状态获取】 非特殊需求不要使用这个方法, 请直接使用 GetExtra();
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="extra"></param>
        /// <returns></returns>
        bool GetConnectionExtra(IntPtr connId, out IntPtr extra);

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="bytes"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        bool Send(IntPtr connId, byte[] bytes, int length);

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="bytes"></param>
        /// <param name="offset">针对bytes的偏移</param>
        /// <param name="length">发多大</param>
        /// <returns></returns>
        bool Send(IntPtr connId, byte[] bytes, int offset, int length);

        /// <summary>
        /// 发送多组数据
        /// 向指定连接发送多组数据
        /// TCP - 顺序发送所有数据包
        /// </summary>
        /// <param name="connId">连接 ID</param>
        /// <param name="buffers">发送缓冲区数组</param>
        /// <returns>true.成功,false.失败，可通过 SYSGetLastError() 获取 Windows 错误代码</returns>
        bool SendPackets(IntPtr connId, Wsabuf[] buffers);

        /// <summary>
        /// 断开某个的连接
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="force">是否强制断开</param>
        /// <returns></returns>
        bool Disconnect(IntPtr connId, bool force = true);

        /// <summary>
        /// 断开超过指定时间的连接
        /// </summary>
        /// <param name="period">毫秒</param>
        /// <param name="force">强制</param>
        /// <returns></returns>
        bool DisconnectLongConnections(uint period, bool force = true);

        /// <summary>
        /// 暂停接收
        /// </summary>
        /// <param name="connId"></param>
        /// <returns></returns>
        bool PauseReceive(IntPtr connId);

        /// <summary>
        /// 唤醒接收
        /// </summary>
        /// <param name="connId"></param>
        /// <returns></returns>
        bool ResumeReceive(IntPtr connId);

        /// <summary>
        /// 获取连接的接收状态
        /// </summary>
        /// <param name="connId"></param>
        /// <returns></returns>
        ReceiveState GetReceiveState(IntPtr connId);

        /// <summary>
        /// 断开超过指定时长的静默连接
        /// </summary>
        /// <param name="period">毫秒</param>
        /// <param name="force">强制</param>
        /// <returns></returns>
        bool DisconnectSilenceConnections(uint period, bool force = true);

        /// <summary>
        /// 获取某个连接的本地地址信息
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        bool GetLocalAddress(IntPtr connId, out string ip, out ushort port);

        /// <summary>
        /// 获取某个连接的远程地址信息
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        bool GetRemoteAddress(IntPtr connId, out string ip, out ushort port);

        /// <summary>
        /// 获取连接中未发出数据的长度
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        bool GetPendingDataLength(IntPtr connId, out int length);

        /// <summary>
        /// 获取指定连接的连接时长（毫秒）
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="period"></param>
        /// <returns></returns>
        bool GetConnectPeriod(IntPtr connId, out uint period);

        /// <summary>
        /// 获取某个连接静默时间（毫秒）
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="period"></param>
        /// <returns></returns>
        bool GetSilencePeriod(IntPtr connId, out uint period);

        /// <summary>
        /// 获取所有连接
        /// </summary>
        /// <returns></returns>
        List<IntPtr> GetAllConnectionIds();

        /// <summary>
        /// 获取某个连接的远程主机信息
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="address"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        bool GetRemoteHost(IntPtr connId, out string address, out ushort port);

        /// <summary>
        /// 检测是否有效连接
        /// </summary>
        /// <param name="connId"></param>
        /// <returns></returns>
        bool IsConnected(IntPtr connId);

        /// <summary>
        /// 设置附加数据
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        bool SetExtra(IntPtr connId, object obj);

        /// <summary>
        /// 获取附加数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connId"></param>
        /// <returns></returns>
        T GetExtra<T>(IntPtr connId);

        /// <summary>
        /// 删除附加数据
        /// </summary>
        /// <param name="connId"></param>
        /// <returns></returns>
        bool RemoveExtra(IntPtr connId);

        /// <summary>
        /// 获取连接状态
        /// </summary>
        /// <param name="connId"></param>
        /// <returns></returns>
        TcpConnectionState GetConnectionState(IntPtr connId);

        #endregion
    }
}
