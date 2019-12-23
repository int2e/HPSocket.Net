#if !NET20
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace HPSocket.AsyncQueue
{
    /// <summary>
    /// 多消费者异步消费队列
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AsyncQueue<T> : IDisposable
    {
        #region 私有
        /// <summary>
        /// 数据操作类
        /// </summary>
        private ConcurrentBag<Worker<T>> workers { get; set; }
        /// <summary>
        /// 消费队列实体
        /// </summary>
        private BlockingCollection<T> queue { get; set; }
        /// <summary>
        /// 控制线程令牌源
        /// </summary>
        private CancellationTokenSource cts { get; set; }
        #endregion
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="consumerCount">消费者数量</param>
        /// <param name="taskProc">任务处理函数</param>
        public AsyncQueue(uint consumerCount, Action<T> taskProc)
        {
            if (consumerCount == 0) { throw new ArgumentException($"{nameof(consumerCount)} must be > 0"); }
            if (taskProc == null) { throw new ArgumentNullException($"{nameof(taskProc)} must not be null"); }
            workers = new ConcurrentBag<Worker<T>>();
            queue = new BlockingCollection<T>();
            cts = new CancellationTokenSource();
            for (var i = 0; i < consumerCount; i++)
            {
                workers.Add(new Worker<T>(queue, taskProc, cts.Token));
            }
        }
        /// <summary>
        /// 剩余数据数量
        /// </summary>
        /// <returns></returns>
        public int Count { get => this.queue.Count; }
        /// <summary>
        /// 队列添加数据
        /// </summary>
        /// <param name="item">数据实体</param>
        /// <returns>成功返回true，失败返回false</returns>
        public bool Enqueue(T item) => this.queue.TryAdd(item);
        /// <summary>
        /// 停止消费
        /// </summary>
        public void Shutdown()
        {
            this.cts.Cancel();
            while (!this.workers.IsEmpty)
            {
                Worker<T> w;
                this.workers.TryTake(out w);
            }
        }
        public void Dispose() => Shutdown();
    }
}
#endif