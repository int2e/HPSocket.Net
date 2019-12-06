using System;
using System.Runtime.InteropServices;
using System.Text;

namespace HPSocket.Sdk
{
    internal static class Agent
    {
        /**********************************************************************************/
        /****************************** Agent 回调函数设置方法 *****************************/

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_Agent_OnPrepareConnect(IntPtr pListener, OnPrepareConnect fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_Agent_OnConnect(IntPtr pListener, OnConnect fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_Agent_OnHandShake(IntPtr pListener, OnHandShake fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_Agent_OnSend(IntPtr pListener, OnSend fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_Agent_OnReceive(IntPtr pListener, OnReceive fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_Agent_OnPullReceive(IntPtr pListener, OnPullReceive fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_Agent_OnClose(IntPtr pListener, OnClose fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_Agent_OnShutdown(IntPtr pListener, OnShutdown fn);

        /**************************************************************************/
        /***************************** Agent 操作方法 *****************************/

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_Agent_Start(IntPtr pAgent, string pszBindAddress, bool bAsyncConnect);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_Agent_Stop(IntPtr pAgent);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_Agent_Wait(IntPtr pAgent, int dwMilliseconds);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_Agent_Connect(IntPtr pAgent, string lpszRemoteAddress, ushort usPort, ref IntPtr pdwConnId);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_Agent_ConnectWithExtra(IntPtr pAgent, string lpszRemoteAddress, ushort usPort, ref IntPtr pdwConnId, IntPtr pExtra);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_Agent_ConnectWithLocalPort(IntPtr pAgent, string lpszRemoteAddress, ushort usPort, ref IntPtr pdwConnId, ushort usLocalPort);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_Agent_ConnectWithLocalAddress(IntPtr pAgent, string lpszRemoteAddress, ushort usPort, ref IntPtr pdwConnId, string lpszLocalAddress);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_Agent_ConnectWithExtraAndLocalAddressPort(IntPtr pAgent, string lpszRemoteAddress, ushort usPort, ref IntPtr pdwConnId, IntPtr pExtra, ushort usLocalPort, string lpszLocalAddress);


        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_Agent_Send(IntPtr pAgent, IntPtr connId, IntPtr pBuffer, int length);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_Agent_SendPart(IntPtr pAgent, IntPtr connId, IntPtr pBuffer, int length, int iOffset);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_Agent_SendPackets(IntPtr pAgent, IntPtr connId, Wsabuf[] pBuffers, int iCount);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_Agent_PauseReceive(IntPtr pAgent, IntPtr dwConnId, bool bPause);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_Agent_Disconnect(IntPtr pAgent, IntPtr connId, bool bForce);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_Agent_DisconnectLongConnections(IntPtr pAgent, uint dwPeriod, bool bForce);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_Agent_DisconnectSilenceConnections(IntPtr pAgent, uint dwPeriod, bool bForce);


        /******************************************************************************/
        /***************************** Agent 属性访问方法 *****************************/

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Agent_SetSendPolicy(IntPtr pAgent, SendPolicy enSendPolicy);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern SendPolicy HP_Agent_GetSendPolicy(IntPtr pAgent);


        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern OnSendSyncPolicy HP_Agent_GetOnSendSyncPolicy(IntPtr pAgent);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Agent_SetOnSendSyncPolicy(IntPtr pAgent, OnSendSyncPolicy syncPolicy);


        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_Agent_SetConnectionExtra(IntPtr pAgent, IntPtr connId, IntPtr pExtra);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_Agent_GetConnectionExtra(IntPtr pAgent, IntPtr connId, ref IntPtr pExtra);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_Agent_IsSecure(IntPtr pAgent);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_Agent_HasStarted(IntPtr pAgent);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern ServiceState HP_Agent_GetState(IntPtr pAgent);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_Agent_GetConnectionCount(IntPtr pAgent);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_Agent_GetAllConnectionIDs(IntPtr pAgent, IntPtr[] pIds, ref uint pdwCount);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_Agent_GetConnectPeriod(IntPtr pAgent, IntPtr connId, ref uint pdwPeriod);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_Agent_GetSilencePeriod(IntPtr pAgent, IntPtr connId, ref uint pdwPeriod);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_Agent_GetLocalAddress(IntPtr pAgent, IntPtr connId, [MarshalAs(UnmanagedType.LPStr)] StringBuilder lpszAddress, ref int piAddressLen, ref ushort pusPort);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_Agent_GetRemoteAddress(IntPtr pAgent, IntPtr connId, [MarshalAs(UnmanagedType.LPStr)] StringBuilder lpszAddress, ref int piAddressLen, ref ushort pusPort);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_Agent_GetRemoteHost(IntPtr pAgent, IntPtr dwConnId, [MarshalAs(UnmanagedType.LPStr)]StringBuilder lpszAddress, ref int piHostLen, ref ushort pusPort);


        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern SocketError HP_Agent_GetLastError(IntPtr pAgent);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr HP_Agent_GetLastErrorDesc(IntPtr pAgent);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_Agent_GetPendingDataLength(IntPtr pAgent, IntPtr connId, ref int piPending);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_Agent_IsPauseReceive(IntPtr pAgent, IntPtr dwConnId, ref int pbPaused);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_Agent_IsConnected(IntPtr pAgent, IntPtr dwConnId);


        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Agent_SetFreeSocketObjLockTime(IntPtr pAgent, uint dwFreeSocketObjLockTime);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Agent_SetFreeSocketObjPool(IntPtr pAgent, uint dwFreeSocketObjPool);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Agent_SetFreeBufferObjPool(IntPtr pAgent, uint dwFreeBufferObjPool);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Agent_SetFreeSocketObjHold(IntPtr pAgent, uint dwFreeSocketObjHold);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Agent_SetFreeBufferObjHold(IntPtr pAgent, uint dwFreeBufferObjHold);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Agent_SetMaxConnectionCount(IntPtr pAgent, uint dwMaxConnectionCount);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Agent_SetWorkerThreadCount(IntPtr pAgent, uint dwWorkerThreadCount);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Agent_SetMarkSilence(IntPtr pAgent, bool bMarkSilence);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_Agent_GetFreeSocketObjLockTime(IntPtr pAgent);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_Agent_GetFreeSocketObjPool(IntPtr pAgent);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_Agent_GetFreeBufferObjPool(IntPtr pAgent);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_Agent_GetFreeSocketObjHold(IntPtr pAgent);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_Agent_GetFreeBufferObjHold(IntPtr pAgent);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_Agent_GetMaxConnectionCount(IntPtr pAgent);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_Agent_GetWorkerThreadCount(IntPtr pAgent);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_Agent_IsMarkSilence(IntPtr pAgent);

        /**********************************************************************************/

    }
}
