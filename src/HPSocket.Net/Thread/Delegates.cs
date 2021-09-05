using System;

namespace HPSocket.Thread
{
    /// <summary>
    /// 任务处理函数
    /// <para>任务处理入口函数</para>
    /// </summary>
    /// <param name="arg">自定义参数</param>
    public delegate void TaskProc(IntPtr arg);

    /// <summary>
    /// 任务处理函数扩展板
    /// <para>任务处理入口函数</para>
    /// </summary>
    /// <param name="obj">自定义参数</param>
    public delegate void TaskProcEx(object obj);
    
    /// <summary>
    /// 线程池启动事件处理器
    /// </summary>
    /// <param name="threadPool">线程池对象</param>
    public delegate void StartupEventHandler(ThreadPool threadPool);

    /// <summary>
    /// 线程池关闭事件处理器
    /// </summary>
    /// <param name="threadPool">线程池对象</param>
    public delegate void ShutdownEventHandler(ThreadPool threadPool);

    /// <summary>
    /// 工作线程启动事件处理器(每个工作线程触发一次)
    /// </summary>
    /// <param name="threadPool">线程池对象</param>
    /// <param name="threadId">当前线程id</param>
    public delegate void WorkerThreadStartEventHandler(ThreadPool threadPool, ulong threadId);

    /// <summary>
    /// 工作线程退出事件处理器(每个工作线程触发一次)
    /// </summary>
    /// <param name="threadPool">线程池对象</param>
    /// <param name="threadId">当前线程id</param>
    public delegate void WorkerThreadEndEventHandler(ThreadPool threadPool, ulong threadId);

}
