using System;

namespace OpenGL
{
    public abstract class UIntHandle : IDisposable
    {
        public static bool operator true (UIntHandle p) => p.handle != 0;
        public static bool operator false(UIntHandle p) => p.handle == 0;

        public static bool operator !(UIntHandle p) => p.handle == 0;

        private Action<uint> deleteHandle;

        private uint handle;
        public bool IsValid => handle != 0;

        internal uint Handle => handle;

        protected UIntHandle(uint handle, Action<uint> deleteHandle)
        {
            this.handle = handle;
            this.deleteHandle = deleteHandle;
        }

        protected UIntHandle(Func<uint> createHandle, Action<uint> deleteHandle)
        {
            handle = createHandle();
            this.deleteHandle = deleteHandle;
        }

        protected void Invalidate()
        {
            handle = 0;
        }

        public virtual void Delete()
        {
            if (handle != 0) deleteHandle(handle);
            handle = 0;
        }

        void IDisposable.Dispose() => Delete();
    }
}
