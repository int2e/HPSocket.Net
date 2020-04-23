using System;
using System.Collections.Generic;

namespace HPSocket
{
    /// <summary>
    /// tcp 端口转发
    /// </summary>
    public interface ITcpPortForwarding : ISocket
    {
        #region 事件

        /// <summary>
        /// server 连接进入
        /// </summary>
        event ServerAcceptEventHandler OnServerAccept;
        /// <summary>
        /// server 数据到达
        /// </summary>
        event ServerReceiveEventHandler OnServerReceive;
        /// <summary>
        /// server 连接离开
        /// </summary>
        event ServerCloseEventHandler OnServerClose;

        /// <summary>
        /// agent 连接成功(使用代理连接时,代理连接成功才会进入)
        /// </summary>
        event AgentConnectEventHandler OnAgentConnect;
        /// <summary>
        /// agent 数据到达
        /// </summary>
        event AgentReceiveEventHandler OnAgentReceive;
        /// <summary>
        /// agent 连接断开
        /// </summary>
        event AgentCloseEventHandler OnAgentClose;

        #endregion

        #region 属性

        /// <summary>
        /// 内部server对象
        /// </summary>
        ITcpServer Server { get; }

        /// <summary>
        /// 内部agent对象
        /// </summary>
        ITcpAgent Agent { get; }

        /// <summary>
        /// 本地绑定地址, 默认0.0.0.0
        /// </summary>
        string LocalBindAddress { get; set; }
        /// <summary>
        /// 本地绑定端口
        /// </summary>
        ushort LocalBindPort { get; set; }
        /// <summary>
        /// 目标服务器地址
        /// </summary>
        string TargetAddress { get; set; }
        /// <summary>
        /// 目标服务器端口
        /// </summary>
        ushort TargetPort { get; set; }

        /// <summary>
        /// 转发服务器和客户端工作线程数,  通常设置为 2 * CPU + 2
        /// <para>注意: 当前值是每个组件的工作线程数, 比如设置为5, 服务器和客户端各5个, 一共10个工作线程</para>
        /// <para></para>
        /// </summary>
        uint EachWorkThreadCount { get; set; }

        /// <summary>
        /// 最大连接数, 默认10000
        /// </summary>
        uint MaxConnectionCount { get; set; }

        /// <summary>
        /// 连接目标服务器的超时时间, 默认0, 不启用, 使用系统默认时间
        /// </summary>
        int ConnectionTimeout { get; set; }

        /// <summary>
        /// 代理列表
        /// <para>转发访问目标服务器时,可以使用代理</para>
        /// </summary>
        List<IProxy> ProxyList { get; set; }

        /// <summary>
        /// 错误码
        /// </summary>
        SocketError ErrorCode { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        string ErrorMessage { get; set; }

        #endregion

        #region 方法

        /// <summary>
        /// 开启服务
        /// </summary>
        bool Start();

        /// <summary>
        /// 停止服务
        /// </summary>
        bool Stop();


        /// <summary>
        /// 根据Agent组件的连接id设置附加数据
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        bool SetExtraByAgentConnId (IntPtr connId, object obj);

        /// <summary>
        /// 根据Server组件的连接id设置附加数据
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        bool SetExtraByServerConnId(IntPtr connId, object obj);

        /// <summary>
        /// 根据Agent组件的连接id获取附加数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connId"></param>
        /// <returns></returns>
        T GetExtraByAgentConnId<T>(IntPtr connId);

        /// <summary>
        /// 根据Server组件的连接id获取附加数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connId"></param>
        /// <returns></returns>
        T GetExtraByServerConnId<T>(IntPtr connId);

        #endregion
    }
}
