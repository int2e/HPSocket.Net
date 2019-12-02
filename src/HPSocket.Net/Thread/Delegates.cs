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
    /// Socket 任务处理函数
    /// <para>Socket 任务处理入口函数</para>
    /// </summary>
    /// <param name="task">task -- Socket 任务结构体指针</param>
    public delegate void SocketTaskProc(ref SocketTask task);
}
