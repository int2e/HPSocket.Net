#if !NET20 && !NET30 && !NET35
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace HPSocket.AsyncQueue
{
    public class Worker<T> : IDisposable
    {
        #region 私有成员

        /// <summary>
        /// 消费队列实体
        /// </summary>
        private readonly BlockingCollection<T> _collection;
        /// <summary>
        /// 任务处理函数
        /// </summary>
        private readonly Action<T> _taskProc;
        /// <summary>
        /// 控制线程令牌
        /// </summary>
        private readonly CancellationTokenSource _cts;

        #endregion

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="collection">消费队列实体</param>
        /// <param name="taskProc">任务处理函数</param>
        public Worker(BlockingCollection<T> collection, Action<T> taskProc)
        {
            this._collection = collection ?? throw new ArgumentNullException(nameof(collection));
            this._taskProc = taskProc ?? throw new ArgumentNullException(nameof(taskProc));
            this._cts = new CancellationTokenSource();
            Task.Factory.StartNew(this.DoWork, _cts.Token);
        }

        /// <summary>
        /// 消费数据方法
        /// </summary>
        private void DoWork()
        {
            while (!this._cts.Token.IsCancellationRequested)
            {
                try
                {
                    var item = this._collection.Take(this._cts.Token);
                    this._taskProc?.Invoke(item);
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
        public void Stop() => _cts.Cancel();

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose() => Stop();
    }
}
#endif