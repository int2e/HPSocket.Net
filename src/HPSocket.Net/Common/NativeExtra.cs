using System;
using System.Runtime.InteropServices;
using HPSocket.Proxy;

namespace HPSocket
{
    /// <summary>
    /// 非托管内存附加数据
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeExtra
    {
        /// <summary>
        /// 连接状态
        /// </summary>
        [MarshalAs(UnmanagedType.I4)] public Tcp.TcpConnectionState TcpConnectionState;

        /// <summary>
        /// 代理连接状态
        /// </summary>
        [MarshalAs(UnmanagedType.I4)] public ProxyConnectionState ProxyConnectionState;

        /// <summary>
        /// 代理连接标记
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)] public string ProxyConnectionFlag;

        /// <summary>
        /// 用户的附加数据
        /// </summary>
        [MarshalAs(UnmanagedType.SysUInt)] public IntPtr UserExtra;
    }
}
