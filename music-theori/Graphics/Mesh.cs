using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;

using OpenGL;
using static OpenGL.GL;

namespace theori.Graphics
{
    public class Mesh : Disposable
    {
        public static Mesh CreatePlane(Vector3 axis0, Vector3 axis1, float size0 = 1, float size1 = 1, Anchor anchor = Anchor.Center)
        {
            axis0 *= size0;
            axis1 *= size1;

            float offset0 = 0.5f, offset1 = 0.5f;
            if (anchor.HasFlag(Anchor.Top))
                offset1 = 0;
            else if (anchor.HasFlag(Anchor.Bottom))
                offset1 = 1;
            if (anchor.HasFlag(Anchor.Left))
                offset0 = 0;
            else if (anchor.HasFlag(Anchor.Right))
                offset0 = 1;
            
            Vector3 min0 = -offset0 * axis0;
            Vector3 max0 = min0 + axis0;
            Vector3 min1 = -offset1 * axis1;
            Vector3 max1 = min1 + axis1;
            
            var plane = new Mesh();
            plane.SetVertices(new VertexP3T2[]
            {
                new VertexP3T2(min0 + min1, Vector2.Zero),
                new VertexP3T2(max0 + min1, Vector2.UnitX),
                new VertexP3T2(min0 + max1, Vector2.UnitY),
                new VertexP3T2(max0 + max1, Vector2.One),
            });
            plane.SetIndices(new ushort[] { 0, 1, 2, 2, 1, 3 });

            return plane;
        }

        private VertexArray vao;
        private GpuBuffer vertexBuffer;
        private GpuBuffer indexBuffer;
        
        private int indexCount;
        
        public PrimitiveType PrimitiveType = PrimitiveType.Triangles;
        public DataType IndexType;

        public Mesh()
        {
            vao = new VertexArray();
            vertexBuffer = new GpuBuffer(BufferTarget.Array);
            indexBuffer = new GpuBuffer(BufferTarget.ElementArray);
        }
        
        public virtual unsafe void SetVertices(float[] vertices, List<VertexFormatDescriptor> desc)
        {
            vertexBuffer.SetData(vertices, Usage.DynamicDraw);
            SetupVertexAttribs(desc);
        }
        
        public virtual unsafe void SetVertices<T>(T[] vertices)
            where T : struct
        {
            object[] attribs = typeof(T).GetCustomAttributes(typeof(VertexTypeAttribute), false);
            if (attribs.Length == 0)
                throw new ArgumentException($"{ typeof(T).Name } is not a valid vertex type.");
            var desc = (attribs[0] as VertexTypeAttribute).Descriptors;

            int structSize = Marshal.SizeOf<T>();
            int bufSize = vertices.Length * structSize;
            var ptr = Marshal.AllocHGlobal(bufSize);
            for (int i = 0; i < vertices.Length; i++)
                Marshal.StructureToPtr(vertices[i], ptr + i * structSize, true);
            vertexBuffer.SetData(bufSize, ptr, Usage.DynamicDraw);
            Marshal.FreeHGlobal(ptr);

            SetupVertexAttribs(desc);
        }

        public virtual void SetupVertexAttribs(IReadOnlyList<VertexFormatDescriptor> desc)
        {
            vao.Bind();

            uint vertexSize = 0;
            for (int count = desc.Count, i = 0; i < count; i++)
                vertexSize += desc[i].ComponentCount * desc[i].ComponentSize;

			uint index = 0;
			uint offset = 0;
            for (int count = desc.Count, i = 0; i < count; i++)
			{
                var e = desc[i];

				uint type = uint.MaxValue;
				if (!e.IsFloat)
				{
					if (e.ComponentSize == 4)
						type = e.IsSigned ? GL_INT : GL_UNSIGNED_INT;
					else if (e.ComponentSize == 2)
						type = e.IsSigned ? GL_SHORT : GL_UNSIGNED_SHORT;
					else if (e.ComponentSize == 1)
						type = e.IsSigned ? GL_BYTE : GL_UNSIGNED_BYTE;
				}
				else
				{
					if (e.ComponentSize == 4)
						type = GL_FLOAT;
					else if (e.ComponentSize == 8)
						type = GL_DOUBLE;
				}
				Debug.Assert(type != uint.MaxValue);

                vao.SetVertexAttrib(index, vertexBuffer, (int)e.ComponentCount, (DataType)type, true, (int)vertexSize, offset);

				offset += e.ComponentSize * e.ComponentCount;
				index++;
			}
        }

        public virtual void SetIndices(params ushort[] indices)
        {
            indexCount = indices.Length;

            vao.Bind();
            indexBuffer.SetData(indices, Usage.DynamicDraw);
            IndexType = DataType.UnsignedShort;
        }

        public virtual void Draw()
        {
            vao.Bind();
            Redraw();
        }

        public virtual void Redraw()
        {
            DrawElements((uint)PrimitiveType, indexCount, (uint)IndexType, IntPtr.Zero);
        }

        protected override void DisposeManaged()
        {
            vertexBuffer.Dispose();
            indexBuffer.Dispose();
            vao.Dispose();
        }
    }
}
