using System;
#if !NET20 && !NET30 && !NET35
using System.Threading.Tasks;
#endif

namespace HPSocket
{
    /// <summary>
    /// 所有组件的基接口
    /// </summary>
    public interface ISocket : IDisposable
    {
        /// <summary>
        /// 组件原始指针
        /// <para>比如在 hp-socket 的线程池中需要使用</para>
        /// </summary>
        IntPtr SenderPtr { get; }

        /// <summary>
        /// 当前组件版本
        /// </summary>
        string Version { get; }

        /// <summary>
        /// 等待通信组件停止运行
        /// <para>可用在控制台程序, 用来阻塞主线程, 防止程序退出</para>
        /// </summary>
        /// <param name="milliseconds">超时时间（毫秒，默认：-1，永不超时）</param>
        bool Wait(int milliseconds = -1);

#if !NET20 && !NET30 && !NET35
        /// <summary>
        /// 等待通信组件停止运行
        /// <para>可用在控制台程序, 用来阻塞主线程, 防止程序退出</para>
        /// </summary>
        /// <param name="milliseconds">超时时间（毫秒，默认：-1，永不超时）</param>
        Task<bool> WaitAsync(int milliseconds = -1);

        /// <summary>
        /// 停止服务
        /// </summary>
        Task<bool> StopAsync();
#endif
    }
}
