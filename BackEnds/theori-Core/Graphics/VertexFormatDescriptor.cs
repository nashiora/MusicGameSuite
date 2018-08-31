using System.Collections.Generic;

namespace theori.Graphics
{
    public struct VertexFormatDescriptor
    {
        public static VertexFormatDescriptor SByte      = new VertexFormatDescriptor(1, 1, false, true);
        public static VertexFormatDescriptor Short      = new VertexFormatDescriptor(1, 2, false, true);
        public static VertexFormatDescriptor Int        = new VertexFormatDescriptor(1, 4, false, true);
        public static VertexFormatDescriptor Long       = new VertexFormatDescriptor(1, 8, false, true);
                                                        
        public static VertexFormatDescriptor Byte       = new VertexFormatDescriptor(1, 1, false, false);
        public static VertexFormatDescriptor UShort     = new VertexFormatDescriptor(1, 2, false, false);
        public static VertexFormatDescriptor UInt       = new VertexFormatDescriptor(1, 4, false, false);
        public static VertexFormatDescriptor ULong      = new VertexFormatDescriptor(1, 8, false, false);
                                                        
        public static VertexFormatDescriptor Float      = new VertexFormatDescriptor(1, 4, true, true);
        public static VertexFormatDescriptor Double     = new VertexFormatDescriptor(1, 8, true, true);
                                                        
        public static VertexFormatDescriptor Vector2    = new VertexFormatDescriptor(2, 4, true, true);
        public static VertexFormatDescriptor Vector3    = new VertexFormatDescriptor(3, 4, true, true);
        public static VertexFormatDescriptor Vector4    = new VertexFormatDescriptor(4, 4, true, true);
                                                        
        public static VertexFormatDescriptor Matrix4x4  = new VertexFormatDescriptor(16, 4, true, true);

        public uint ComponentCount;
        public uint ComponentSize;
        public bool IsFloat;
        public bool IsSigned;

        public VertexFormatDescriptor(uint count, uint size, bool f, bool s)
        {
            ComponentCount = count;
            ComponentSize = size;
            IsFloat = f;
            IsSigned = s;
        }
    }
}
