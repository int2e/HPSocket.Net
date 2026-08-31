using System;

using Timer = System.Timers.Timer;

namespace HPSocket
{
    internal sealed class ConnectionTimeoutTimer : IDisposable
    {
        private readonly object _syncRoot = new object();
        private bool _disposed;

        internal ConnectionTimeoutTimer(NativeExtra nativeExtra, int timeout)
        {
            NativeExtra = nativeExtra;
            Timer = new Timer(timeout)
            {
                AutoReset = false
            };
        }

        internal NativeExtra NativeExtra { get; }

        internal Timer Timer { get; }

        internal void Start()
        {
            lock (_syncRoot)
            {
                if (!_disposed)
                {
                    Timer.Start();
                }
            }
        }

        public void Dispose()
        {
            lock (_syncRoot)
            {
                if (_disposed)
                {
                    return;
                }

                _disposed = true;
                Timer.Stop();
                Timer.Dispose();
            }
        }
    }
}
