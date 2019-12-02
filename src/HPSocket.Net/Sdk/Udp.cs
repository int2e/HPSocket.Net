using System;
using System.Runtime.InteropServices;
using System.Text;
using HPSocket.Udp;

namespace HPSocket.Sdk
{
    internal class Udp
    {
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr Create_HP_UdpServer(IntPtr pListener);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr Create_HP_UdpClient(IntPtr pListener);


        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr Create_HP_UdpCast(IntPtr pListener);


        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void Destroy_HP_UdpServer(IntPtr pServer);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void Destroy_HP_UdpClient(IntPtr pClient);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void Destroy_HP_UdpCast(IntPtr pCast);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr Create_HP_UdpServerListener();

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr Create_HP_UdpClientListener();

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr Create_HP_UdpCastListener();



        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void Destroy_HP_UdpServerListener(IntPtr pListener);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void Destroy_HP_UdpClientListener(IntPtr pListener);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void Destroy_HP_UdpCastListener(IntPtr pListener);


        /**********************************************************************************/
        /***************************** UDP Server 属性访问方法 *****************************/

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_UdpServer_SetMaxDatagramSize(IntPtr pServer, uint dwMaxDatagramSize);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_UdpServer_GetMaxDatagramSize(IntPtr pServer);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_UdpServer_SetPostReceiveCount(IntPtr pServer, uint dwPostReceiveCount);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_UdpServer_GetPostReceiveCount(IntPtr pServer);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_UdpServer_SetDetectAttempts(IntPtr pServer, uint dwMaxDatagramSize);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_UdpServer_SetDetectInterval(IntPtr pServer, uint dwMaxDatagramSize);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_UdpServer_GetDetectAttempts(IntPtr pServer);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_UdpServer_GetDetectInterval(IntPtr pServer);


        /**********************************************************************************/
        /***************************** UDP Client 属性访问方法 *****************************/

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_UdpClient_SetMaxDatagramSize(IntPtr pClient, uint dwMaxDatagramSize);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_UdpClient_GetMaxDatagramSize(IntPtr pClient);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_UdpClient_SetDetectAttempts(IntPtr pClient, uint dwDetectAttempts);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_UdpClient_SetDetectInterval(IntPtr pClient, uint dwDetectInterval);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_UdpClient_GetDetectAttempts(IntPtr pClient);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_UdpClient_GetDetectInterval(IntPtr pClient);


        /**********************************************************************************/
        /****************************** UDP Cast 属性访问方法 ******************************/

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_UdpCast_SetMaxDatagramSize(IntPtr pCast, uint dwMaxDatagramSize);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_UdpCast_GetMaxDatagramSize(IntPtr pCast);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_UdpCast_GetRemoteAddress(IntPtr pCast, [MarshalAs(UnmanagedType.LPStr)] StringBuilder lpszAddress, ref int piAddressLen, ref ushort pusPort);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_UdpCast_SetCastMode(IntPtr pCast, CastMode enCastMode);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern CastMode HP_UdpCast_GetCastMode(IntPtr pCast);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_UdpCast_SetMultiCastTtl(IntPtr pCast, int iMcTtl);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern int HP_UdpCast_GetMultiCastTtl(IntPtr pCast);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_UdpCast_SetMultiCastLoop(IntPtr pCast, bool bMcLoop);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_UdpCast_IsMultiCastLoop(IntPtr pCast);

        /**********************************************************************************/
        /*************************** UDP ARQ  ***************************/

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr Create_HP_UdpArqServer(IntPtr pListener);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr Create_HP_UdpArqClient(IntPtr pListener);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void Destroy_HP_UdpArqServer(IntPtr pServer);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void Destroy_HP_UdpArqClient(IntPtr pClient);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr Create_HP_UdpArqServerListener();

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr Create_HP_UdpArqClientListener();

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void Destroy_HP_UdpArqServerListener(IntPtr pListener);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void Destroy_HP_UdpArqClientListener(IntPtr pListener);

        /**********************************************************************************/
        /*************************** UDP ARQ Server 属性访问方法 ***************************/


        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_UdpArqServer_SetNoDelay(IntPtr pServer, bool bNoDelay);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_UdpArqServer_SetTurnoffCongestCtrl(IntPtr pServer, bool bTurnOff);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_UdpArqServer_SetFlushInterval(IntPtr pServer, uint dwFlushInterval);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_UdpArqServer_SetResendByAcks(IntPtr pServer, uint dwResendByAcks);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_UdpArqServer_SetSendWndSize(IntPtr pServer, uint dwSendWndSize);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_UdpArqServer_SetRecvWndSize(IntPtr pServer, uint dwRecvWndSize);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_UdpArqServer_SetMinRto(IntPtr pServer, uint dwMinRto);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_UdpArqServer_SetMaxTransUnit(IntPtr pServer, uint dwMaxTransUnit);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_UdpArqServer_SetMaxMessageSize(IntPtr pServer, uint dwMaxMessageSize);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_UdpArqServer_SetHandShakeTimeout(IntPtr pServer, uint dwHandShakeTimeout);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_UdpArqServer_SetFastLimit(IntPtr pServer, uint dwFastLimit);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_UdpArqServer_IsNoDelay(IntPtr pServer);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_UdpArqServer_IsTurnoffCongestCtrl(IntPtr pServer);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_UdpArqServer_GetFlushInterval(IntPtr pServer);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_UdpArqServer_GetResendByAcks(IntPtr pServer);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_UdpArqServer_GetSendWndSize(IntPtr pServer);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_UdpArqServer_GetRecvWndSize(IntPtr pServer);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_UdpArqServer_GetMinRto(IntPtr pServer);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_UdpArqServer_GetMaxTransUnit(IntPtr pServer);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_UdpArqServer_GetMaxMessageSize(IntPtr pServer);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_UdpArqServer_GetHandShakeTimeout(IntPtr pServer);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_UdpArqServer_GetFastLimit(IntPtr pServer);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_UdpArqServer_GetWaitingSendMessageCount(IntPtr pServer, IntPtr dwConnId, ref int piCount);

        /**********************************************************************************/
        /*************************** UDP ARQ Client 属性访问方法 ***************************/

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_UdpArqClient_SetNoDelay(IntPtr pClient, bool bNoDelay);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_UdpArqClient_SetTurnoffCongestCtrl(IntPtr pClient, bool bTurnOff);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_UdpArqClient_SetFlushInterval(IntPtr pClient, uint dwFlushInterval);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_UdpArqClient_SetResendByAcks(IntPtr pClient, uint dwResendByAcks);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_UdpArqClient_SetSendWndSize(IntPtr pClient, uint dwSendWndSize);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_UdpArqClient_SetRecvWndSize(IntPtr pClient, uint dwRecvWndSize);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_UdpArqClient_SetMinRto(IntPtr pClient, uint dwMinRto);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_UdpArqClient_SetMaxTransUnit(IntPtr pClient, uint dwMaxTransUnit);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_UdpArqClient_SetMaxMessageSize(IntPtr pClient, uint dwMaxMessageSize);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_UdpArqClient_SetHandShakeTimeout(IntPtr pClient, uint dwHandShakeTimeout);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_UdpArqClient_SetFastLimit(IntPtr pClient, uint dwFastLimit);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_UdpArqClient_IsNoDelay(IntPtr pClient);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_UdpArqClient_IsTurnoffCongestCtrl(IntPtr pClient);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_UdpArqClient_GetFlushInterval(IntPtr pClient);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_UdpArqClient_GetResendByAcks(IntPtr pClient);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_UdpArqClient_GetSendWndSize(IntPtr pClient);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_UdpArqClient_GetRecvWndSize(IntPtr pClient);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_UdpArqClient_GetMinRto(IntPtr pClient);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_UdpArqClient_GetMaxTransUnit(IntPtr pClient);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_UdpArqClient_GetMaxMessageSize(IntPtr pClient);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_UdpArqClient_GetHandShakeTimeout(IntPtr pClient);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_UdpArqClient_GetFastLimit(IntPtr pClient);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_UdpArqClient_GetWaitingSendMessageCount(IntPtr pClient, ref int piCount);

        /**********************************************************************************/
        /****************************** UDP Node 操作方法 ******************************/
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr Create_HP_UdpNode(IntPtr pListener);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void Destroy_HP_UdpNode(IntPtr pNode);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr Create_HP_UdpNodeListener();
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void Destroy_HP_UdpNodeListener(IntPtr pListener);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_UdpNode_Start(IntPtr pNode, string lpszBindAddress, ushort usPort);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_UdpNode_StartWithCast(IntPtr pNode, string lpszBindAddress, ushort usPort, CastMode castMode, string lpszCastAddress);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_UdpNode_Stop(IntPtr pNode);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_UdpNode_Wait(IntPtr pNode, int dwMilliseconds);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_UdpNode_Send(IntPtr pNode, string lpszRemoteAddress, ushort usRemotePort, IntPtr pBuffer, int iLength);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_UdpNode_SendPart(IntPtr pNode, string lpszRemoteAddress, ushort usRemotePort, IntPtr pBuffer, int iLength, int iOffset);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_UdpNode_SendPackets(IntPtr pNode, string lpszRemoteAddress, ushort usRemotePort, Wsabuf[] pBuffers, int iCount);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_UdpNode_SendCast(IntPtr pNode, IntPtr pBuffer, int iLength);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_UdpNode_SendCastPart(IntPtr pNode, IntPtr pBuffer, int iLength, int iOffset);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_UdpNode_SendCastPackets(IntPtr pNode, Wsabuf[] pBuffers, int iCount);

        /**********************************************************************************/
        /****************************** UDP Node 属性访问方法 ******************************/
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_UdpNode_SetExtra(IntPtr pNode, IntPtr pExtra);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr HP_UdpNode_GetExtra(IntPtr pNode);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_UdpNode_HasStarted(IntPtr pNode);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern ServiceState HP_UdpNode_GetState(IntPtr pNode);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern SocketError HP_UdpNode_GetLastError(IntPtr pNode);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr HP_UdpNode_GetLastErrorDesc(IntPtr pNode);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_UdpNode_GetLocalAddress(IntPtr pNode, [MarshalAs(UnmanagedType.LPStr)] StringBuilder lpszAddress, ref int piAddressLen, ref ushort pusPort);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_UdpNode_GetCastAddress(IntPtr pNode, [MarshalAs(UnmanagedType.LPStr)] StringBuilder lpszAddress, ref int piAddressLen, ref ushort pusPort);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern CastMode HP_UdpNode_GetCastMode(IntPtr pNode);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_UdpNode_GetPendingDataLength(IntPtr pNode, ref int piPending);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_UdpNode_SetMaxDatagramSize(IntPtr pNode, uint dwMaxDatagramSize);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_UdpNode_GetMaxDatagramSize(IntPtr pNode);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_UdpNode_SetReuseAddress(IntPtr pNode, bool bReuseAddress);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_UdpNode_IsReuseAddress(IntPtr pNode);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_UdpNode_SetMultiCastTtl(IntPtr pNode, int iMcTtl);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern int HP_UdpNode_GetMultiCastTtl(IntPtr pNode);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_UdpNode_SetMultiCastLoop(IntPtr pNode, bool bMcLoop);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_UdpNode_IsMultiCastLoop(IntPtr pNode);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_UdpNode_SetWorkerThreadCount(IntPtr pNode, uint dwWorkerThreadCount);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_UdpNode_SetPostReceiveCount(IntPtr pNode, uint dwPostReceiveCount);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_UdpNode_SetFreeBufferPoolSize(IntPtr pNode, uint dwFreeBufferPoolSize);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_UdpNode_SetFreeBufferPoolHold(IntPtr pNode, uint dwFreeBufferPoolHold);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_UdpNode_GetWorkerThreadCount(IntPtr pNode);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_UdpNode_GetPostReceiveCount(IntPtr pNode);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_UdpNode_GetFreeBufferPoolSize(IntPtr pNode);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_UdpNode_GetFreeBufferPoolHold(IntPtr pNode);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern ReuseAddressPolicy HP_UdpNode_GetReuseAddressPolicy(IntPtr pServer);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_UdpNode_SetReuseAddressPolicy(IntPtr pServer, ReuseAddressPolicy enReusePolicy);

        /**********************************************************************************/
        /***************************** UdpNode 回调函数设置方法 *****************************/
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_UdpNode_OnPrepareListen(IntPtr pListener, UdpNodeOnPrepareListen fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_UdpNode_OnSend(IntPtr pListener, UdpNodeOnSend fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_UdpNode_OnReceive(IntPtr pListener, UdpNodeOnReceive fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_UdpNode_OnError(IntPtr pListener, UdpNodeOnError fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_UdpNode_OnShutdown(IntPtr pListener, UdpNodeOnShutdown fn);
    }
}
