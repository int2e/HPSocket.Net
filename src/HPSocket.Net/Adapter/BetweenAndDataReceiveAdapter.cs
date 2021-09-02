using System;
namespace HPSocket.Adapter
{
    /// <summary>
    /// 区间数据接收适配器基类
    /// </summary>
    /// <typeparam name="TRequestBodyType">包体解析对象类型</typeparam>

    public abstract class BetweenAndDataReceiveAdapter<TRequestBodyType> : DataReceiveAdapter<TRequestBodyType>
    {
        #region 私有成员

        /// <summary>
        /// Start Boyer Moore
        /// <remarks>因为同一个连接不会同时触发OnReceive回调, 所以可以这么玩</remarks>
        /// </summary>
        private readonly BoyerMoore _startBoyerMoore;

        /// <summary>
        /// End Boyer Moore
        /// <remarks>因为同一个连接不会同时触发OnReceive回调, 所以可以这么玩</remarks>
        /// </summary>
        private readonly BoyerMoore _endBoyerMoore;
        #endregion

        /// <summary>
        /// 区间数据接收适配器基类构造函数
        /// </summary>
        /// <param name="start">区间起始</param>
        /// <param name="end">区间结束</param>
        protected BetweenAndDataReceiveAdapter(byte[] start, byte[] end)
        {
            _startBoyerMoore = new BoyerMoore(start);
            _endBoyerMoore = new BoyerMoore(end);
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
                if (cache.Data.Count > data.Length)
                {
                    data = cache.Data.ToArray();
                }

                var startLength = _startBoyerMoore.PatternLength;
                var endLength = _endBoyerMoore.PatternLength;

                HandleResult result;
                var lastPosition = 0;
                do
                {
                    // 长度不够一个包等下次再来
                    if (data.Length - lastPosition < startLength + endLength)
                    {
                        result = HandleResult.Ok;
                        break;
                    }

                    // 搜索起始标志
                    var startPosition = _startBoyerMoore.Search(data, lastPosition);

                    // 是否找到了
                    if (startPosition == -1)
                    {
                        result = HandleResult.Error;
                        break;
                    }

                    startPosition = lastPosition + startPosition + startLength;

                    // 搜索结束标志, 从起始位置+起始标志长度开始找
                    var count = _endBoyerMoore.Search(data, startPosition);
                    if (count == -1)
                    {
                        result = HandleResult.Ignore;
                        break;
                    }

                    // 得到一条完整数据包
                    var bodyData = cache.Data.GetRange(startPosition, count).ToArray();
                    lastPosition += count + startLength + endLength;

                    // 包体解析对象从适配器子类中获取
                    var obj = ParseRequestBody(bodyData);

                    // 调用解析请求包体事件
                    if (parseRequestBody.Invoke(sender, connId, obj) == HandleResult.Error)
                    {
                        result = HandleResult.Error;
                        break;
                    }

                    // 下次继续解析

                } while (true);

                if (lastPosition > 0)
                {
                    // 再移除已经读了的数据
                    cache.Data.RemoveRange(0, lastPosition);
                }

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
