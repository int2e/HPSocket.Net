using System;
using System.Runtime.InteropServices;
using System.Text;

namespace HPSocket.Sdk
{
    internal static class Client
    {

        /**********************************************************************************/
        /***************************** Client 回调函数设置方法 *****************************/

        [DllImport(HpSocketLibrary.DllName)]
        public static extern void HP_Set_FN_Client_OnPrepareConnect(IntPtr pListener, OnPrepareConnect fn);
        [DllImport(HpSocketLibrary.DllName)]
        public static extern void HP_Set_FN_Client_OnConnect(IntPtr pListener, OnConnect fn);
        [DllImport(HpSocketLibrary.DllName)]
        public static extern void HP_Set_FN_Client_OnHandShake(IntPtr pListener, OnHandShake fn);
        [DllImport(HpSocketLibrary.DllName)]
        public static extern void HP_Set_FN_Client_OnSend(IntPtr pListener, OnSend fn);
        [DllImport(HpSocketLibrary.DllName)]
        public static extern void HP_Set_FN_Client_OnReceive(IntPtr pListener, OnReceive fn);
        [DllImport(HpSocketLibrary.DllName)]
        public static extern void HP_Set_FN_Client_OnPullReceive(IntPtr pListener, OnPullReceive fn);
        [DllImport(HpSocketLibrary.DllName)]
        public static extern void HP_Set_FN_Client_OnClose(IntPtr pListener, OnClose fn);


        /******************************************************************************/
        /***************************** Client 组件操作方法 *****************************/

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi)]
        public static extern bool HP_Client_Start(IntPtr pClient, string pszRemoteAddress, ushort usPort, bool bAsyncConnect);


        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi)]
        public static extern bool HP_Client_StartWithBindAddress(IntPtr pClient, string lpszRemoteAddress, ushort usPort, bool bAsyncConnect, string lpszBindAddress);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi)]
        public static extern bool HP_Client_StartWithBindAddressAndLocalPort(IntPtr pClient, string lpszRemoteAddress, ushort usPort, bool bAsyncConnect, string lpszBindAddress, ushort usLocalPort);


        [DllImport(HpSocketLibrary.DllName)]
        public static extern bool HP_Client_Stop(IntPtr pClient);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_Client_Send(IntPtr pClient, IntPtr pBuffer, int length);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_Client_SendPart(IntPtr pClient, IntPtr pBuffer, int length, int iOffset);

        [DllImport(HpSocketLibrary.DllName, SetLastError = true)]
        public static extern bool HP_Client_SendPackets(IntPtr pClient, Wsabuf[] pBuffers, int iCount);


        [DllImport(HpSocketLibrary.DllName, SetLastError = true)]
        public static extern bool HP_Client_PauseReceive(IntPtr pClient, bool bPause);

        /******************************************************************************/
        /***************************** Client 属性访问方法 *****************************/

        [DllImport(HpSocketLibrary.DllName, SetLastError = true)]
        public static extern void HP_Client_SetReuseAddressPolicy(IntPtr pClient, ReuseAddressPolicy opt);

        [DllImport(HpSocketLibrary.DllName, SetLastError = true)]
        public static extern ReuseAddressPolicy HP_Client_GetReuseAddressPolicy(IntPtr pClient);

        [DllImport(HpSocketLibrary.DllName)]
        public static extern bool HP_Client_IsSecure(IntPtr pClient);

        [DllImport(HpSocketLibrary.DllName)]
        public static extern bool HP_Client_HasStarted(IntPtr pClient);

        [DllImport(HpSocketLibrary.DllName)]
        public static extern ServiceState HP_Client_GetState(IntPtr pClient);

        [DllImport(HpSocketLibrary.DllName)]
        public static extern SocketError HP_Client_GetLastError(IntPtr pClient);

        [DllImport(HpSocketLibrary.DllName)]
        public static extern IntPtr HP_Client_GetLastErrorDesc(IntPtr pClient);

        [DllImport(HpSocketLibrary.DllName)]
        public static extern IntPtr HP_Client_GetConnectionID(IntPtr pClient);

        [DllImport(HpSocketLibrary.DllName)]
        public static extern bool HP_Client_GetLocalAddress(IntPtr pClient, [MarshalAs(UnmanagedType.LPStr)] StringBuilder lpszAddress, ref int piAddressLen, ref ushort pusPort);


        [DllImport(HpSocketLibrary.DllName)]
        public static extern bool HP_Client_GetRemoteHost(IntPtr pClient, [MarshalAs(UnmanagedType.LPStr)] StringBuilder lpszHost, ref int piHostLen, ref ushort pusPort);


        [DllImport(HpSocketLibrary.DllName)]
        public static extern bool HP_Client_GetPendingDataLength(IntPtr pClient, ref int piPending);

        [DllImport(HpSocketLibrary.DllName)]
        public static extern bool HP_Client_IsPauseReceive(IntPtr pClient, ref int pbPaused);

        [DllImport(HpSocketLibrary.DllName)]
        public static extern bool HP_Client_IsConnected(IntPtr pClient);


        [DllImport(HpSocketLibrary.DllName)]
        public static extern void HP_Client_SetFreeBufferPoolSize(IntPtr pClient, uint dwFreeBufferPoolSize);

        [DllImport(HpSocketLibrary.DllName)]
        public static extern void HP_Client_SetFreeBufferPoolHold(IntPtr pClient, uint dwFreeBufferPoolHold);

        [DllImport(HpSocketLibrary.DllName)]
        public static extern uint HP_Client_GetFreeBufferPoolSize(IntPtr pClient);

        [DllImport(HpSocketLibrary.DllName)]
        public static extern uint HP_Client_GetFreeBufferPoolHold(IntPtr pClient);
    }
}
