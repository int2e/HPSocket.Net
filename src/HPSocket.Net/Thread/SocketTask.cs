using System;
using System.Runtime.InteropServices;
using HPSocket.Sdk;

namespace HPSocket.Thread
{
    /// <summary>
    /// Socket 任务结构体, 封装 Socket 任务相关数据结构
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SocketTask
    {
        /// <summary>
        /// 任务处理函数
        /// </summary>
        public SocketTaskProc SocketTaskProc;
        /// <summary>
        /// 发起对象
        /// </summary>
        public IntPtr Sender;
        /// <summary>
        /// 连接 Id
        /// </summary>
        public IntPtr ConnId;
        /// <summary>
        /// 数据缓冲区
        /// </summary>
        public IntPtr Buffer;
        /// <summary>
        /// 数据缓冲区长度
        /// </summary>
        public int BufferSize;
        /// <summary>
        /// 缓冲区类型
        /// </summary>
        public TaskBufferType BufferType;
        /// <summary>
        /// 自定义参数
        /// </summary>
        public IntPtr WParam;
        /// <summary>
        /// 自定义参数
        /// </summary>
        public IntPtr LParam;
    }
}
