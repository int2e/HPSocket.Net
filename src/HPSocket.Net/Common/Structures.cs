using System;
using System.Runtime.InteropServices;

namespace HPSocket
{
    /// <summary>
    /// wsabuf
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Wsabuf
    {
        public int Length;
        public IntPtr Buffer;
    }

    /// <summary>
    /// Name/Value 结构体
    /// 字符串名值对结构体
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct NameValue
    {
        [MarshalAs(UnmanagedType.LPStr)]
        public string Name;

        [MarshalAs(UnmanagedType.LPStr)]
        public string Value;
    }

    /// <summary>
    /// Name/Value 结构体
    /// 字符串名值对结构体
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct NameValueIntPtr
    {
        [MarshalAs(UnmanagedType.LPStr)]
        public IntPtr Name;
        [MarshalAs(UnmanagedType.LPStr)]
        public IntPtr Value;
    }


}
