using System;
using HPSocket.Adapter;
using HPSocket.Ssl;

namespace HPSocket
{
    /// <summary>
    /// ssl client
    /// </summary>
    /// <typeparam name="TRequestBodyType">包体解析对象类型</typeparam>
    public interface ISslClient<TRequestBodyType> : ISslClient
    {
        [Obsolete("adapter组件无需添加OnReceive事件, 请添加OnParseRequestBody事件", true)]
        new event ClientReceiveEventHandler OnReceive;

        /// <summary>
        /// 解析请求包体对象事件
        /// </summary>
        event ParseRequestBody<ISslClient, TRequestBodyType> OnParseRequestBody;

        /// <summary>
        /// 数据接收适配器
        /// </summary>
        DataReceiveAdapter<TRequestBodyType> DataReceiveAdapter { get; set; }
    }


    /// <summary>
    /// ssl client
    /// </summary>
    public interface ISslClient : ISsl, ITcpClient
    {

        /// <summary>
        /// 启动 SSL 握手，当通信组件设置为非自动握手时，需要调用本方法启动 SSL 握手
        /// </summary>
        /// <returns></returns>
        bool StartHandShake();

        /// <summary>
        /// 获取指定类型的 SSL WebSocketSession 信息（输出类型参考：SslSessionInfo）
        /// </summary>
        /// <param name="info"></param>
        /// <param name="sessionInfo"></param>
        /// <returns></returns>
        bool GetSessionInfo(SslSessionInfo info, out IntPtr sessionInfo);
    }
}
