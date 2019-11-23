using System;

namespace HPSocket
{
    /// <summary>
    /// 初始化异常
    /// </summary>
    public class InitializationException : Exception
    {
        public InitializationException(string msg)
            : base(msg)
        {

        }
    }
}
