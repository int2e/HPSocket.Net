using System;
using System.Collections.Generic;

namespace HPSocket.Adapter
{
    /// <summary>
    /// 数据接收适配器基类
    /// </summary>
    /// <typeparam name="TRequestBodyType">包体解析对象类型</typeparam>
    public abstract class DataReceiveAdapter<TRequestBodyType>
    {
        #region 保护成员

        /// <summary>
        /// 数据接收适配器信息
        /// </summary>
        protected readonly ExtraData<IntPtr, DataReceiveAdapterInfo> DataReceiveAdapterCache = new ExtraData<IntPtr, DataReceiveAdapterInfo>();

        #endregion

        /// <summary>
        /// 打开连接
        /// </summary>
        /// <param name="connId"></param>
        internal virtual void OnOpen(IntPtr connId)
        {
            DataReceiveAdapterCache.Set(connId, new DataReceiveAdapterInfo
            {
                ConnId = connId,
                Data = new List<byte>(),
            });
        }

        /// <summary>
        /// 数据到达
        /// </summary>
        /// <typeparam name="TSender"></typeparam>
        /// <param name="sender"></param>
        /// <param name="connId"></param>
        /// <param name="data"></param>
        /// <param name="parseRequestBody"></param>
        /// <returns></returns>
        internal virtual HandleResult OnReceive<TSender>(TSender sender, IntPtr connId, byte[] data, ParseRequestBody<TSender, TRequestBodyType> parseRequestBody)
        {
            return HandleResult.Ignore;
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <param name="connId"></param>
        internal virtual void OnClose(IntPtr connId)
        {
            DataReceiveAdapterCache.Remove(connId);
        }
    }
}
