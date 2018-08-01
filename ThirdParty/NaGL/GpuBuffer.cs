using System;

namespace OpenGL
{
    public sealed class GpuBuffer : UIntHandle
    {
        public static GpuBuffer[] Create(int n, BufferTarget target = BufferTarget.Array)
        {
            uint[] handles = new uint[n];
            GL.GenBuffers(n, handles);
            
            var result = new GpuBuffer[n];
            for (int i = 0; i < n; i++)
                result[i] = new GpuBuffer(handles[i], target);

            return result;
        }
        
        public static GpuBuffer[] Create(BufferTarget[] targets)
        {
            int n = targets.Length;

            uint[] handles = new uint[n];
            GL.GenBuffers(n, handles);
            
            var result = new GpuBuffer[n];
            for (int i = 0; i < n; i++)
                result[i] = new GpuBuffer(handles[i], targets[i]);

            return result;
        }

        public static void Delete(GpuBuffer[] buffers)
        {
            int n = buffers.Length;

            uint[] handles = new uint[n];
            for (int i = 0; i < n; i++)
            {
                handles[i] = buffers[i].Handle;
                buffers[i].Invalidate();
            }

            GL.DeleteBuffers(n, handles);
        }

        public BufferTarget Target;

        public DataType Type;

        private GpuBuffer(uint handle, BufferTarget target)
            : base(handle, GL.DeleteBuffer)
        {
            Target = target;
        }

        public GpuBuffer(BufferTarget target)
            : base(GL.GenBuffer, GL.DeleteBuffer)
        {
            Target = target;
        }

        public void Bind() => GL.BindBuffer((uint)Target, Handle);
        
        public void SetData(int size, IntPtr data, Usage usage)
        {
            GL.BindBuffer((uint)Target, Handle);
            GL.BufferData((uint)Target, size, data, (uint)usage);

            Type = DataType.UnsignedByte;
        }

        public void SetData(float[] data, Usage usage)
        {
            GL.BindBuffer((uint)Target, Handle);
            GL.BufferData((uint)Target, data, (uint)usage);

            Type = DataType.Float;
        }

        public void SetData(ushort[] data, Usage usage)
        {
            GL.BindBuffer((uint)Target, Handle);
            GL.BufferData((uint)Target, data, (uint)usage);

            Type = DataType.UnsignedShort;
        }
    }
}
