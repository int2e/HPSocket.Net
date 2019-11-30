using System;
using System.Runtime.InteropServices;
using HPSocket.Thread;

namespace HPSocket.Sdk
{
    public class ThreadPool
    {
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr Create_HP_ThreadPool();

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void Destroy_HP_ThreadPool(IntPtr pThreadPool);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr Create_HP_SocketTaskObj(SocketTaskProc fnTaskProc, IntPtr pSender, IntPtr connId, IntPtr pBuffer, int iBuffLen, TaskBufferType enBuffType /*= TBT_COPY*/, IntPtr wParam /*= 0*/, IntPtr lParam /*= 0*/);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void Destroy_HP_SocketTaskObj(IntPtr pTask);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_ThreadPool_Start(IntPtr pThreadPool, int dwThreadCount /*= 0*/, uint dwMaxQueueSize /*= 0*/, RejectedPolicy enRejectedPolicy /*= TRP_CALL_FAIL*/, uint dwStackSize /*= 0*/);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_ThreadPool_Stop(IntPtr pThreadPool, int dwMaxWait /*= INFINITE*/);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_ThreadPool_Submit(IntPtr pThreadPool, TaskProc fnTaskProc, IntPtr pvArg, int dwMaxWait /*= INFINITE*/);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_ThreadPool_Submit_Task(IntPtr pThreadPool, IntPtr pTask, int dwMaxWait /*= INFINITE*/);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_ThreadPool_AdjustThreadCount(IntPtr pThreadPool, int dwNewThreadCount);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern bool HP_ThreadPool_HasStarted(IntPtr pThreadPool);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern ServiceState HP_ThreadPool_GetState(IntPtr pThreadPool);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_ThreadPool_GetQueueSize(IntPtr pThreadPool);
        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_ThreadPool_GetTaskCount(IntPtr pThreadPool);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern int HP_ThreadPool_GetThreadCount(IntPtr pThreadPool);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern uint HP_ThreadPool_GetMaxQueueSize(IntPtr pThreadPool);

        [DllImport(HpSocketLibrary.DllName, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern RejectedPolicy HP_ThreadPool_GetRejectedPolicy(IntPtr pThreadPool);
    }
}
