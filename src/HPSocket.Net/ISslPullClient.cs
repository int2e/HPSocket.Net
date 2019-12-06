using System;
using HPSocket.Tcp;

namespace HPSocket
{
    /// <summary>
    /// ssl pull client
    /// </summary>
    public interface ISslPullClient : ISslClient
    {

        #region 客户端事件

        /// <summary>
        /// 数据到达事件
        /// </summary>
        new event PullClientReceiveEventHandler OnReceive;

        #endregion


        #region 客户端方法

        /// <summary>
        /// 抓取数据，用户通过该方法从 Socket 组件中抓取数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        FetchResult Fetch(IntPtr buffer, int length);

        /// <summary>
        /// 抓取数据，用户通过该方法从 Socket 组件中抓取数据
        /// </summary>
        /// <param name="length"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        FetchResult Fetch(int length, out byte[] bytes);

        /// <summary>
        /// 窥探数据（不会移除缓冲区数据），用户通过该方法从 Socket 组件中窥探数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        FetchResult Peek(IntPtr buffer, int length);

        /// <summary>
        /// 窥探数据（不会移除缓冲区数据），用户通过该方法从 Socket 组件中窥探数据
        /// </summary>
        /// <param name="length"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        FetchResult Peek(int length, out byte[] bytes);

        #endregion
    }
}
