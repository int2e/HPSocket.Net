#if !NET20
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace HPSocket.AsyncQueue
{
    public class Worker<T> :IDisposable
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
        private CancellationTokenSource cts { get; set; }
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="collection">消费队列实体</param>
        /// <param name="taskProc">任务处理函数</param>
        /// <param name="ct"></param>
        public Worker(BlockingCollection<T> collection, Action<T> taskProc)
        {
            this.collection = collection ?? throw new ArgumentNullException(nameof(collection));
            this.taskProc = taskProc ?? throw new ArgumentNullException(nameof(taskProc));
            this.cts = new CancellationTokenSource();
            Task.Factory.StartNew(this.DoWork, cts.Token);
        }
        /// <summary>
        /// 消费数据方法
        /// </summary>
        private void DoWork()
        {
            while (!this.cts.Token.IsCancellationRequested)
            {
                try
                {
                    var item = this.collection.Take(this.cts.Token);
                    this.taskProc?.Invoke(item);
                }
                catch (OperationCanceledException)
                {
                    //令牌已取消
                }
            }
        }
        /// <summary>
        /// 停止线程工作
        /// </summary>
        public void Stop() => cts.Cancel();

        public void Dispose() => Stop();
    }
}
#endif