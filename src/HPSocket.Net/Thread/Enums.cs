namespace HPSocket.Thread
{
    /// <summary>
    /// 拒绝策略, 调用被拒绝后的处理策略
    /// </summary>
    public enum RejectedPolicy
    {
        /// <summary>
        /// 立刻返回失败
        /// </summary>
        CallFail = 0,

        /// <summary>
        /// 等待（直到成功、超时或线程池关闭等原因导致失败）
        /// </summary>
        WaitFor = 1,

        /// <summary>
        /// 调用者线程直接执行
        /// </summary>
        CallerRun = 2,
    }

    /// <summary>
    /// 任务缓冲区类型,  SocketTask 对象创建和销毁时，根据不同类型的缓冲区类型作不同的处理
    /// </summary>
    public enum TaskBufferType
    {
        /// <summary>
        /// 深拷贝
        /// </summary>
        Copy = 0,

        /// <summary>
        ///  浅拷贝
        /// </summary>
        Refer = 1,

        /// <summary>
        /// 连接（不负责创建，但负责销毁）
        /// </summary>
        Attach = 2,
    }
}
