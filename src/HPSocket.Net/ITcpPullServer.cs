using System;
using HPSocket.Tcp;

namespace HPSocket
{
    /// <summary>
    /// tcp pull server
    /// </summary>
    public interface ITcpPullServer : ITcpServer
    {
        #region 服务器事件

        /// <summary>
        /// 数据到达事件
        /// </summary>
        new event PullServerReceiveEventHandler OnReceive;

        #endregion


        #region 服务器方法

        /// <summary>
        /// 抓取数据，用户通过该方法从 Socket 组件中抓取数据
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="buffer"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        FetchResult Fetch(IntPtr connId, IntPtr buffer, int length);

        /// <summary>
        /// 抓取数据，用户通过该方法从 Socket 组件中抓取数据
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="length"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        FetchResult Fetch(IntPtr connId, int length, out byte[] bytes);

        /// <summary>
        /// 窥探数据（不会移除缓冲区数据），用户通过该方法从 Socket 组件中窥探数据
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="buffer"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        FetchResult Peek(IntPtr connId, IntPtr buffer, int length);

        /// <summary>
        /// 窥探数据（不会移除缓冲区数据），用户通过该方法从 Socket 组件中窥探数据
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="length"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        FetchResult Peek(IntPtr connId, int length, out byte[] bytes);
        #endregion

    }
}
