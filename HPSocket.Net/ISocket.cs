using System;

// ReSharper disable once CheckNamespace
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

        // 等待服务结束
        void Wait();
    }
}
