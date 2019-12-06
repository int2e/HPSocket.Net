using System;
using System.Runtime.InteropServices;
using System.Text;
using HPSocket.Http;
using HPSocket.WebSocket;

namespace HPSocket.Sdk
{
    internal static class Http
    {
        /****************************************************/
        /***************** HTTP 对象创建函数 *****************/

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr Create_HP_HttpServer(IntPtr pListener);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr Create_HP_HttpAgent(IntPtr pListener);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr Create_HP_HttpClient(IntPtr pListener);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr Create_HP_HttpSyncClient(IntPtr pListener);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void Destroy_HP_HttpServer(IntPtr pServer);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void Destroy_HP_HttpAgent(IntPtr pAgent);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void Destroy_HP_HttpClient(IntPtr pClient);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void Destroy_HP_HttpSyncClient(IntPtr pClient);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr Create_HP_HttpServerListener();
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr Create_HP_HttpAgentListener();
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr Create_HP_HttpClientListener();

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void Destroy_HP_HttpServerListener(IntPtr pListener);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void Destroy_HP_HttpAgentListener(IntPtr pListener);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void Destroy_HP_HttpClientListener(IntPtr pListener);


        /**********************************************************************************/
        /*************************** HTTP Server 回调函数设置方法 **************************/

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpServer_OnMessageBegin(IntPtr pListener, OnMessageBegin fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpServer_OnRequestLine(IntPtr pListener, OnRequestLine fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpServer_OnHeader(IntPtr pListener, OnHeader fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpServer_OnHeadersComplete(IntPtr pListener, OnHeadersComplete fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpServer_OnBody(IntPtr pListener, OnBody fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpServer_OnChunkHeader(IntPtr pListener, OnChunkHeader fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpServer_OnChunkComplete(IntPtr pListener, OnChunkComplete fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpServer_OnMessageComplete(IntPtr pListener, OnMessageComplete fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpServer_OnUpgrade(IntPtr pListener, OnUpgrade fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpServer_OnParseError(IntPtr pListener, OnParseError fn);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpServer_OnWSMessageHeader(IntPtr pListener, OnWsMessageHeader fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpServer_OnWSMessageBody(IntPtr pListener, OnWsMessageBody fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpServer_OnWSMessageComplete(IntPtr pListener, OnWsMessageComplete fn);


        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpServer_OnPrepareListen(IntPtr pListener, OnPrepareListen fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpServer_OnAccept(IntPtr pListener, OnAccept fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpServer_OnHandShake(IntPtr pListener, OnHandShake fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpServer_OnReceive(IntPtr pListener, OnReceive fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpServer_OnSend(IntPtr pListener, OnSend fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpServer_OnClose(IntPtr pListener, OnClose fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpServer_OnShutdown(IntPtr pListener, OnShutdown fn);

        /**********************************************************************************/
        /**************************** HTTP Agent 回调函数设置方法 **************************/

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpAgent_OnMessageBegin(IntPtr pListener, OnMessageBegin fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpAgent_OnStatusLine(IntPtr pListener, OnStatusLine fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpAgent_OnHeader(IntPtr pListener, OnHeader fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpAgent_OnHeadersComplete(IntPtr pListener, OnHeadersComplete fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpAgent_OnBody(IntPtr pListener, OnBody fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpAgent_OnChunkHeader(IntPtr pListener, OnChunkHeader fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpAgent_OnChunkComplete(IntPtr pListener, OnChunkComplete fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpAgent_OnMessageComplete(IntPtr pListener, OnMessageComplete fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpAgent_OnUpgrade(IntPtr pListener, OnUpgrade fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpAgent_OnParseError(IntPtr pListener, OnParseError fn);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpAgent_OnWSMessageHeader(IntPtr pListener, OnWsMessageHeader fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpAgent_OnWSMessageBody(IntPtr pListener, OnWsMessageBody fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpAgent_OnWSMessageComplete(IntPtr pListener, OnWsMessageComplete fn);


        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpAgent_OnPrepareConnect(IntPtr pListener, OnPrepareConnect fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpAgent_OnConnect(IntPtr pListener, OnConnect fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpAgent_OnHandShake(IntPtr pListener, OnHandShake fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpAgent_OnReceive(IntPtr pListener, OnReceive fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpAgent_OnSend(IntPtr pListener, OnSend fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpAgent_OnClose(IntPtr pListener, OnClose fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpAgent_OnShutdown(IntPtr pListener, OnShutdown fn);

        /**********************************************************************************/
        /*************************** HTTP Client 回调函数设置方法 **************************/

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpClient_OnMessageBegin(IntPtr pListener, OnMessageBegin fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpClient_OnStatusLine(IntPtr pListener, OnStatusLine fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpClient_OnHeader(IntPtr pListener, OnHeader fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpClient_OnHeadersComplete(IntPtr pListener, OnHeadersComplete fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpClient_OnBody(IntPtr pListener, OnBody fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpClient_OnChunkHeader(IntPtr pListener, OnChunkHeader fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpClient_OnChunkComplete(IntPtr pListener, OnChunkComplete fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpClient_OnMessageComplete(IntPtr pListener, OnMessageComplete fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpClient_OnUpgrade(IntPtr pListener, OnUpgrade fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpClient_OnParseError(IntPtr pListener, OnParseError fn);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpClient_OnWSMessageHeader(IntPtr pListener, OnWsMessageHeader fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpClient_OnWSMessageBody(IntPtr pListener, OnWsMessageBody fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpClient_OnWSMessageComplete(IntPtr pListener, OnWsMessageComplete fn);


        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpClient_OnPrepareConnect(IntPtr pListener, OnPrepareConnect fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpClient_OnConnect(IntPtr pListener, OnConnect fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpClient_OnHandShake(IntPtr pListener, OnHandShake fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpClient_OnReceive(IntPtr pListener, OnReceive fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpClient_OnSend(IntPtr pListener, OnSend fn);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_Set_FN_HttpClient_OnClose(IntPtr pListener, OnClose fn);



        /**************************************************************************/
        /***************************** Server 操作方法 *****************************/

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpServer_SendResponse(IntPtr pServer, IntPtr dwConnId, ushort usStatusCode, string lpszDesc, NameValue[] lpHeaders, int iHeaderCount, IntPtr pData, int iLength);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpServer_SendLocalFile(IntPtr pServer, IntPtr dwConnId, string lpszFileName, ushort usStatusCode, string lpszDesc, NameValue[] lpHeaders, int iHeaderCount);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpServer_SendChunkData(IntPtr pServer, IntPtr dwConnId, IntPtr pData /*= nullptr*/, int iLength /*= 0*/, string lpszExtensions /*= nullptr*/);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpServer_SendWSMessage(IntPtr pServer, IntPtr dwConnId, bool bFinal, Rsv iRsv, OpCode iOperationCode, IntPtr pdata, int iLength, ulong ullBodyLen);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpServer_Release(IntPtr pServer, IntPtr dwConnId);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpServer_StartHttp(IntPtr pServer, IntPtr dwConnId);

        /******************************************************************************/
        /***************************** Server 属性访问方法 *****************************/

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_HttpServer_SetReleaseDelay(IntPtr pServer, uint dwReleaseDelay);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_HttpServer_GetReleaseDelay(IntPtr pServer);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern ushort HP_HttpServer_GetUrlFieldSet(IntPtr pServer, IntPtr dwConnId);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr HP_HttpServer_GetUrlField(IntPtr pServer, IntPtr dwConnId, HttpUrlField enField);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr HP_HttpServer_GetMethod(IntPtr pServer, IntPtr dwConnId);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_HttpServer_SetLocalVersion(IntPtr pServer, HttpVersion usVersion);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern HttpVersion HP_HttpServer_GetLocalVersion(IntPtr pServer);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpServer_IsUpgrade(IntPtr pServer, IntPtr dwConnId);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpServer_IsKeepAlive(IntPtr pServer, IntPtr dwConnId);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern HttpVersion HP_HttpServer_GetVersion(IntPtr pServer, IntPtr dwConnId);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr HP_HttpServer_GetHost(IntPtr pServer, IntPtr dwConnId);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern long HP_HttpServer_GetContentLength(IntPtr pServer, IntPtr dwConnId);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr HP_HttpServer_GetContentType(IntPtr pServer, IntPtr dwConnId);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr HP_HttpServer_GetContentEncoding(IntPtr pServer, IntPtr dwConnId);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr HP_HttpServer_GetTransferEncoding(IntPtr pServer, IntPtr dwConnId);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern HttpUpgradeType HP_HttpServer_GetUpgradeType(IntPtr pServer, IntPtr dwConnId);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern ushort HP_HttpServer_GetParseErrorCode(IntPtr pServer, IntPtr dwConnId, ref IntPtr lpszErrorDesc);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpServer_GetHeader(IntPtr pServer, IntPtr dwConnId, string lpszName, ref IntPtr lpszValue);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpServer_GetHeaders(IntPtr pServer, IntPtr dwConnId, string lpszName, IntPtr[] lpszValue, ref uint pdwCount);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpServer_GetAllHeaders(IntPtr pServer, IntPtr dwConnId, IntPtr lpHeaders, ref uint pdwCount);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpServer_GetAllHeaderNames(IntPtr pServer, IntPtr dwConnId, IntPtr[] lpszName, ref uint pdwCount);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpServer_GetCookie(IntPtr pServer, IntPtr dwConnId, string lpszName, ref IntPtr lpszValue);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpServer_GetAllCookies(IntPtr pServer, IntPtr dwConnId, IntPtr lpCookies, ref uint pdwCount);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpServer_GetWSMessageState(IntPtr pServer, IntPtr dwConnId, ref bool lpbFinal, ref Rsv lpiRsv, ref OpCode lpiOperationCode, ref IntPtr lpszMask, ref ulong bodyLen, ref ulong bodyRemain);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_HttpServer_SetHttpAutoStart(IntPtr pServer, bool bAutoStart);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpServer_IsHttpAutoStart(IntPtr pServer);

        /**************************************************************************/
        /***************************** Agent 操作方法 ******************************/


        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpAgent_SendRequest(IntPtr pAgent, IntPtr dwConnId, string lpszMethod, string lpszPath, NameValue[] lpHeaders, int iHeaderCount, IntPtr pData, int iLength);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpAgent_SendLocalFile(IntPtr pAgent, IntPtr dwConnId, string lpszFileName, string lpszMethod, string lpszPath, NameValue[] lpHeaders, int iHeaderCount);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpAgent_SendChunkData(IntPtr pAgent, IntPtr dwConnId, IntPtr pData /*= nullptr*/, int iLength /*= 0*/, string lpszExtensions /*= nullptr*/);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpAgent_SendPost(IntPtr pAgent, IntPtr dwConnId, string lpszPath, NameValue[] lpHeaders, int iHeaderCount, string pBody, int iLength);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpAgent_SendPut(IntPtr pAgent, IntPtr dwConnId, string lpszPath, NameValue[] lpHeaders, int iHeaderCount, string pBody, int iLength);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpAgent_SendPatch(IntPtr pAgent, IntPtr dwConnId, string lpszPath, NameValue[] lpHeaders, int iHeaderCount, string pBody, int iLength);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpAgent_SendGet(IntPtr pAgent, IntPtr dwConnId, string lpszPath, NameValue[] lpHeaders, int iHeaderCount);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpAgent_SendDelete(IntPtr pAgent, IntPtr dwConnId, string lpszPath, NameValue[] lpHeaders, int iHeaderCount);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpAgent_SendHead(IntPtr pAgent, IntPtr dwConnId, string lpszPath, NameValue[] lpHeaders, int iHeaderCount);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpAgent_SendTrace(IntPtr pAgent, IntPtr dwConnId, string lpszPath, NameValue[] lpHeaders, int iHeaderCount);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpAgent_SendOptions(IntPtr pAgent, IntPtr dwConnId, string lpszPath, NameValue[] lpHeaders, int iHeaderCount);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpAgent_SendConnect(IntPtr pAgent, IntPtr dwConnId, string lpszHost, NameValue[] lpHeaders, int iHeaderCount);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpAgent_SendWSMessage(IntPtr pAgent, IntPtr dwConnId, bool bFinal, Rsv iRsv, OpCode iOperationCode, IntPtr lpszMask, IntPtr pData, int iLength, ulong ullBodyLen);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpAgent_StartHttp(IntPtr pAgent, IntPtr dwConnId);

        /******************************************************************************/
        /***************************** Agent 属性访问方法 ******************************/

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern HttpStatusCode HP_HttpAgent_GetStatusCode(IntPtr pAgent, IntPtr dwConnId);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_HttpAgent_SetLocalVersion(IntPtr pAgent, HttpVersion usVersion);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern HttpVersion HP_HttpAgent_GetLocalVersion(IntPtr pAgent);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpAgent_IsUpgrade(IntPtr pAgent, IntPtr dwConnId);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpAgent_IsKeepAlive(IntPtr pAgent, IntPtr dwConnId);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern HttpVersion HP_HttpAgent_GetVersion(IntPtr pAgent, IntPtr dwConnId);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern long HP_HttpAgent_GetContentLength(IntPtr pAgent, IntPtr dwConnId);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr HP_HttpAgent_GetContentType(IntPtr pAgent, IntPtr dwConnId);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr HP_HttpAgent_GetContentEncoding(IntPtr pAgent, IntPtr dwConnId);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr HP_HttpAgent_GetTransferEncoding(IntPtr pAgent, IntPtr dwConnId);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern HttpUpgradeType HP_HttpAgent_GetUpgradeType(IntPtr pAgent, IntPtr dwConnId);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern ushort HP_HttpAgent_GetParseErrorCode(IntPtr pAgent, IntPtr dwConnId, ref IntPtr lpszErrorDesc);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpAgent_GetHeader(IntPtr pAgent, IntPtr dwConnId, string lpszName, ref IntPtr lpszValue);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpAgent_GetHeaders(IntPtr pAgent, IntPtr dwConnId, string lpszName, IntPtr[] lpszValue, ref uint pdwCount);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpAgent_GetAllHeaders(IntPtr pAgent, IntPtr dwConnId, IntPtr lpHeaders, ref uint pdwCount);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpAgent_GetAllHeaderNames(IntPtr pAgent, IntPtr dwConnId, IntPtr[] lpszName, ref uint pdwCount);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_HttpAgent_SetUseCookie(IntPtr pAgent, bool bUseCookie);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpAgent_IsUseCookie(IntPtr pAgent);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpAgent_GetCookie(IntPtr pAgent, IntPtr dwConnId, string lpszName, ref IntPtr lpszValue);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpAgent_GetAllCookies(IntPtr pAgent, IntPtr dwConnId, IntPtr lpCookies, ref uint pdwCount);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpAgent_GetWSMessageState(IntPtr pAgent, IntPtr dwConnId, ref bool lpbFinal, ref Rsv lpiRsv, ref OpCode lpiOperationCode, ref IntPtr lpszMask, ref ulong bodyLen, ref ulong bodyRemain);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_HttpAgent_SetHttpAutoStart(IntPtr pAgent, bool bAutoStart);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpAgent_IsHttpAutoStart(IntPtr pAgent);



        /**************************************************************************/
        /***************************** Client 操作方法 *****************************/

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpClient_SendRequest(IntPtr pClient, string lpszMethod, string lpszPath, NameValue[] lpHeaders, int iHeaderCount, IntPtr pBody, int iLength);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpClient_SendLocalFile(IntPtr pClient, string lpszFileName, string lpszMethod, string lpszPath, NameValue[] lpHeaders, int iHeaderCount);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpClient_SendChunkData(IntPtr pClient, IntPtr pData /*= nullptr*/, int iLength /*= 0*/, string lpszExtensions /*= nullptr*/);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpClient_SendPost(IntPtr pClient, string lpszPath, NameValue[] lpHeaders, int iHeaderCount, string pBody, int iLength);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpClient_SendPut(IntPtr pClient, string lpszPath, NameValue[] lpHeaders, int iHeaderCount, string pBody, int iLength);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpClient_SendPatch(IntPtr pClient, string lpszPath, NameValue[] lpHeaders, int iHeaderCount, string pBody, int iLength);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpClient_SendGet(IntPtr pClient, string lpszPath, NameValue[] lpHeaders, int iHeaderCount);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpClient_SendDelete(IntPtr pClient, string lpszPath, NameValue[] lpHeaders, int iHeaderCount);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpClient_SendHead(IntPtr pClient, string lpszPath, NameValue[] lpHeaders, int iHeaderCount);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpClient_SendTrace(IntPtr pClient, string lpszPath, NameValue[] lpHeaders, int iHeaderCount);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpClient_SendOptions(IntPtr pClient, string lpszPath, NameValue[] lpHeaders, int iHeaderCount);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpClient_SendConnect(IntPtr pClient, string lpszHost, NameValue[] lpHeaders, int iHeaderCount);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpClient_SendWSMessage(IntPtr pClient, bool bFinal, Rsv iRsv, OpCode iOperationCode, IntPtr lpszMask, IntPtr pData, int iLength, ulong bodyLen);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpClient_StartHttp(IntPtr pClient);


        /******************************************************************************/
        /***************************** Client 属性访问方法 *****************************/

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern HttpStatusCode HP_HttpClient_GetStatusCode(IntPtr pClient);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_HttpClient_SetLocalVersion(IntPtr pClient, HttpVersion usVersion);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern HttpVersion HP_HttpClient_GetLocalVersion(IntPtr pClient);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpClient_IsUpgrade(IntPtr pClient);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpClient_IsKeepAlive(IntPtr pClient);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern HttpVersion HP_HttpClient_GetVersion(IntPtr pClient);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern long HP_HttpClient_GetContentLength(IntPtr pClient);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr HP_HttpClient_GetContentType(IntPtr pClient);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr HP_HttpClient_GetContentEncoding(IntPtr pClient);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr HP_HttpClient_GetTransferEncoding(IntPtr pClient);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern HttpUpgradeType HP_HttpClient_GetUpgradeType(IntPtr pClient);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern ushort HP_HttpClient_GetParseErrorCode(IntPtr pClient, ref IntPtr lpszErrorDesc);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpClient_GetHeader(IntPtr pClient, string lpszName, ref IntPtr lpszValue);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpClient_GetHeaders(IntPtr pClient, string lpszName, IntPtr[] lpszValue, ref uint pdwCount);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpClient_GetAllHeaders(IntPtr pClient, IntPtr lpHeaders, ref uint pdwCount);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpClient_GetAllHeaderNames(IntPtr pClient, IntPtr[] lpszName, ref uint pdwCount);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_HttpClient_SetUseCookie(IntPtr pClient, bool bUseCookie);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpClient_IsUseCookie(IntPtr pClient);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpClient_GetCookie(IntPtr pClient, string lpszName, ref IntPtr lpszValue);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpClient_GetAllCookies(IntPtr pClient, IntPtr lpCookies, ref uint pdwCount);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpClient_GetWSMessageState(IntPtr pClient, ref bool lpbFinal, ref Rsv lpiRsv, ref OpCode lpiOperationCode, ref IntPtr lpszMask, ref ulong bodyLen, ref ulong bodyRemain);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_HttpClient_SetHttpAutoStart(IntPtr pClient, bool bAutoStart);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpClient_IsHttpAutoStart(IntPtr pClient);
        /**************************************************************************/
        /************************ HTTP Sync Client 操作方法 ************************/
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpSyncClient_OpenUrl(IntPtr pClient, string lpszMethod, string lpszUrl, NameValue[] lpHeaders, int iHeaderCount, IntPtr pData, int iLength, bool bForceReconnect);


        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpSyncClient_CleanupRequestResult(IntPtr pClient);

        /******************************************************************************/
        /************************ HTTP Sync Client 属性访问方法 ************************/
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_HttpSyncClient_SetConnectTimeout(IntPtr pClient, uint dwConnectTimeout);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_HttpSyncClient_SetRequestTimeout(IntPtr pClient, uint dwRequestTimeout);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_HttpSyncClient_GetConnectTimeout(IntPtr pClient);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_HttpSyncClient_GetRequestTimeout(IntPtr pClient);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpSyncClient_GetResponseBody(IntPtr pClient, ref IntPtr lpszBody, ref int iLength);

        /**************************************************************************/
        /*************************** HTTP Cookie 管理方法 **************************/
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpCookie_MGR_LoadFromFile(string lpszFile, bool bKeepExists /*= true*/);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpCookie_MGR_SaveToFile(string lpszFile, bool bKeepExists /*= true*/);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpCookie_MGR_ClearCookies(string lpszDomain /*= nullptr*/, string lpszPath /*= nullptr*/);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpCookie_MGR_RemoveExpiredCookies(string lpszDomain /*= nullptr*/, string lpszPath /*= nullptr*/);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpCookie_MGR_SetCookie(string lpszName, string lpszValue, string lpszDomain, string lpszPath, int iMaxAge /*= -1*/, bool bHttpOnly /*= false*/, bool bSecure /*= false*/, int enSameSite /*= 0*/, bool bOnlyUpdateValueIfExists /*= true*/);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpCookie_MGR_DeleteCookie(string lpszDomain, string lpszPath, string lpszName);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void HP_HttpCookie_MGR_SetEnableThirdPartyCookie(bool bEnableThirdPartyCookie /*= true*/);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpCookie_MGR_IsEnableThirdPartyCookie();

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpCookie_HLP_ParseExpires(string lpszExpires, ref ulong ptmExpires);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpCookie_HLP_MakeExpiresStr([MarshalAs(UnmanagedType.LPStr)] StringBuilder lpszBuff, ref int piBuffLen, ulong tmExpires);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_HttpCookie_HLP_ToString([MarshalAs(UnmanagedType.LPStr)] StringBuilder lpszBuff, ref int piBuffLen, string lpszName, string lpszValue, string lpszDomain, string lpszPath, int iMaxAge /*= -1*/, bool bHttpOnly /*= false*/, bool bSecure /*= false*/, int enSameSite /*= 0*/);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern ulong HP_HttpCookie_HLP_CurrentUTCTime();
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern ulong HP_HttpCookie_HLP_MaxAgeToExpires(int iMaxAge);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern int HP_HttpCookie_HLP_ExpiresToMaxAge(ulong tmExpires);


        /**************************************************************************/
        /*************************** HTTPS 方法 **************************/
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr Create_HP_HttpsServer(IntPtr pListener);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr Create_HP_HttpsAgent(IntPtr pListener);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr Create_HP_HttpsClient(IntPtr pListener);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr Create_HP_HttpsSyncClient(IntPtr pListener);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void Destroy_HP_HttpsServer(IntPtr pServer);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void Destroy_HP_HttpsAgent(IntPtr pAgent);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void Destroy_HP_HttpsClient(IntPtr pClient);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void Destroy_HP_HttpsSyncClient(IntPtr pClient);
    }
}
