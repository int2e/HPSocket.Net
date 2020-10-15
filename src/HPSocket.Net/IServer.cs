using System;
using System.Collections.Generic;
#if !NET20 && !NET30 && !NET35
using System.Collections.Concurrent;
#endif

namespace HPSocket
{
    /// <summary>
    /// server 基础接口
    /// </summary>
    public interface IServer : ISocket
    {
        #region 基础属性

        /// <summary>
        /// 要绑定的服务器地址
        /// </summary>
        string Address { get; set; }

        /// <summary>
        /// 要绑定的服务器端口
        /// </summary>
        ushort Port { get; set; }

        #endregion

        #region 服务器事件

        /// <summary>
        /// 连接到达事件
        /// </summary>
        event ServerAcceptEventHandler OnAccept;

        /// <summary>
        /// 数据包发送事件
        /// </summary>
        event ServerSendEventHandler OnSend;
        /// <summary>
        /// 准备监听了事件
        /// </summary>
        event ServerPrepareListenEventHandler OnPrepareListen;

        /// <summary>
        /// 数据到达事件
        /// </summary>
        event ServerReceiveEventHandler OnReceive;

        /// <summary>
        /// 连接关闭事件
        /// </summary>
        event ServerCloseEventHandler OnClose;

        /// <summary>
        /// 服务器关闭事件
        /// </summary>
        event ServerShutdownEventHandler OnShutdown;

        /// <summary>
        /// 握手成功事件
        /// </summary>
        event ServerHandShakeEventHandler OnHandShake;

        #endregion

        #region 服务器属性

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

        #endregion

        #region 服务器方法

        /// <summary>
        /// 启动服务
        /// </summary>
        /// <returns></returns>
        bool Start();

        /// <summary>
        /// 停止服务
        /// </summary>
        /// <returns></returns>
        bool Stop();

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
        /// 断开与某个客户的连接
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
        /// 获取监听socket的地址信息
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        bool GetListenAddress(out string ip, out ushort port);

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
        /// 是否有效连接
        /// </summary>
        /// <param name="connId"></param>
        /// <returns></returns>
        bool IsConnected(IntPtr connId);

        /// <summary>
        /// 设置连接附加数据, 非托管版本, hp-socket自带方法；非特殊需求不要使用这个方法, 请直接使用 SetExtra();
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="extra"></param>
        /// <returns></returns>
        bool NativeSetConnectionExtra(IntPtr connId, IntPtr extra);

        /// <summary>
        /// 获取连接附加数据, 非托管版本, hp-socket自带方法；非特殊需求不要使用这个方法, 请直接使用 GetExtra();
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="extra"></param>
        /// <returns></returns>
        bool NativeGetConnectionExtra(IntPtr connId, out IntPtr extra);

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

#if NET20 || NET30 || NET35
        /// <summary>
        /// 获取所有附加数据
        /// </summary>
        /// <returns></returns>
        Dictionary<IntPtr, object> GetAllExtra();
#else
        /// <summary>
        /// 获取所有附加数据
        /// </summary>
        /// <returns></returns>
        ConcurrentDictionary<IntPtr, object> GetAllExtra();
		#endif

        /// <summary>
        /// 删除附加数据
        /// </summary>
        /// <param name="connId"></param>
        /// <returns></returns>
        bool RemoveExtra(IntPtr connId);

        #endregion
    }
}
