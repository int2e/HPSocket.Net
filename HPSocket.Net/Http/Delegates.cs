using System;
using HPSocket.WebSocket;

namespace HPSocket.Http
{
    public delegate HttpParseResult MessageBeginEventHandler(IHttp sender, IntPtr connId);
    public delegate HttpParseResult HeaderEventHandler(IHttp sender, IntPtr connId, string name, string value);
    public delegate HttpParseResultEx HeadersCompleteEventHandler(IHttp sender, IntPtr connId);
    public delegate HttpParseResult BodyEventHandler(IHttp sender, IntPtr connId, byte[] data);

    public delegate HttpParseResult ChunkHeaderEventHandler(IHttp sender, IntPtr connId, int length);
    public delegate HttpParseResult ChunkCompleteEventHandler(IHttp sender, IntPtr connId);
    public delegate HttpParseResult MessageCompleteEventHandler(IHttp sender, IntPtr connId);
    public delegate HttpParseResult UpgradeEventHandler(IHttp sender, IntPtr connId, HttpUpgradeType upgradeType);
    public delegate HttpParseResult ParseErrorEventHandler(IHttp sender, IntPtr connId, int errorCode, string errorDesc);

    // server
    public delegate HttpParseResult RequestLineEventHandler(IHttp sender, IntPtr connId, string method, string url);

    // client
    public delegate HttpParseResult StatusLineEventHandler(IHttp sender, IntPtr connId, ushort statusCode, string desc);

    // websocket
    public delegate HandleResult WsMessageHeaderEventHandler(IHttp sender, IntPtr connId, bool final, Rsv rsv, OpCode opCode, byte[] mask, ulong bodyLength);
    public delegate HandleResult WsMessageBodyEventHandler(IHttp sender, IntPtr connId, byte[] data);
    public delegate HandleResult WsMessageCompleteEventHandler(IHttp sender, IntPtr connId);

    // easy data
    public delegate HttpParseResult HttpServerEasyDataEventHandler(IHttpEasyServer sender, IntPtr connId, byte[] data);
    public delegate HttpParseResult HttpAgentEasyDataEventHandler(IHttpEasyAgent sender, IntPtr connId, byte[] data);
    public delegate HttpParseResult HttpClientEasyDataEventHandler(IHttpEasyClient sender, byte[] data);
    public delegate HandleResult HttpServerWebSocketEasyDataEventHandler(IHttpEasyServer sender, IntPtr connId, bool final, Rsv rsv, OpCode opCode, byte[] mask, byte[] data);
    public delegate HandleResult HttpAgentWebSocketEasyDataEventHandler(IHttpEasyAgent sender, IntPtr connId, bool final, Rsv rsv, OpCode opCode, byte[] mask, byte[] data);
    public delegate HandleResult HttpClientWebSocketEasyDataEventHandler(IHttpEasyClient sender, bool final, Rsv rsv, OpCode opCode, byte[] mask, byte[] data);
}
