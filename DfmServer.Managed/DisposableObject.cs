using System;

namespace DfmServer.Managed
{
    public abstract class DisposableObject : IDisposable
    {
        private readonly object _lock = new object();

        public bool IsDisposed { get; private set; }

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
                if (this.IsDisposed)
                {
                    return;
                }

                this.IsDisposed = true;
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
