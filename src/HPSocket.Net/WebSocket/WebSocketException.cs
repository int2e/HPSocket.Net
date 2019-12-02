using System;

namespace HPSocket.WebSocket
{
    public class WebSocketException : Exception
    {
        public WebSocketException(string errorMsg)
            : base(errorMsg)
        {

        }
        public WebSocketException(SocketError errorCode, string errorMsg)
        : base($"[{errorCode}]-{errorMsg}")
        {

        }
    }
}
