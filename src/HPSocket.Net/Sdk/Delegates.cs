using System;

using HPSocket.Http;
using HPSocket.Thread;
using HPSocket.WebSocket;
// ReSharper disable InconsistentNaming

namespace HPSocket.Sdk
{
    public delegate IntPtr CreateListenerDelegate();
    public delegate IntPtr CreateServiceDelegate(IntPtr ptr);
    public delegate void DestroyListenerDelegate(IntPtr ptr);


    /****************************************************/
    /************** HPSocket4C.dll 回调函数 **************/
    /* Agent & Server & Client */
    public delegate HandleResult OnSend(IntPtr sender, IntPtr connId, IntPtr data, int length);
    public delegate HandleResult OnReceive(IntPtr sender, IntPtr connId, IntPtr data, int length);
    public delegate HandleResult OnPullReceive(IntPtr sender, IntPtr connId, int length);
    public delegate HandleResult OnClose(IntPtr sender, IntPtr connId, SocketOperation socketOperation, int errorCode);
    public delegate HandleResult OnHandShake(IntPtr sender, IntPtr connId);


    /* Agent & Server */
    public delegate HandleResult OnShutdown(IntPtr sender);

    /* Agent & Client */
    public delegate HandleResult OnPrepareConnect(IntPtr sender, IntPtr connId /* IntPtr pClient */, IntPtr socket);
    public delegate HandleResult OnConnect(IntPtr sender, IntPtr connId /* IntPtr pClient */);

    /* Server */
    public delegate HandleResult OnPrepareListen(IntPtr sender, IntPtr soListen);
    public delegate HandleResult OnAccept(IntPtr sender, IntPtr connId, IntPtr pClient);


    /****************************************************/
    /******************* HTTP 回调函数 *******************/
    /* Agent & Server & client */
    public delegate HttpParseResult OnMessageBegin(IntPtr sender, IntPtr connId);
    public delegate HttpParseResult OnHeader(IntPtr sender, IntPtr connId, string lpszName, string lpszValue);
    public delegate HttpParseResultEx OnHeadersComplete(IntPtr sender, IntPtr connId);
    public delegate HttpParseResult OnBody(IntPtr sender, IntPtr connId, IntPtr data, int iLength);
    public delegate HttpParseResult OnChunkHeader(IntPtr sender, IntPtr connId, int iLength);
    public delegate HttpParseResult OnChunkComplete(IntPtr sender, IntPtr connId);
    public delegate HttpParseResult OnMessageComplete(IntPtr sender, IntPtr connId);
    public delegate HttpParseResult OnUpgrade(IntPtr sender, IntPtr connId, HttpUpgradeType enUpgradeType);
    public delegate HttpParseResult OnParseError(IntPtr sender, IntPtr connId, int iErrorCode, string lpszErrorDesc);

    /* Server */
    public delegate HttpParseResult OnRequestLine(IntPtr sender, IntPtr connId, string lpszMethod, string lpszUrl);

    /* Agent & Client */
    public delegate HttpParseResult OnStatusLine(IntPtr sender, IntPtr connId, ushort usStatusCode, string lpszDesc);

    /* WebSocket */
    public delegate HandleResult OnWsMessageHeader(IntPtr sender, IntPtr dwConnId, bool bFinal, Rsv iRsv, OpCode opCode, byte[] lpszMask, ulong ullBodyLen);
    public delegate HandleResult OnWsMessageBody(IntPtr sender, IntPtr dwConnId, IntPtr data, int length);
    public delegate HandleResult OnWsMessageComplete(IntPtr sender, IntPtr dwConnId);

    /* UdpNode */
    public delegate HandleResult UdpNodeOnPrepareListen(IntPtr sender, IntPtr soListen);
    public delegate HandleResult UdpNodeOnSend(IntPtr sender, string remoteAddress, ushort usRemotePort, IntPtr pData, int length);
    public delegate HandleResult UdpNodeOnReceive(IntPtr sender, string remoteAddress, ushort usRemotePort, IntPtr pData, int length);
    public delegate HandleResult UdpNodeOnError(IntPtr sender, SocketOperation socketOperation, int errorCode, string remoteAddress, ushort remotePort, IntPtr pData, int length);
    public delegate HandleResult UdpNodeOnShutdown(IntPtr sender);


    /****************************************************/
    /******************* 线程池 回调函数 *******************/
    public delegate void SocketTaskProc(ref SocketTask task);
    public delegate void ThreadPoolOnStartup(IntPtr threadPoolPtr);
    public delegate void ThreadPoolOnShutdown(IntPtr threadPoolPtr);
    public delegate void ThreadPoolOnWorkerThreadStart(IntPtr threadPoolPtr, IntPtr threadId);
    public delegate void ThreadPoolOnWorkerThreadEnd(IntPtr threadPoolPtr, IntPtr threadId);

}
