using System.Diagnostics;

namespace System
{
    public abstract class Disposable : IDisposable
    {
        public bool IsDisposed { get; private set; } = false;

        protected virtual bool SuppressFinalize => true;

#if DEBUG
        private readonly StackTrace m_constructionStackTrace;
#endif

        protected Disposable()
        {
#if DEBUG
            m_constructionStackTrace = new StackTrace();
#endif
        }

        protected virtual void DisposeManaged() { }
        protected virtual void DisposeUnmanaged() { }

        private void Dispose(bool fromManagedDispose)
        {
            if (IsDisposed) return;

            if (fromManagedDispose) DisposeManaged();
            DisposeUnmanaged();

            IsDisposed = true;
        }

        ~Disposable()
        {
            if (!IsDisposed)
#if DEBUG
                Logger.Log("[DISPOSEABLE] Disposable object finalized without previous managed dispose! The object was created:\n" + m_constructionStackTrace.ToString());
#else
                Logger.Log("[DISPOSEABLE] Disposable object finalized without previous managed dispose!");
#endif
            //Debug.Assert(IsDisposed, "Disposable object finalized without previous managed dispose! The object was created:\n" + m_constructionStackTrace.ToString());
            try
            {
                Dispose(false);
            }
            catch (Exception) { }
        }

        void IDisposable.Dispose() => Dispose();
        public void Dispose()
        {
            Dispose(true);
            if (SuppressFinalize)
                GC.SuppressFinalize(this);
        }
    }
}
