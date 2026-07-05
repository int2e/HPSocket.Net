using System;

using HPSocket.Proxy;
using HPSocket.Tcp;

namespace HPSocket
{
    /// <summary>
    /// 非托管内存附加数据
    /// </summary>
    internal class NativeExtra
    {
        /// <summary>
        /// 连接状态(int 后备字段,仅供 Interlocked.CompareExchange 使用; 业务代码请用 <see cref="TcpConnectionState"/> 属性)
        /// </summary>
        internal int TcpConnectionStateRef;

        /// <summary>
        /// 连接状态
        /// </summary>
        public TcpConnectionState TcpConnectionState
        {
            get => (TcpConnectionState)TcpConnectionStateRef;
            set => TcpConnectionStateRef = (int)value;
        }

        /// <summary>
        /// 代理连接状态
        /// </summary>
        public ProxyConnectionState ProxyConnectionState { get; set; }
        
        /// <summary>
        /// 代理
        /// </summary>
        public IProxy Proxy { get; set; }

        /// <summary>
        /// 用户的附加数据
        /// </summary>
        public IntPtr UserExtra { get; set; }
    }
}
