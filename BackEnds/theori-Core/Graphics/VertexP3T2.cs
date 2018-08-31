using System.Numerics;
using System.Runtime.InteropServices;

namespace theori.Graphics
{
    [StructLayout(LayoutKind.Sequential, Size = 5 * 4)]
    [VertexType(VertexData.Vector3, VertexData.Vector2)]
    public struct VertexP3T2
    {
        public Vector3 Position;
        public Vector2 TextureCoords;

        public VertexP3T2(Vector3 pos, Vector2 tex)
        {
            Position = pos;
            TextureCoords = tex;
        }
    }
}
