using System;

namespace HPSocket.WebSocket
{
    /// <summary>
    /// web socket hub
    /// </summary>
    public interface IHub
    {
        /// <summary>
        /// cont/text/binary 消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="connId"></param>
        /// <param name="final"></param>
        /// <param name="opCode"></param>
        /// <param name="mask"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        // ReSharper disable once InconsistentNaming
        HandleResult OnMessage(IWebSocketServer sender, IntPtr connId, bool final, OpCode opCode, byte[] mask, byte[] data);

        /// <summary>
        /// 握手成功, 打开/进入 连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="connId"></param>
        /// <returns></returns>
        // ReSharper disable once InconsistentNaming
        HandleResult OnOpen(IWebSocketServer sender, IntPtr connId);

        /// <summary>
        /// 连接关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="connId"></param>
        /// <param name="socketOperation"></param>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        // ReSharper disable once InconsistentNaming
        HandleResult OnClose(IWebSocketServer sender, IntPtr connId, SocketOperation socketOperation, int errorCode);

        /// <summary>
        /// ping 消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="connId"></param>
        /// <param name="data"></param>
        // ReSharper disable once InconsistentNaming
        void OnPing(IWebSocketServer sender, IntPtr connId, byte[] data);

        /// <summary>
        /// pong 消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="connId"></param>
        /// <param name="data"></param>
        // ReSharper disable once InconsistentNaming
        void OnPong(IWebSocketServer sender, IntPtr connId, byte[] data);

    }
}
