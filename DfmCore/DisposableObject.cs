using System;

namespace DfmCore
{
    public abstract class DisposableObject : IDisposable
    {
        private bool _disposed;
        private readonly object _lock = new object();

        public bool IsDisposed => this._disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~DisposableObject()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            lock (this._lock)
            {
                if (this._disposed)
                {
                    return;
                }

                this._disposed = true;
            }

            DisposeUnmanagedResources();

            if (disposing)
            {
                DisposeResources();
            }
        }

        protected void CheckDisposed()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(nameof(DisposableObject));
            }
        }

        protected virtual void DisposeResources() { }

        protected virtual void DisposeUnmanagedResources() { }
    }
}
