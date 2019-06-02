using System.Collections.Generic;
using vfd = theori.Graphics.VertexFormatDescriptor;

namespace theori.Graphics
{
    public enum VertexData
    {
        SByte,
        Short,
        Int,
        Long,

        Byte,
        UShort,
        UInt,
        ULong,

        Float,
        Double,

        Vector2,
        Vector3,
        Vector4,
        
        Matrix4x4,
    }

    [System.AttributeUsage(System.AttributeTargets.Struct, AllowMultiple = false)]
    sealed class VertexTypeAttribute : System.Attribute
    {
        public VertexTypeAttribute(params VertexData[] types)
        {
            Types = types;
        }

        public VertexData[] Types { get; }

        private List<vfd> descriptors;
        public List<vfd> Descriptors
        {
            get
            {
                if (descriptors == null)
                {
                    descriptors = new List<vfd>(Types.Length);
                    for (int i = 0; i < Types.Length; i++)
                    {
                        var t = Types[i];

                        var desc = default(vfd);
                        switch (t)
                        {
                            case VertexData.SByte:      desc = vfd.SByte;     break;
                            case VertexData.Short:      desc = vfd.Short;     break;
                            case VertexData.Int:        desc = vfd.Int;       break;
                            case VertexData.Long:       desc = vfd.Long;      break;
                                
                            case VertexData.Byte:       desc = vfd.Byte;      break;
                            case VertexData.UShort:     desc = vfd.UShort;    break;
                            case VertexData.UInt:       desc = vfd.UInt;      break;
                            case VertexData.ULong:      desc = vfd.ULong;     break;

                            case VertexData.Float:      desc = vfd.Float;     break;
                            case VertexData.Double:     desc = vfd.Double;    break;

                            case VertexData.Vector2:    desc = vfd.Vector2;   break;
                            case VertexData.Vector3:    desc = vfd.Vector3;   break;
                            case VertexData.Vector4:    desc = vfd.Vector4;   break;
                            
                            case VertexData.Matrix4x4:  desc = vfd.Matrix4x4; break;
                        }

                        descriptors.Add(desc);
                    }
                }

                return descriptors;
            }
        }
    }
}
