#if !NET20
using System;
using System.Collections.Concurrent;

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
        /// 任务处理函数
        /// </summary>
        private Action<T> taskProc { get; set; }
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
            this.workers = new ConcurrentBag<Worker<T>>();
            this.queue = new BlockingCollection<T>();
            this.taskProc = taskProc;
            for (var i = 0; i < consumerCount; i++)
            {
                workers.Add(new Worker<T>(queue, taskProc));
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
        /// 消费者数量
        /// </summary>
        public int ConsumerCount { get => this.workers.Count; }
        /// <summary>
        /// 添加消费者
        /// </summary>
        public void AddConsumer(uint consumerCount)
        {
            for (var i = 0; i < consumerCount; i++)
            {
                workers.Add(new Worker<T>(queue, taskProc));
            }
        }
        /// <summary>
        /// 删除消费者
        /// </summary>
        public void RemoveConsumer(uint consumerCount)
        {
            while (!this.workers.IsEmpty && consumerCount-- > 0)
            {
                if(workers.TryTake(out var w))
                {
                    w.Stop();
                }
            }
        }
        /// <summary>
        /// 停止消费
        /// </summary>
        public void Shutdown()
        {
            while (!this.workers.IsEmpty)
            {
                if(workers.TryTake(out var w))
                {
                    w.Stop();
                }
            }
        }
        public void Dispose() => Shutdown();
    }
}
#endif