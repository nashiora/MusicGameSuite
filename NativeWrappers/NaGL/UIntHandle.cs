using System;

namespace OpenGL
{
    public abstract class UIntHandle : Disposable
    {
        protected override bool SuppressFinalize => true;

        public static bool operator true (UIntHandle p) => p.Handle != 0;
        public static bool operator false(UIntHandle p) => p.Handle == 0;

        public static bool operator !(UIntHandle p) => p.Handle == 0;

        private Action<uint> m_deleteHandle;

        public bool IsValid => Handle != 0;
        public uint Handle { get; protected set; }

        protected UIntHandle(uint handle, Action<uint> deleteHandle)
        {
            Handle = handle;
            m_deleteHandle = deleteHandle;
        }

        protected UIntHandle(Func<uint> createHandle, Action<uint> deleteHandle)
        {
            Handle = createHandle();
            m_deleteHandle = deleteHandle;
        }

        protected void Invalidate() => Handle = 0;

        protected override void DisposeManaged()
        {
            if (Handle != 0) m_deleteHandle(Handle);
            Handle = 0;
        }
    }
}
