using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace HPSocket.Sdk
{
    internal static class Sys
    {
        /// <summary>
        /// 获取 hp socket版本号
        /// </summary>
        /// <returns></returns>
        public static string GetVersion()
        {
            var ver = HP_GetHPSocketVersion();
            var major = ver >> 24;
            var minor = (ver << 8) >> 24;
            var revision = (ver << 16) >> 24;
            var build = (ver << 24) >> 24;

            return $"{major}.{minor}.{revision}_{build}";
        }

        /// <summary>
        /// 获取主机地址类型
        /// </summary>
        /// <param name="host"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool GetHostAddressType(string host, out IpAddressType type)
        {
            if (SYS_IsIPAddress(host, out type))
            {
                return true;
            }

            var reg = new Regex(@"^(?=^.{3,255}$)[a-zA-Z0-9][-a-zA-Z0-9]{0,62}(\.[a-zA-Z0-9][-a-zA-Z0-9]{0,62})+$", RegexOptions.IgnoreCase);
            if (reg.IsMatch(host))
            {
                type = IpAddressType.Domain;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 获取 HPSocket 版本号（4 个字节分别为：主版本号，子版本号，修正版本号，构建编号）
        /// </summary>
        /// <returns></returns>
        [DllImport(HpSocketLibrary.DllName)]
        private static extern uint HP_GetHPSocketVersion();

        /// <summary>
        /// 获取错误描述文本
        /// </summary>
        /// <param name="enCode"></param>
        /// <returns></returns>
        [DllImport(HpSocketLibrary.DllName)]
        public static extern IntPtr HP_GetSocketErrorDesc(SocketError enCode);

        /// <summary>
        /// 调用系统的 GetLastWin32Error() 方法获取系统错误代码
        /// </summary>
        /// <returns></returns>
        public static int GetLastError()
        {
            return Marshal.GetLastWin32Error();
        }

        //
        // /// <summary>
        // /// 调用系统的 ::GetLastError() 方法获取通信错误代码
        // /// </summary>
        // /// <returns></returns>
        // [DllImport(HpSocketLibrary.DllName)]
        // public static extern int SYS_GetLastError();

        /// <summary>
        /// 调用系统的 GetLastWin32Error() 方法获取系统错误代码
        /// </summary>
        /// <returns></returns>
        public static int SYS_GetLastError() => GetLastError();

        /// <summary>
        /// 调用系统的 ::WSAGetLastError() 方法获取通信错误代码
        /// </summary>
        /// <returns></returns>
        [DllImport(HpSocketLibrary.DllName)]
        private static extern int SYS_WSAGetLastError();

        /// <summary>
        /// 调用系统的 ::WSAGetLastError() 方法获取通信错误代码
        /// </summary>
        /// <returns></returns>
        public static int WsaGetLastError() => SYS_WSAGetLastError();

        /// <summary>
        /// 调用系统的 setsockopt()
        /// </summary>
        /// <param name="sock"></param>
        /// <param name="level"></param>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        [DllImport(HpSocketLibrary.DllName, SetLastError = true)]
        private static extern int SYS_SetSocketOption(IntPtr sock, int level, int name, IntPtr val, int len);

        /// <summary>
        /// 调用系统的 setsockopt()
        /// </summary>
        /// <param name="sock"></param>
        /// <param name="level"></param>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static int SetSocketOption(IntPtr sock, int level, int name, IntPtr val, int len)
        {
            return SYS_SetSocketOption(sock, level, name, val, len);
        }

        /// <summary>
        /// 调用系统的 getsockopt()
        /// </summary>
        /// <param name="sock"></param>
        /// <param name="level"></param>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        [DllImport(HpSocketLibrary.DllName, SetLastError = true)]
        private static extern int SYS_GetSocketOption(IntPtr sock, int level, int name, IntPtr val, ref int len);

        /// <summary>
        /// 调用系统的 getsockopt()
        /// </summary>
        /// <param name="sock"></param>
        /// <param name="level"></param>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static int GetSocketOption(IntPtr sock, int level, int name, IntPtr val, ref int len)
        {
            return SYS_GetSocketOption(sock, level, name, val, ref len);
        }

        /// <summary>
        /// 调用系统的 ioctlsocket()
        /// </summary>
        /// <param name="sock"></param>
        /// <param name="cmd"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        [DllImport(HpSocketLibrary.DllName, SetLastError = true)]
        public static extern int SYS_IoctlSocket(IntPtr sock, long cmd, IntPtr arg);

        /// <summary>
        /// 调用系统的 ::WSAIoctl()
        /// </summary>
        /// <param name="sock"></param>
        /// <param name="dwIoControlCode"></param>
        /// <param name="lpvInBuffer"></param>
        /// <param name="cbInBuffer"></param>
        /// <param name="lpvOutBuffer"></param>
        /// <param name="cbOutBuffer"></param>
        /// <param name="lpcbBytesReturned"></param>
        /// <returns></returns>
        [DllImport(HpSocketLibrary.DllName, SetLastError = true)]
        public static extern int SYS_WSAIoctl(IntPtr sock, uint dwIoControlCode, IntPtr lpvInBuffer, uint cbInBuffer,
                                              IntPtr lpvOutBuffer, uint cbOutBuffer, uint lpcbBytesReturned);

        /// <summary>
        /// 设置 socket 选项：IPPROTO_TCP -> TCP_NODELAY
        /// </summary>
        /// <param name="sock"></param>
        /// <param name="bNoDelay"></param>
        /// <returns></returns>
        [DllImport(HpSocketLibrary.DllName)]
        public static extern int SYS_SSO_NoDelay(IntPtr sock, bool bNoDelay);


        /// <summary>
        /// 设置 socket 选项：SOL_SOCKET -> SO_DONTLINGER
        /// </summary>
        /// <param name="sock"></param>
        /// <param name="bDont"></param>
        /// <returns></returns>
        [DllImport(HpSocketLibrary.DllName)]
        public static extern int SYS_SSO_DontLinger(IntPtr sock, bool bDont);

        /// <summary>
        /// 设置 socket 选项：SOL_SOCKET -> SO_LINGER
        /// </summary>
        /// <param name="sock"></param>
        /// <param name="onoff"></param>
        /// <param name="linger"></param>
        /// <returns></returns>
        [DllImport(HpSocketLibrary.DllName)]
        public static extern int SYS_SSO_Linger(IntPtr sock, ushort onoff, ushort linger);


        /// <summary>
        /// 设置 socket 选项：SOL_SOCKET -> SO_RCVBUF
        /// </summary>
        /// <param name="sock"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        [DllImport(HpSocketLibrary.DllName)]
        public static extern int SYS_SSO_RecvBuffSize(IntPtr sock, int length);

        /// <summary>
        /// 设置 socket 选项：SOL_SOCKET -> SO_SNDBUF
        /// </summary>
        /// <param name="sock"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        [DllImport(HpSocketLibrary.DllName)]
        public static extern int SYS_SSO_SendBuffSize(IntPtr sock, int length);

        /// <summary>
        /// 设置 socket 选项：SOL_SOCKET -> SO_RCVTIMEO
        /// </summary>
        /// <param name="sock"></param>
        /// <param name="ms"></param>
        /// <returns></returns>
        [DllImport(HpSocketLibrary.DllName)]
        public static extern int SYS_SSO_RecvTimeOut(IntPtr sock, int ms);

        /// <summary>
        /// 设置 socket 选项：SOL_SOCKET -> SO_SNDTIMEO
        /// </summary>
        /// <param name="sock"></param>
        /// <param name="ms"></param>
        /// <returns></returns>
        [DllImport(HpSocketLibrary.DllName)]
        public static extern int SYS_SSO_SendTimeOut(IntPtr sock, int ms);

        /// <summary>
        /// 设置 socket 选项：SOL_SOCKET -> SO_EXCLUSIVEADDRUSE
        /// </summary>
        /// <param name="sock"></param>
        /// <param name="bExclusive"></param>
        /// <returns></returns>
        [DllImport(HpSocketLibrary.DllName)]
        public static extern int SYS_SSO_ExclusiveAddressUse(IntPtr sock, bool bExclusive);

        /// <summary>
        /// 设置 socket 选项：SOL_SOCKET -> SO_EXCLUSIVEADDRUSE / SO_REUSEADDR
        /// </summary>
        /// <param name="sock"></param>
        /// <param name="opt"></param>
        /// <returns></returns>
        [DllImport(HpSocketLibrary.DllName)]
        public static extern int SYS_SSO_ReuseAddress(IntPtr sock, ReuseAddressPolicy opt);

        /// <summary>
        /// 获取 SOCKET 本地地址信息
        /// </summary>
        /// <param name="pSocket"></param>
        /// <param name="lpszAddress"></param>
        /// <param name="piAddressLen">传入传出值,大小最好在222.222.222.222的长度以上</param>
        /// <param name="pusPort"></param>
        /// <returns></returns>
        [DllImport(HpSocketLibrary.DllName)]
        public static extern bool SYS_GetSocketLocalAddress(IntPtr pSocket, [MarshalAs(UnmanagedType.LPStr)] StringBuilder lpszAddress, ref int piAddressLen, ref ushort pusPort);

        /// <summary>
        /// 获取 SOCKET 远程地址信息
        /// </summary>
        /// <param name="pSocket"></param>
        /// <param name="lpszAddress"></param>
        /// <param name="piAddressLen">传入传出值,大小最好在222.222.222.222的长度以上</param>
        /// <param name="pusPort"></param>
        /// <returns></returns>
        [DllImport(HpSocketLibrary.DllName)]
        public static extern bool SYS_GetSocketRemoteAddress(IntPtr pSocket, [MarshalAs(UnmanagedType.LPStr)] StringBuilder lpszAddress, ref int piAddressLen, ref ushort pusPort);

        /// <summary>
        /// 枚举主机 IP 地址
        /// 不要用,未测试
        /// 不要用,未测试
        /// 不要用,未测试
        /// </summary>
        /// <param name="lpszHost"></param>
        /// <param name="enType"></param>
        /// <param name="lpppIpAddr"></param>
        /// <param name="piIpAddrCount"></param>
        /// <returns></returns>
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi)]
        public static extern bool SYS_EnumHostIPAddresses(string lpszHost, IpAddressType enType, ref IntPtr lpppIpAddr, ref int piIpAddrCount);

        /// <summary>
        /// 释放 HP_LPTIPAddr
        /// </summary>
        /// <param name="lppIpAddr"></param>
        /// <returns></returns>
        [DllImport(HpSocketLibrary.DllName)]
        public static extern bool SYS_FreeHostIPAddresses(IntPtr lppIpAddr);

        /// <summary>
        /// 检查字符串是否符合 IP 地址格式
        /// </summary>
        /// <param name="lpszAddress"></param>
        /// <param name="penType"></param>
        /// <returns></returns>
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi)]
        public static extern bool SYS_IsIPAddress(string lpszAddress, out IpAddressType penType);

        /// <summary>
        /// 通过主机名获取 IP 地址
        /// </summary>
        /// <param name="lpszHost"></param>
        /// <param name="lpszIp"></param>
        /// <param name="piIpLength"></param>
        /// <param name="penType"></param>
        /// <returns></returns>
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi)]
        public static extern bool SYS_GetIPAddress(string lpszHost, [MarshalAs(UnmanagedType.LPStr)] StringBuilder lpszIp, ref int piIpLength, ref IpAddressType penType);


        /// <summary>
        /// 64 位网络字节序转主机字节序
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [DllImport(HpSocketLibrary.DllName)]
        public static extern ulong SYS_NToH64(ulong value);

        /// <summary>
        /// 64 位主机字节序转网络字节序
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [DllImport(HpSocketLibrary.DllName)]
        public static extern ulong SYS_HToN64(ulong value);

        /// <summary>
        /// 短整型高低字节交换
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [DllImport(HpSocketLibrary.DllName)]
        public static extern ushort SYS_SwapEndian16(ushort value);

        /// <summary>
        /// 长整型高低字节交换
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [DllImport(HpSocketLibrary.DllName)]
        public static extern uint SYS_SwapEndian32(uint value);

        /// <summary>
        /// 检查是否小端字节序
        /// </summary>
        /// <returns></returns>
        [DllImport(HpSocketLibrary.DllName)]
        public static extern bool SYS_IsLittleEndian();

        /// <summary>
        /// 分配内存
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [DllImport(HpSocketLibrary.DllName)]
        public static extern IntPtr SYS_Malloc(int value);

        /// <summary>
        /// 重新分配内存
        /// </summary>
        /// <param name="p"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [DllImport(HpSocketLibrary.DllName)]
        public static extern IntPtr SYS_Realloc(IntPtr p, int value);

        /// <summary>
        /// 释放内存
        /// </summary>
        /// <param name="p"></param>
        [DllImport(HpSocketLibrary.DllName)]
        public static extern void SYS_Free(IntPtr p);


        /// <summary>
        /// Brotli 压缩
        /// </summary>
        /// <param name="src"></param>
        /// <param name="srcLen"></param>
        /// <param name="dest"></param>
        /// <param name="destLen"></param>
        /// <returns>0.成功，-3.输入数据不正确，-5.输出缓冲区不足</returns>
        [DllImport(HpSocketLibrary.DllName)]
        public static extern int SYS_BrotliCompress(byte[] src, int srcLen, ref IntPtr dest, ref uint destLen);

        /// <summary>
        /// Brotli 高级压缩
        /// </summary>
        /// <param name="lpszSrc"></param>
        /// <param name="srcLen"></param>
        /// <param name="dest"></param>
        /// <param name="destLen"></param>
        /// <param name="quality"></param>
        /// <param name="window"></param>
        /// <param name="mode"></param>
        /// <returns>0.成功，-3.输入数据不正确，-5.输出缓冲区不足</returns>
        [DllImport(HpSocketLibrary.DllName)]
        public static extern int SYS_BrotliCompressEx(IntPtr lpszSrc, uint srcLen, ref IntPtr dest, ref uint destLen, int quality, int window, int mode);
  
        /// <summary>
        /// Brotli 解压
        /// </summary>
        /// <param name="lpszSrc"></param>
        /// <param name="srcLen"></param>
        /// <param name="dest"></param>
        /// <param name="destLen"></param>
        /// <returns>0.成功，-3.输入数据不正确，-5.输出缓冲区不足</returns>
        [DllImport(HpSocketLibrary.DllName)]
        public static extern int SYS_BrotliUncompress(IntPtr lpszSrc, uint srcLen, ref IntPtr dest, ref uint destLen);

        /// <summary>
        /// Brotli 推测压缩结果长度
        /// </summary>
        /// <param name="srcLen"></param>
        /// <returns></returns>
        [DllImport(HpSocketLibrary.DllName)]
        public static extern uint SYS_BrotliGuessCompressBound(uint srcLen);
    }
}
