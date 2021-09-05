using System;

namespace HPSocket.Adapter
{
    /// <summary>
    /// 固定包头数据接收适配器基类
    /// </summary>
    /// <typeparam name="TRequestBodyType">包体解析对象类型</typeparam>
    public abstract class FixedHeaderDataReceiveAdapter<TRequestBodyType> : DataReceiveAdapter<TRequestBodyType>
    {
        #region 私有成员

        /// <summary>
        /// 包头长度, 组件在调用GetBodySize()方法时候会给定此长度的数据, 需要继承当前接口的类在构造函数中设置包头长度
        /// </summary>
        private readonly int _headerSize;

        /// <summary>
        ///  最大封包长度
        /// </summary>
        private readonly int _maxPacketSize;

        #endregion

        /// <summary>
        /// 固定包头数据接收适配器基类构造函数
        /// </summary>
        /// <param name="headerSize">包头长度</param>
        /// <param name="maxPacketSize">最大封包长度, 0.不限</param>
        protected FixedHeaderDataReceiveAdapter(int headerSize, int maxPacketSize)
        {
            if (headerSize < 1 || maxPacketSize < 0)
            {
                throw new InitializationException("参数headerSize必须大于0, 参数maxPacketSize必须大于或等于0");
            }
            _headerSize = headerSize;
            _maxPacketSize = maxPacketSize;
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
                    // 数据小于包头长度
                    if (cache.Data.Count < _headerSize)
                    {
                        return HandleResult.Ok;
                    }

                    // 包头
                    var header = cache.Data.GetRange(0, _headerSize).ToArray();

                    // 包体长度从适配器子类中获取
                    var bodySize = GetBodySize(header);

                    // 完整的包长度(含包头和完整数据的大小)
                    var fullSize = bodySize + _headerSize;

                    // 判断最大封包长度
                    if (_maxPacketSize != 0 && fullSize > _maxPacketSize)
                    {
                        // 超过断开
                        return HandleResult.Error;
                    }

                    // 如果来的数据小于一个完整的包
                    if (cache.Data.Count < fullSize)
                    {
                        // 下次数据到达处理
                        return HandleResult.Ignore;
                    }

                    // 如果来的数据比一个完整的包长
                    if (cache.Data.Count >= fullSize)
                    {
                        // 得到完整包并处理
                        var bodyData = cache.Data.GetRange(_headerSize, bodySize).ToArray();

                        // 包体解析对象从适配器子类中获取
                        var obj = ParseRequestBody(header, bodyData);

                        // 调用解析请求包体事件
                        result = parseRequestBody.Invoke(sender, connId, obj);
                        if (result == HandleResult.Error)
                        {
                            break;
                        }

                        // 再移除已经读了的数据
                        cache.Data.RemoveRange(0, fullSize);

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
        /// 获取包体长度
        /// </summary>
        /// <param name="header">包头, 其长度是HeaderLength属性指定的长度</param>
        /// <returns>需子类根据header自己解析包体实际长度并返回</returns>
        protected virtual int GetBodySize(byte[] header)
        {
            return 0;
        }

        /// <summary>
        /// 解析请求包体到对象
        /// </summary>
        /// <param name="header">包头</param>
        /// <param name="data">包体</param>
        /// <returns>需子类根据包体data自己解析对象并返回</returns>
        protected virtual TRequestBodyType ParseRequestBody(byte[] header, byte[] data)
        {
            return default;
        }
    }
}
