using System;
using System.Runtime.InteropServices;
using HPSocket.Tcp;

namespace HPSocket.Sdk
{
    internal static class Tcp
    {
        /****************************************************/
        /************** HPSocket4C.dll 导出函数 **************/

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr Create_HP_TcpServer(IntPtr pListener);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr Create_HP_TcpClient(IntPtr pListener);


        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr Create_HP_TcpPullServer(IntPtr pListener);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr Create_HP_TcpPullClient(IntPtr pListener);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr Create_HP_TcpPackServer(IntPtr pListener);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr Create_HP_TcpPackClient(IntPtr pListener);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void Destroy_HP_TcpServer(IntPtr pServer);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void Destroy_HP_TcpClient(IntPtr pClient);


        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void Destroy_HP_TcpPullServer(IntPtr pServer);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void Destroy_HP_TcpPullClient(IntPtr pClient);


        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void Destroy_HP_TcpPackServer(IntPtr pServer);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void Destroy_HP_TcpPackClient(IntPtr pClient);


        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr Create_HP_TcpServerListener();

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr Create_HP_TcpClientListener();


        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr Create_HP_TcpPullServerListener();

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr Create_HP_TcpPullClientListener();

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr Create_HP_TcpPackServerListener();

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr Create_HP_TcpPackClientListener();


        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void Destroy_HP_TcpServerListener(IntPtr pListener);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void Destroy_HP_TcpClientListener(IntPtr pListener);


        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void Destroy_HP_TcpPullServerListener(IntPtr pListener);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void Destroy_HP_TcpPullClientListener(IntPtr pListener);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void Destroy_HP_TcpPackServerListener(IntPtr pListener);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void Destroy_HP_TcpPackClientListener(IntPtr pListener);


        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr Create_HP_TcpAgent(IntPtr pListener);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr Create_HP_TcpPullAgent(IntPtr pListener);


        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr Create_HP_TcpPackAgent(IntPtr pListener);


        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void Destroy_HP_TcpAgent(IntPtr pAgent);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void Destroy_HP_TcpPullAgent(IntPtr pAgent);


        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void Destroy_HP_TcpPackAgent(IntPtr pAgent);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr Create_HP_TcpAgentListener();


        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr Create_HP_TcpPullAgentListener();


        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr Create_HP_TcpPackAgentListener();


        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void Destroy_HP_TcpAgentListener(IntPtr pListener);


        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void Destroy_HP_TcpPullAgentListener(IntPtr pListener);


        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void Destroy_HP_TcpPackAgentListener(IntPtr pListener);

        /**********************************************************************************/
        /***************************** TCP Server 操作方法 *****************************/

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_TcpServer_SendSmallFile(IntPtr pServer, IntPtr connId, string lpszFileName, ref Wsabuf pHead, ref Wsabuf pTail);

        /**********************************************************************************/
        /***************************** TCP Server 属性访问方法 *****************************/

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_TcpServer_SetAcceptSocketCount(IntPtr pServer, uint dwAcceptSocketCount);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_TcpServer_SetSocketBufferSize(IntPtr pServer, uint dwSocketBufferSize);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_TcpServer_SetSocketListenQueue(IntPtr pServer, uint dwSocketListenQueue);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_TcpServer_SetKeepAliveTime(IntPtr pServer, uint dwKeepAliveTime);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_TcpServer_SetKeepAliveInterval(IntPtr pServer, uint dwKeepAliveInterval);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_TcpServer_SetNoDelay(IntPtr pServer, bool bNoDelay);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_TcpServer_GetAcceptSocketCount(IntPtr pServer);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_TcpServer_GetSocketBufferSize(IntPtr pServer);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_TcpServer_GetSocketListenQueue(IntPtr pServer);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_TcpServer_GetKeepAliveTime(IntPtr pServer);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_TcpServer_GetKeepAliveInterval(IntPtr pServer);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_TcpServer_IsNoDelay(IntPtr pServer);



        /**********************************************************************************/
        /***************************** TCP Client 操作方法 *****************************/

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_TcpClient_SendSmallFile(IntPtr pClient, string lpszFileName, ref Wsabuf pHead, ref Wsabuf pTail);

        /**********************************************************************************/
        /***************************** TCP Client 属性访问方法 *****************************/

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_TcpClient_SetSocketBufferSize(IntPtr pClient, uint dwSocketBufferSize);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_TcpClient_SetKeepAliveTime(IntPtr pClient, uint dwKeepAliveTime);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_TcpClient_SetKeepAliveInterval(IntPtr pClient, uint dwKeepAliveInterval);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_TcpClient_SetNoDelay(IntPtr pClient, bool bNoDelay);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_TcpClient_GetSocketBufferSize(IntPtr pClient);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_TcpClient_GetKeepAliveTime(IntPtr pClient);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_TcpClient_GetKeepAliveInterval(IntPtr pClient);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_TcpClient_IsNoDelay(IntPtr pClient);


        /***************************************************************************************/
        /***************************** TCP Pull Server 组件操作方法 *****************************/

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern FetchResult HP_TcpPullServer_Fetch(IntPtr pServer, IntPtr connId, IntPtr pBuffer, int length);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern FetchResult HP_TcpPullServer_Peek(IntPtr pServer, IntPtr connId, IntPtr pBuffer, int length);

        /***************************************************************************************/
        /***************************** TCP Pull Server 属性访问方法 *****************************/

        /***************************************************************************************/
        /***************************** TCP Pull Client 组件操作方法 *****************************/

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern FetchResult HP_TcpPullClient_Fetch(IntPtr pClient, IntPtr pBuffer, int length);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern FetchResult HP_TcpPullClient_Peek(IntPtr pClient, IntPtr pBuffer, int length);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_TcpPackServer_SetMaxPackSize(IntPtr pServer, uint dwMaxPackSize);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_TcpPackServer_SetPackHeaderFlag(IntPtr pServer, ushort usPackHeaderFlag);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_TcpPackServer_GetMaxPackSize(IntPtr pServer);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern ushort HP_TcpPackServer_GetPackHeaderFlag(IntPtr pServer);

        /***************************************************************************************/
        /***************************** TCP Pack Client 组件操作方法 *****************************/

        /***************************************************************************************/
        /***************************** TCP Pack Client 属性访问方法 *****************************/
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_TcpPackClient_SetMaxPackSize(IntPtr pClient, uint dwMaxPackSize);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_TcpPackClient_SetPackHeaderFlag(IntPtr pClient, ushort usPackHeaderFlag);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_TcpPackClient_GetMaxPackSize(IntPtr pClient);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern ushort HP_TcpPackClient_GetPackHeaderFlag(IntPtr pClient);

        /***************************************************************************************/
        /***************************** TCP Agent 属性访问方法 *****************************/

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Agent_SetReuseAddressPolicy(IntPtr pAgent, ReuseAddressPolicy enReusePolicy);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern ReuseAddressPolicy HP_Agent_GetReuseAddressPolicy(IntPtr pAgent);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_TcpAgent_SetSocketBufferSize(IntPtr pAgent, uint dwSocketBufferSize);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_TcpAgent_SetKeepAliveTime(IntPtr pAgent, uint dwKeepAliveTime);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_TcpAgent_SetKeepAliveInterval(IntPtr pAgent, uint dwKeepAliveInterval);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_TcpAgent_SetNoDelay(IntPtr pAgent, bool bNoDelay);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_TcpAgent_GetSocketBufferSize(IntPtr pAgent);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_TcpAgent_GetKeepAliveTime(IntPtr pAgent);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_TcpAgent_GetKeepAliveInterval(IntPtr pAgent);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_TcpAgent_IsNoDelay(IntPtr pAgent);


        /***************************************************************************************/
        /***************************** TCP Pull Agent 组件操作方法 *****************************/

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern FetchResult HP_TcpPullAgent_Fetch(IntPtr pAgent, IntPtr connId, IntPtr pBuffer, int length);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern FetchResult HP_TcpPullAgent_Peek(IntPtr pAgent, IntPtr connId, IntPtr pBuffer, int length);

        /***************************************************************************************/
        /***************************** TCP Pack Agent 属性访问方法 *****************************/

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_TcpPackAgent_SetMaxPackSize(IntPtr pAgent, uint dwMaxPackSize);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_TcpPackAgent_SetPackHeaderFlag(IntPtr pAgent, ushort usPackHeaderFlag);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_TcpPackAgent_GetMaxPackSize(IntPtr pAgent);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern ushort HP_TcpPackAgent_GetPackHeaderFlag(IntPtr pAgent);

        /******************************************************************************/
        /***************************** Agent 操作方法 *****************************/

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_TcpAgent_SendSmallFile(IntPtr pAgent, IntPtr connId, string lpszFileName, ref Wsabuf pHead, ref Wsabuf pTail);

    }
}
