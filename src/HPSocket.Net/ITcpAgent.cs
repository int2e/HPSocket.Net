using System;
using System.Collections.Generic;
using HPSocket.Adapter;

namespace HPSocket
{
    /// <summary>
    /// tcp agent
    /// </summary>
    /// <typeparam name="TRequestBodyType">包体解析对象类型</typeparam>
    public interface ITcpAgent<TRequestBodyType> : ITcpAgent
    {
        [Obsolete("adapter组件无需添加OnReceive事件, 请添加OnParseRequestBody事件", true)]
        new event AgentReceiveEventHandler OnReceive;

        /// <summary>
        /// 解析请求包体对象事件
        /// </summary>
        event ParseRequestBody<ITcpAgent, TRequestBodyType> OnParseRequestBody;

        /// <summary>
        /// 数据接收适配器
        /// </summary>
        DataReceiveAdapter<TRequestBodyType> DataReceiveAdapter { get; set; }
    }


    /// <summary>
    /// tcp agent
    /// </summary>
    public interface ITcpAgent : IAgent
    {
        #region 客户端属性

        /// <summary>
        /// 读取或设置通信数据缓冲区大小（根据平均通信数据包大小调整设置，通常设置为 1024 的倍数）
        /// </summary>
        uint SocketBufferSize { get; set; }

        /// <summary>
        /// 读取或设置心跳包间隔（毫秒，0 则不发送心跳包）
        /// </summary>
        uint KeepAliveTime { get; set; }

        /// <summary>
        /// 读取或设置心跳确认包检测间隔（毫秒，0 不发送心跳包，如果超过若干次 [默认：WinXP 5 次, Win7 10 次] 检测不到心跳确认包则认为已断线）
        /// </summary>
        uint KeepAliveInterval { get; set; }

        /// <summary>
        /// 获取或设置是否开启 nodelay 模式 (默认: false, 不开启)
        /// </summary>
        bool NoDelay { get; set; }

        #endregion

        #region 客户端方法

        /// <summary>
        /// 发送本地小文件
        /// <para>向指定连接发送 4096 KB 以下的小文件</para>
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="filePath">文件路径</param>
        /// <param name="head">头部附加数据</param>
        /// <param name="tail">尾部附加数据</param>
        /// <returns>true.成功,false.失败，可通过 SYSGetLastError() 获取 Windows 错误代码</returns>
        bool SendSmallFile(IntPtr connId, string filePath, ref Wsabuf head, ref Wsabuf tail);

        /// <summary>
        /// 发送本地小文件
        /// <para>向指定连接发送 4096 KB 以下的小文件</para>
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="filePath">文件路径</param>
        /// <param name="head">头部附加数据,可以为null</param>
        /// <param name="tail">尾部附加数据,可以为null</param>
        /// <returns>true.成功,false.失败，可通过 SYSGetLastError() 获取 Windows 错误代码</returns>
        bool SendSmallFile(IntPtr connId, string filePath, byte[] head, byte[] tail);

        #endregion
    }
}
