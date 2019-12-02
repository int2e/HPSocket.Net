using System;

namespace HPSocket.WebSocket
{
    public delegate HandleResult MessageEventHandler(IWebSocketAgent sender, IntPtr connId, bool final, OpCode opCode, byte[] mask, byte[] data);
    public delegate HandleResult OpenEventHandler(IWebSocketAgent sender, IntPtr connId);
    public delegate HandleResult CloseEventHandler(IWebSocketAgent sender, IntPtr connId, SocketOperation socketOperation, int errorCode);
    public delegate void PingEventHandler(IWebSocketAgent sender, IntPtr connId, byte[] data);
    public delegate void PongEventHandler(IWebSocketAgent sender, IntPtr connId, byte[] data);
}
