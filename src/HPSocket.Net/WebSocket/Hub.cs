using System;
#if NET20 || NET30 || NET35
using System.Collections.Generic;
#else
using System.Collections.Concurrent;
#endif


namespace HPSocket.WebSocket
{
    /// <summary>
    /// web socket hub
    /// </summary>
    public abstract class Hub : IHub
    {
#if NET20 || NET30 || NET35
        private readonly List<IntPtr> _connectionIds = new List<IntPtr>();
        
        /// <summary>
        /// 获取连接到当前Hub的连接
        /// </summary>
        public IntPtr[] ConnectionIds => _connectionIds.ToArray();
#else
        private readonly ConcurrentBag<IntPtr> _connectionIds = new ConcurrentBag<IntPtr>();

        /// <summary>
        /// 获取连接到当前Hub的连接
        /// </summary>
        public IntPtr[] ConnectionIds => _connectionIds.ToArray();
#endif

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
        public virtual HandleResult OnMessage(IWebSocketServer sender, IntPtr connId, bool final, OpCode opCode, byte[] mask, byte[] data) => HandleResult.Ok;

        /// <summary>
        /// 握手成功, 打开/进入 连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="connId"></param>
        /// <returns></returns>
        public virtual HandleResult OnOpen(IWebSocketServer sender, IntPtr connId)
        {
#if NET20 || NET30 || NET35
            lock (_connectionIds)
            {
                _connectionIds.Add(connId);
            }
#else
            _connectionIds.Add(connId);
#endif
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
        public virtual HandleResult OnClose(IWebSocketServer sender, IntPtr connId, SocketOperation socketOperation, int errorCode)
        {
#if NET20 || NET30 || NET35
            lock (_connectionIds)
            {
                _connectionIds.Remove(connId);
            }
#else
            _connectionIds.TryTake(out var _);
#endif
            return HandleResult.Ok;
        }

        /// <summary>
        /// ping 消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="connId"></param>
        /// <param name="data"></param>
        public virtual void OnPing(IWebSocketServer sender, IntPtr connId, byte[] data)
        {

        }

        /// <summary>
        /// pong 消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="connId"></param>
        /// <param name="data"></param>
        public virtual void OnPong(IWebSocketServer sender, IntPtr connId, byte[] data)
        {

        }
    }
}
