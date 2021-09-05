using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
#if !NET20 && !NET30 && !NET35
using System.Threading.Tasks;
#endif


namespace HPSocket.Thread
{
    /// <summary>
    /// 线程池
    /// </summary>
    public class ThreadPool : IDisposable
    {
        #region 保护成员

        /// <summary>
        /// 是否释放了
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// 线程池指针
        /// </summary>
        private readonly IntPtr _pool;

        /// <summary>
        /// 线程池事件监听器
        /// </summary>
        private readonly IntPtr _listener;

        /// <summary>
        /// 附加数据, 用于管理扩展板回调函数参数
        /// </summary>
        private readonly ExtraData<string, ThreadProcExArgs> _extraData = new ExtraData<string, ThreadProcExArgs>();

        /// <summary>
        /// 任务回调函数, 用于产生扩展板回调函数
        /// </summary>
        private readonly TaskProc _taskProc;

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly Sdk.ThreadPoolOnStartup _onStartup;
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly Sdk.ThreadPoolOnShutdown _onShutdown;
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly Sdk.ThreadPoolOnWorkerThreadStart _onWorkerThreadStart;
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly Sdk.ThreadPoolOnWorkerThreadEnd _onWorkerThreadEnd;
        #endregion

#if !NET20 && !NET30 && !NET35
        /// <summary>
        /// 系统错误码
        /// </summary>
        public ThreadLocal<int> SysErrorCode { get; private set; }
#endif
        /// <summary>
        /// 线程池启动事件
        /// </summary>
        public event StartupEventHandler OnStartup;
        /// <summary>
        /// 线程池关闭事件
        /// </summary>
        public event ShutdownEventHandler OnShutdown;
        /// <summary>
        /// 工作线程启动事件(每个工作线程触发一次)
        /// </summary>
        public event WorkerThreadStartEventHandler OnWorkerThreadStart;
        /// <summary>
        /// 工作线程退出事件(每个工作线程触发一次)
        /// </summary>
        public event WorkerThreadEndEventHandler OnWorkerThreadEnd;

        public ThreadPool()
        {
#if !NET20 && !NET30 && !NET35
            SysErrorCode = new ThreadLocal<int>(() => System.Threading.Thread.CurrentThread.ManagedThreadId);
#endif
            _taskProc = MyTaskProc;

            GC.KeepAlive(_taskProc);


            _listener = Sdk.ThreadPool.Create_HP_ThreadPoolListener();
            if (_listener != IntPtr.Zero)
            {
                _onStartup = SdkOnStartup;
                _onShutdown = SdkOnShutdown;
                _onWorkerThreadStart = SdkOnWorkerThreadStart;
                _onWorkerThreadEnd = SdkOnWorkerThreadEnd;
                GC.KeepAlive(_onStartup);
                GC.KeepAlive(_onShutdown);
                GC.KeepAlive(_onWorkerThreadStart);
                GC.KeepAlive(_onWorkerThreadEnd);
                Sdk.ThreadPool.HP_Set_FN_ThreadPool_OnStartup(_listener, _onStartup);
                Sdk.ThreadPool.HP_Set_FN_ThreadPool_OnShutdown(_listener, _onShutdown);
                Sdk.ThreadPool.HP_Set_FN_ThreadPool_OnWorkerThreadStart(_listener, _onWorkerThreadStart);
                Sdk.ThreadPool.HP_Set_FN_ThreadPool_OnWorkerThreadEnd(_listener, _onWorkerThreadEnd);

            }
            _pool = Sdk.ThreadPool.Create_HP_ThreadPool(_listener);
            if (_pool == IntPtr.Zero)
            {
                throw new InitializationException("创建线程池对象失败");
            }
        }

        ~ThreadPool() => Dispose(false);

        private void Destroy()
        {
            Stop();
            if (_listener != IntPtr.Zero)
            {
                Sdk.ThreadPool.Destroy_HP_ThreadPoolListener(_listener);
            }
            if (_pool != IntPtr.Zero)
            {
                Sdk.ThreadPool.Destroy_HP_ThreadPool(_pool);
            }
        }

        private void SdkOnStartup(IntPtr threadPoolPtr)
        {
            OnStartup?.Invoke(this);
        }

        private void SdkOnShutdown(IntPtr threadPoolPtr)
        {
            OnShutdown?.Invoke(this);
        }

        private void SdkOnWorkerThreadStart(IntPtr threadPoolPtr, IntPtr threadId)
        {
            //OnWorkerThreadStart?.Invoke(this, System.Threading.Thread.CurrentThread.ManagedThreadId);
            OnWorkerThreadStart?.Invoke(this, (ulong)threadId);
        }

        private void SdkOnWorkerThreadEnd(IntPtr threadPoolPtr, IntPtr threadId)
        {
            // OnWorkerThreadEnd?.Invoke(this, System.Threading.Thread.CurrentThread.ManagedThreadId);
            OnWorkerThreadEnd?.Invoke(this, (ulong)threadId);
        }
        

        /// <summary>
        /// 启动线程池组件
        /// </summary>
        /// <param name="threadCount">线程数量, 大于0: dwThreadCount, 等于0: (CPU核数* 2 + 2), 小于0: (CPU核数* (-threadCount))</param>
        /// <param name="policy">任务拒绝处理策略</param>
        /// <param name="maxQueueSize">任务队列最大容量（0：不限制，默认：0）</param>
        /// <param name="stackSize">线程堆栈空间大小（默认：0 -> 操作系统默认）</param>
        /// <returns>true: 成功, false: 失败，可通过 SysErrorCode 属性 获取系统错误代码</returns>
        public bool Start(int threadCount, RejectedPolicy policy, uint maxQueueSize = 0, uint stackSize = 0)
        {
            var ok = Sdk.ThreadPool.HP_ThreadPool_Start(_pool, threadCount, maxQueueSize, policy, stackSize);
#if !NET20 && !NET30 && !NET35
            SysErrorCode.Value = ok ? 0 : Sdk.Sys.SYS_GetLastError();
#endif
            return ok;
        }

        /// <summary>
        /// 在规定时间内关闭线程池组件，如果工作线程在最大等待时间内未能正常关闭，会尝试强制关闭，这种情况下很可能会造成系统资源泄漏
        /// </summary>
        /// <param name="maxWait">最大等待时间（毫秒，默认：INFINITE即-1，一直等待）</param>
        /// <returns>true: 成功, false: 失败，可通过 SysErrorCode 属性 获取系统错误代码</returns>
        public bool Stop(int maxWait = -1)
        {
            if (!HasStarted)
            {
                return false;
            }
            var ok = Sdk.ThreadPool.HP_ThreadPool_Stop(_pool, maxWait);
#if !NET20 && !NET30 && !NET35
            SysErrorCode.Value = ok ? 0 : Sdk.Sys.SYS_GetLastError();
#endif
            return ok;
        }

        ///// <summary>
        ///// 向线程池提交异步任务
        ///// </summary>
        ///// <param name="taskProc">任务处理函数</param>
        ///// <param name="args">任务参数</param>
        ///// <param name="maxWait">最大等待时间（毫秒，默认：INFINITE即-1，一直等待）</param>
        ///// <returns>true: 成功, false: 失败，可通过 SysErrorCode 属性 获取系统错误代码，其中，错误码 ERROR_DESTINATION_ELEMENT_FULL 表示任务队列已满</returns>
        //public bool Submit(TaskProc taskProc, IntPtr args, int maxWait = -1) => Sdk.ThreadPool.HP_ThreadPool_Submit(_pool, taskProc, args, maxWait);

        /// <summary>
        /// 向线程池提交异步任务
        /// </summary>
        /// <param name="taskProc">任务处理函数</param>
        /// <param name="obj">任务参数</param>
        /// <param name="maxWait">最大等待时间（毫秒，默认：INFINITE即-1，一直等待）</param>
        /// <returns>true: 成功, false: 失败，可通过 SysErrorCode 属性 获取系统错误代码，其中，错误码 ERROR_DESTINATION_ELEMENT_FULL 表示任务队列已满</returns>
        public bool Submit(TaskProcEx taskProc, object obj, int maxWait = -1)
        {
            var guid = Guid.NewGuid().ToString("N");
            var args = new ThreadProcExArgs
            {
                Arg = obj,
                TaskProc = taskProc,
            };
            if (!_extraData.Set(guid, args))
            {
                return false;
            }

            var bytes = Encoding.ASCII.GetBytes(guid);
            var gch = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            var ok = Sdk.ThreadPool.HP_ThreadPool_Submit(_pool, _taskProc, (IntPtr)gch, maxWait);
#if !NET20 && !NET30 && !NET35
            SysErrorCode.Value = ok ? 0 : Sdk.Sys.SYS_GetLastError();
#endif
            return ok;
        }

        /// <summary>
        /// TaskProc 转 TaskProcEx
        /// </summary>
        /// <param name="ptr"></param>
        private void MyTaskProc(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero) return;

            var gch = GCHandle.FromIntPtr(ptr);
            var bytes = (byte[])gch.Target;
            gch.Free();

            // ReSharper disable once AssignNullToNotNullAttribute
            var guid = Encoding.ASCII.GetString(bytes);
            var args = _extraData.Get(guid);
            if (args == null) return;

            _extraData.Remove(guid);

            args.TaskProc.Invoke(args.Arg);
        }

        ///// <summary>
        ///// 创建 TSocketTask 对象
        ///// 创建任务对象，该对象最终需由 DestroySocketTask() 销毁
        ///// </summary>
        ///// <param name="taskProc">任务入口函数</param>
        ///// <param name="sender">发起对象, 如server, client,agent对象</param>
        ///// <param name="connId">连接id</param>
        ///// <param name="buffer">数据</param>
        ///// <param name="bufferSize">数据长度</param>
        ///// <param name="taskBufferType">数据类型</param>
        ///// <param name="wParam">自定义参数</param>
        ///// <param name="lParam">自定义参数</param>
        ///// <returns>true: 成功, false: 失败，可通过 SysErrorCode 属性 获取系统错误代码，其中，错误码 ERROR_DESTINATION_ELEMENT_FULL 表示任务队列已满</returns>
        //public static IntPtr CreateSocketTask(SocketTaskProc taskProc, IntPtr sender, IntPtr connId, byte[] buffer, int bufferSize, TaskBufferType taskBufferType, IntPtr wParam, IntPtr lParam)
        //{
        //    var gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);
        //    var ptr = Sdk.ThreadPool.Create_HP_SocketTaskObj(taskProc, sender, connId, gch.AddrOfPinnedObject(), bufferSize, taskBufferType, wParam, lParam);
        //    gch.Free();
        //    return ptr;
        //}

        ///// <summary>
        ///// 销毁 TSocketTask 对象
        ///// </summary>
        ///// <param name="task"></param>
        //public static void DestroySocketTask(IntPtr task)
        //{
        //    Sdk.ThreadPool.Destroy_HP_SocketTaskObj(task);
        //}

        ///// <summary>
        /////  提交 Socket 任务
        /////  <para>向线程池提交异步 Socket 任务</para>
        ///// </summary>
        ///// <param name="task">任务参数 SocketTask</param>
        ///// <param name="maxWait">最大等待时间（毫秒，默认：INFINITE即-1，一直等待）</param>
        ///// <returns>true: 成功, false: 失败，可通过 SysErrorCode 属性 获取系统错误代码，其中，错误码 ERROR_DESTINATION_ELEMENT_FULL 表示任务队列已满</returns>
        //public bool SubmitSocketTask(IntPtr task, int maxWait = -1) => Sdk.ThreadPool.HP_ThreadPool_Submit_Task(_pool, task, maxWait);


        ///// <summary>
        ///// 创建 TSocketTask 对象并向线程池提交异步 Socket 任务
        ///// </summary>
        ///// <param name="socketTaskProc">任务入口函数</param>
        ///// <param name="sender">发起对象, 如server, client, agent对象</param>
        ///// <param name="connId">连接id</param>
        ///// <param name="buffer">数据</param>
        ///// <param name="bufferSize">数据长度</param>
        ///// <param name="taskBufferType">数据类型</param>
        ///// <param name="wParam">自定义参数</param>
        ///// <param name="lParam">自定义参数</param>
        ///// <param name="maxWait">最大等待时间（毫秒，默认：INFINITE即-1，一直等待）</param>
        ///// <returns>true: 成功, false: 失败，可通过 SysErrorCode 属性 获取系统错误代码，其中，错误码 ERROR_DESTINATION_ELEMENT_FULL 表示任务队列已满</returns>
        //public bool SubmitSocketTask(SocketTaskProc socketTaskProc, IntPtr sender, IntPtr connId, byte[] buffer, int bufferSize, TaskBufferType taskBufferType, IntPtr wParam, IntPtr lParam, int maxWait = -1)
        //{
        //    var task = CreateSocketTask(socketTaskProc, sender, connId, buffer, bufferSize, taskBufferType, wParam, lParam);
        //    if (task == IntPtr.Zero)
        //    {
        //        return false;
        //    }

        //    var ok = SubmitSocketTask(task, maxWait);
        //    if (!ok)
        //    {
        //        DestroySocketTask(task);
        //    }

        //    return ok;
        //}

        /// <summary>
        /// 增加或减少线程池的工作线程数量
        /// </summary>
        /// <param name="count">线程数量， 大于0: count， 等于0: (CPU核数 * 2 + 2)， 小于0: (CPU核数 * (-count))</param>
        /// <returns>true: 成功, false: 失败，可通过 SysErrorCode 属性 获取系统错误代码</returns>
        // ReSharper disable once UnusedMethodReturnValue.Local
        private bool AdjustThreadCount(int count)
        {
            var ok = Sdk.ThreadPool.HP_ThreadPool_AdjustThreadCount(_pool, count);
#if !NET20 && !NET30 && !NET35
            SysErrorCode.Value = ok ? 0 : Sdk.Sys.SYS_GetLastError();
#endif
            return ok;
        }

        /// <summary>
        /// 等待线程池组件停止运行
        /// </summary>
        /// <param name="milliseconds"></param>
        /// <returns></returns>
        public bool Wait(int milliseconds = -1) => Sdk.ThreadPool.HP_ThreadPool_Wait(_pool, milliseconds);

#if !NET20 && !NET30 && !NET35
        /// <summary>
        /// 等待线程池组件停止运行
        /// </summary>
        /// <param name="milliseconds"></param>
        /// <returns></returns>
        public Task<bool> WaitAsync(int milliseconds = -1)
        {
            return Task.Factory.StartNew((obj) => Wait((int)obj), milliseconds);
        }

        /// <summary>
        /// 在规定时间内关闭线程池组件，如果工作线程在最大等待时间内未能正常关闭，会尝试强制关闭，这种情况下很可能会造成系统资源泄漏
        /// </summary>
        /// <param name="maxWait">最大等待时间（毫秒，默认：INFINITE即-1，一直等待）</param>
        /// <returns>true: 成功, false: 失败，可通过 SysErrorCode 属性 获取系统错误代码</returns>
        public Task<bool> StopAsync(int maxWait = -1)
        {
            return Task.Factory.StartNew((obj) => Stop((int)obj), maxWait);
        }
#endif

        /// <summary>
        /// 获取当前正在执行的任务数量
        /// </summary>
        public uint TaskCount => Sdk.ThreadPool.HP_ThreadPool_GetTaskCount(_pool);

        /// <summary>
        /// 获取或设置线程池数量
        /// <para>设置线程池数量时, 大于0: count, 等于0: (CPU核数 * 2 + 2), 小于0: (CPU核数 * (-count))</para>
        /// </summary>
        /// <returns></returns>
        public int ThreadCount
        {
            get => Sdk.ThreadPool.HP_ThreadPool_GetThreadCount(_pool);
            set => AdjustThreadCount(value);
        }

        /// <summary>
        /// 检查线程池组件是否已启动
        /// </summary>
        /// <returns></returns>
        public bool HasStarted => Sdk.ThreadPool.HP_ThreadPool_HasStarted(_pool);

        /// <summary>
        /// 查看线程池组件当前状态
        /// </summary>
        public ServiceState State => Sdk.ThreadPool.HP_ThreadPool_GetState(_pool);

        /// <summary>
        /// 获取当前任务队列大小
        /// </summary>
        public uint QueueSize => Sdk.ThreadPool.HP_ThreadPool_GetQueueSize(_pool);

        /// <summary>
        /// 获取任务队列最大容量
        /// </summary>
        public uint MaxQueueSize => Sdk.ThreadPool.HP_ThreadPool_GetMaxQueueSize(_pool);

        /// <summary>
        /// 获取任务拒绝处理策略
        /// </summary>
        public RejectedPolicy RejectedPolicy => Sdk.ThreadPool.HP_ThreadPool_GetRejectedPolicy(_pool);

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                // 释放托管对象资源
                _extraData.Clear();
#if !NET20 && !NET30 && !NET35
                SysErrorCode?.Dispose();
#endif
            }
            Destroy();

            _disposed = true;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
