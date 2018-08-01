using System;

namespace FxMania
{
    public abstract class Disposable : IDisposable
    {
        #region IDisposable Support

        private bool isDisposed = false;

        protected virtual bool SuppressFinalize => true;

        protected virtual void DisposeManaged()
        {
        }

        protected virtual void DisposeUnmanaged()
        {
        }

        private void Dispose(bool disposing)
        {
            if (isDisposed) return;

            if (disposing) DisposeManaged();
            DisposeUnmanaged();

            isDisposed = true;
        }

        ~Disposable()
        {
            try
            {
                Dispose(false);
            }
            catch (Exception) { }
        }

        public void Dispose()
        {
            Dispose(true);
            if (SuppressFinalize)
                GC.SuppressFinalize(this);
        }

        #endregion
    }
}
