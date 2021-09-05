using System;
namespace HPSocket.Adapter
{
    /// <summary>
    /// 结束符数据接收适配器基类
    /// </summary>
    /// <typeparam name="TRequestBodyType">包体解析对象类型</typeparam>

    public abstract class TerminatorDataReceiveAdapter<TRequestBodyType> : DataReceiveAdapter<TRequestBodyType>
    {
        #region 私有成员

        /// <summary>
        /// Boyer Moore
        /// <remarks>因为同一个连接不会同时触发OnReceive回调, 所以可以这么玩</remarks>
        /// </summary>
        private readonly BoyerMoore _boyerMoore;

        #endregion

        /// <summary>
        /// 结束符数据接收适配器基类构造函数
        /// </summary>
        /// <param name="terminator">结束符</param>
        protected TerminatorDataReceiveAdapter(byte[] terminator)
        {
            _boyerMoore = new BoyerMoore(terminator);
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

                // 在data中搜索terminator
                var findList = _boyerMoore.SearchAll(data);
                if (findList.Count == 0)
                {
                    // 没找到等下次
                    return HandleResult.Ok;
                }

                // 终止符长度
                var terminatorLength = _boyerMoore.PatternLength;

                // 最后位置
                var lastPosition = 0;

                foreach (var index in findList)
                {
                    // 数量
                    var bodyLength = index - lastPosition;

                    // 得到一条完整数据包
                    var bodyData = cache.Data.GetRange(lastPosition, bodyLength).ToArray();

                    // 记录最后位置
                    lastPosition += bodyLength + terminatorLength;

                    // 包体解析对象从适配器子类中获取
                    var obj = ParseRequestBody(bodyData);

                    // 调用解析请求包体事件
                    if (parseRequestBody.Invoke(sender, connId, obj) == HandleResult.Error)
                    {
                        return HandleResult.Error;
                    }
                }

                if (lastPosition > 0)
                {
                    // 清除已使用缓存
                    cache.Data.RemoveRange(0, lastPosition);
                }

                return HandleResult.Ignore;
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
