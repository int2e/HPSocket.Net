using System;
using System.Runtime.InteropServices;
using System.Text;

namespace HPSocket.Sdk
{
    internal static class Server
    {
        /**********************************************************************************/
        /***************************** Server 回调函数设置方法 *****************************/

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_Server_OnPrepareListen(IntPtr pListener, OnPrepareListen fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_Server_OnAccept(IntPtr pListener, OnAccept fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_Server_OnHandShake(IntPtr pListener, OnHandShake fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_Server_OnSend(IntPtr pListener, OnSend fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_Server_OnReceive(IntPtr pListener, OnReceive fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_Server_OnPullReceive(IntPtr pListener, OnPullReceive fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_Server_OnClose(IntPtr pListener, OnClose fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_Server_OnShutdown(IntPtr pListener, OnShutdown fn);


        /**************************************************************************/
        /***************************** Server 操作方法 *****************************/

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_Server_Start(IntPtr pServer, string pszBindAddress, ushort usPort);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_Server_Stop(IntPtr pServer);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_Server_Wait(IntPtr pServer, int dwMilliseconds);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_Server_Send(IntPtr pServer, IntPtr connId, IntPtr pBuffer, int length);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_Server_SendPart(IntPtr pServer, IntPtr connId, IntPtr pBuffer, int length, int iOffset);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_Server_SendPackets(IntPtr pServer, IntPtr connId, Wsabuf[] pBuffers, int iCount);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_Server_PauseReceive(IntPtr pServer, IntPtr dwConnId, bool bPause);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_Server_Disconnect(IntPtr pServer, IntPtr connId, bool bForce);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_Server_DisconnectLongConnections(IntPtr pServer, uint dwPeriod, bool bForce);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_Server_DisconnectSilenceConnections(IntPtr pServer, uint dwPeriod, bool bForce);

        /******************************************************************************/
        /***************************** Server 属性访问方法 *****************************/

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Server_SetSendPolicy(IntPtr pServer, SendPolicy enSendPolicy);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern SendPolicy HP_Server_GetSendPolicy(IntPtr pServer);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_Server_SetConnectionExtra(IntPtr pServer, IntPtr connId, IntPtr pExtra);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_Server_GetConnectionExtra(IntPtr pServer, IntPtr connId, ref IntPtr pExtra);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_Server_IsSecure(IntPtr pServer);


        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_Server_HasStarted(IntPtr pServer);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern ServiceState HP_Server_GetState(IntPtr pServer);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern SocketError HP_Server_GetLastError(IntPtr pServer);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr HP_Server_GetLastErrorDesc(IntPtr pServer);


        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_Server_GetPendingDataLength(IntPtr pServer, IntPtr connId, ref int piPending);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_Server_IsPauseReceive(IntPtr pServer, IntPtr dwConnId, ref int pbPaused);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_Server_IsConnected(IntPtr pServer, IntPtr dwConnId);


        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_Server_GetConnectionCount(IntPtr pServer);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_Server_GetAllConnectionIDs(IntPtr pServer, IntPtr[] pIds, ref uint pdwCount);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_Server_GetConnectPeriod(IntPtr pServer, IntPtr connId, ref uint pdwPeriod);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_Server_GetSilencePeriod(IntPtr pServer, IntPtr connId, ref uint pdwPeriod);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_Server_GetListenAddress(IntPtr pServer, [MarshalAs(UnmanagedType.LPStr)] StringBuilder lpszAddress, ref int piAddressLen, ref ushort pusPort);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_Server_GetLocalAddress(IntPtr pServer, IntPtr connId, [MarshalAs(UnmanagedType.LPStr)] StringBuilder lpszAddress, ref int piAddressLen, ref ushort pusPort);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_Server_GetRemoteAddress(IntPtr pServer, IntPtr connId, [MarshalAs(UnmanagedType.LPStr)] StringBuilder lpszAddress, ref int piAddressLen, ref ushort pusPort);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Server_SetOnSendSyncPolicy(IntPtr pServer, OnSendSyncPolicy syncPolicy);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern OnSendSyncPolicy HP_Server_GetOnSendSyncPolicy(IntPtr pServer);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern ReuseAddressPolicy HP_Server_GetReuseAddressPolicy(IntPtr pServer);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Server_SetReuseAddressPolicy(IntPtr pServer, ReuseAddressPolicy enReusePolicy);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Server_SetFreeSocketObjLockTime(IntPtr pServer, uint dwFreeSocketObjLockTime);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Server_SetFreeSocketObjPool(IntPtr pServer, uint dwFreeSocketObjPool);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Server_SetFreeBufferObjPool(IntPtr pServer, uint dwFreeBufferObjPool);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Server_SetFreeSocketObjHold(IntPtr pServer, uint dwFreeSocketObjHold);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Server_SetFreeBufferObjHold(IntPtr pServer, uint dwFreeBufferObjHold);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Server_SetMaxConnectionCount(IntPtr pServer, uint dwMaxConnectionCount);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Server_SetWorkerThreadCount(IntPtr pServer, uint dwWorkerThreadCount);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Server_SetMarkSilence(IntPtr pServer, bool bMarkSilence);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_Server_GetFreeSocketObjLockTime(IntPtr pServer);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_Server_GetFreeSocketObjPool(IntPtr pServer);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_Server_GetFreeBufferObjPool(IntPtr pServer);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_Server_GetFreeSocketObjHold(IntPtr pServer);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_Server_GetFreeBufferObjHold(IntPtr pServer);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_Server_GetMaxConnectionCount(IntPtr pServer);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_Server_GetWorkerThreadCount(IntPtr pServer);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_Server_IsMarkSilence(IntPtr pServer);
    }
}
