#if !NET20 && !NET30 && !NET35
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
        #region 私有成员

        /// <summary>
        /// 数据操作类
        /// </summary>
        private readonly ConcurrentBag<Worker<T>> _workers;
        /// <summary>
        /// 消费队列实体
        /// </summary>
        private readonly BlockingCollection<T> _queue;
        /// <summary>
        /// 任务处理函数
        /// </summary>
        private readonly Action<T> _taskProc;

        #endregion

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="consumerCount">消费者数量</param>
        /// <param name="taskProc">任务处理函数</param>
        public AsyncQueue(uint consumerCount, Action<T> taskProc)
        {
            if (consumerCount == 0)
            {
                throw new ArgumentException($"{nameof(consumerCount)} must be > 0");
            }

            _taskProc = taskProc ?? throw new ArgumentNullException($"{nameof(taskProc)} must not be null");
            _workers = new ConcurrentBag<Worker<T>>();
            _queue = new BlockingCollection<T>();
            for (var i = 0; i < consumerCount; i++)
            {
                _workers.Add(new Worker<T>(_queue, taskProc));
            }
        }

        /// <summary>
        /// 剩余数据数量
        /// </summary>
        /// <returns></returns>
        public int Count => _queue.Count;

        /// <summary>
        /// 队列添加数据
        /// </summary>
        /// <param name="item">数据实体</param>
        /// <returns>成功返回true，失败返回false</returns>
        public bool Enqueue(T item) => _queue.TryAdd(item);

        /// <summary>
        /// 消费者数量
        /// </summary>
        public int ConsumerCount => _workers.Count;

        /// <summary>
        /// 添加消费者
        /// </summary>
        public void AddConsumer(uint consumerCount)
        {
            for (var i = 0; i < consumerCount; i++)
            {
                _workers.Add(new Worker<T>(_queue, _taskProc));
            }
        }

        /// <summary>
        /// 删除消费者
        /// </summary>
        public void RemoveConsumer(uint consumerCount)
        {
            while (!_workers.IsEmpty && consumerCount-- > 0)
            {
                if (_workers.TryTake(out var w))
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
            while (!this._workers.IsEmpty)
            {
                if (_workers.TryTake(out var w))
                {
                    w.Stop();
                }
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose() => Shutdown();
    }
}

#endif