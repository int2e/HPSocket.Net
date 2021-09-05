namespace HPSocket.Thread
{
    internal class ThreadProcExArgs
    {
        /// <summary>
        /// 任务参数
        /// </summary>
        public object Arg { get; set; }

        /// <summary>
        /// 任务回调地址
        /// </summary>
        public TaskProcEx TaskProc { get; set; }
    }
}
