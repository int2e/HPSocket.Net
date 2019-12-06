using System;
using System.Text;
using HPSocket;
using HPSocket.WebSocket;

namespace WebSocketEchoServer
{
    public class WelcomeHub : IHub
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
            var ok = sender.Text(connId, $"这里只是一个欢迎页面, 只是欢迎您, 您可以连接到 {(sender.IsSecure ? "wss" : "ws")}://{sender.Uri.Host}:{sender.Uri.Port}/echo");
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
            var httpServer = (IHttpServer)sender.Http;
            Console.WriteLine($"OnOpen({connId}), cookie: [{httpServer.GetHeader(connId, "Cookie")}], user-agent: [{httpServer.GetHeader(connId, "User-Agent")}]");

            var str = "Websocket是一种用于H5浏览器的实时通讯协议，可以做到数据的实时推送，可适用于广泛的工作环境，例如客服系统、物联网数据传输系统。";
            // 发送文本消息1
            var bytes = Encoding.UTF8.GetBytes(str);
            var ok = sender.Send(connId, OpCode.Text, bytes, bytes.Length);
            // 发送文本消息2
            // var ok = sender.Text(connId, str);
            return ok ? HandleResult.Ok : HandleResult.Error;
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
