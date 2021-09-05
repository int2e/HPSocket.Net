using System;

namespace HPSocket.Adapter
{
    /// <summary>
    /// 定长数据接收适配器基类
    /// </summary>
    /// <typeparam name="TRequestBodyType">包体解析对象类型</typeparam>
    public abstract class FixedSizeDataReceiveAdapter<TRequestBodyType> : DataReceiveAdapter<TRequestBodyType>
    {
        #region 私有成员

        /// <summary>
        /// 封包长度
        /// </summary>
        private readonly int _packetSize;

        #endregion

        /// <summary>
        /// 定长数据接收适配器基类构造函数
        /// </summary>
        /// <param name="packetSize">包长</param>
        protected FixedSizeDataReceiveAdapter(int packetSize)
        {
            _packetSize = packetSize;
        }

        /// <inheritdoc />
        internal override HandleResult OnReceive<TSender>(TSender sender, IntPtr connId, byte[] data, ParseRequestBody<TSender, TRequestBodyType> parseRequestBody)
        {
            try
            {
                var cache = DataReceiveAdapterCache.Get(connId);
                if (cache == null)
                {
                    return HandleResult.Error;
                }

                cache.Data.AddRange(data);

                HandleResult result;
                do
                {
                    // 如果来的数据小于一个完整的包
                    if (cache.Data.Count < _packetSize)
                    {
                        // 下次数据到达处理
                        return HandleResult.Ignore;
                    }

                    // 如果来的数据比一个完整的包长
                    if (cache.Data.Count >= _packetSize)
                    {
                        // 得到完整包并处理
                        var bodyData = cache.Data.GetRange(0, _packetSize).ToArray();

                        // 包体解析对象从适配器子类中获取
                        var obj = ParseRequestBody(bodyData);

                        // 调用解析请求包体事件
                        result = parseRequestBody.Invoke(sender, connId, obj);
                        if (result == HandleResult.Error)
                        {
                            break;
                        }

                        // 再移除已经读了的数据
                        cache.Data.RemoveRange(0, _packetSize);

                        // 没有数据了就返回了
                        if (cache.Data.Count == 0)
                        {
                            break;
                        }

                        // 剩余的数据下个循环处理
                    }
                } while (true);

                return result;
            }
            catch (Exception/* ex*/)
            {
                return HandleResult.Error;
            }
        }

        /// <summary>
        /// 解析请求包体到对象
        /// </summary>
        /// <param name="data">包体</param>
        /// <returns>需子类根据包体data自己解析对象并返回</returns>
        protected virtual TRequestBodyType ParseRequestBody(byte[] data)
        {
            return default;
        }
    }
}
