#if !NET20
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace HPSocket.AsyncQueue
{
    public class Worker<T>
    {
        /// <summary>
        /// 消费队列实体
        /// </summary>
        private readonly BlockingCollection<T> collection;
        /// <summary>
        /// 任务处理函数
        /// </summary>
        private readonly Action<T> taskProc;
        /// <summary>
        /// 控制线程令牌
        /// </summary>
        private CancellationToken ct { get; set; }
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="collection">消费队列实体</param>
        /// <param name="taskProc">任务处理函数</param>
        /// <param name="ct"></param>
        public Worker(BlockingCollection<T> collection, Action<T> taskProc, CancellationToken ct)
        {
            this.collection = collection ?? throw new ArgumentNullException(nameof(collection));
            this.taskProc = taskProc ?? throw new ArgumentNullException(nameof(taskProc));
            this.ct = ct;
            Task.Factory.StartNew(this.DoWork, ct);
        }
        /// <summary>
        /// 消费数据方法
        /// </summary>
        private void DoWork()
        {
            while (!this.ct.IsCancellationRequested)
            {
                try
                {
                    var item = this.collection.Take(this.ct);
                    this.taskProc?.Invoke(item);
                }
                catch (OperationCanceledException)
                {
                    //令牌已取消
                }
            }
        }
    }
}
#endif