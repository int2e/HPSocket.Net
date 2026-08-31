using System;
using System.Runtime.InteropServices;

namespace HPSocket.Sdk
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct IpAddressEntry
    {
        public IpAddressType Type;
        public IntPtr Address;
    }
}
