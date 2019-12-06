using System;
using HPSocket;
using HPSocket.WebSocket;

namespace WebSocketEchoServer
{
    public class EchoHub : IHub
    {
        /// <summary>
        /// 消息到达
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="connId"></param>
        /// <param name="final"></param>
        /// <param name="opCode"></param>
        /// <param name="mask"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public HandleResult OnMessage(IWebSocketServer sender, IntPtr connId, bool final, OpCode opCode, byte[] mask, byte[] data)
        {
            Console.WriteLine($"OnMessage({connId}), final: {final}, op code: {opCode}, data length: {data.Length}");
            var ok = sender.Send(connId, opCode, data, data.Length);
            return ok ? HandleResult.Ok : HandleResult.Error;
        }

        /// <summary>
        /// 连接打开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="connId"></param>
        /// <returns></returns>
        public HandleResult OnOpen(IWebSocketServer sender, IntPtr connId)
        {
            var httpServer = (IHttpServer) sender.Http;
            Console.WriteLine($"OnOpen({connId}), cookie: {httpServer.GetHeader(connId, "Cookie")}, user-agent: {httpServer.GetHeader(connId, "User-Agent")}");

            return HandleResult.Ok;
        }

        /// <summary>
        /// 连接关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="connId"></param>
        /// <param name="socketOperation"></param>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        public HandleResult OnClose(IWebSocketServer sender, IntPtr connId, SocketOperation socketOperation, int errorCode)
        {
            Console.WriteLine($"OnClose({connId}), socket operation: {socketOperation}, error code: {errorCode}");
            return HandleResult.Ok;
        }

        public void OnPing(IWebSocketServer sender, IntPtr connId, byte[] data)
        {
        }

        public void OnPong(IWebSocketServer sender, IntPtr connId, byte[] data)
        {
        }
    }
}
