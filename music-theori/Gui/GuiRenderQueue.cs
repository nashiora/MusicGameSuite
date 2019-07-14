using System.Collections.Generic;
using System.Numerics;

using OpenGL;

using theori.Graphics;

namespace theori.Gui
{
    public class GuiRenderQueue : RenderQueue
    {
        private static Material textureMaterialBacking;
        private static Material TextureMaterial
        {
            get
            {
                if (textureMaterialBacking == null)
                    textureMaterialBacking = Host.StaticResources.AquireMaterial("materials/basic");
                return textureMaterialBacking;
            }
        }

        private Stack<Rect> m_scissors = new Stack<Rect>();
        private Rect m_scissor = Rect.EmptyScissor;

        private static readonly Mesh rectMesh = Host.StaticResources.Manage(Mesh.CreatePlane(Vector3.UnitX, Vector3.UnitY, 1, 1, Anchor.TopLeft));

        public GuiRenderQueue(Vector2 viewportSize)
            : base(new RenderState
            {
                ProjectionMatrix = (Transform)Matrix4x4.CreateOrthographicOffCenter(0, viewportSize.X, viewportSize.Y, 0, -10, 10),
                CameraMatrix = Transform.Identity,
                ViewportSize = ((int)viewportSize.X, (int)viewportSize.Y),
            })
        {
        }

        public void PushScissor(Rect s)
        {
            if (m_scissors.Count == 0)
                m_scissor = s;
            else m_scissor = m_scissors.Peek().Clamp(s);
            m_scissors.Push(m_scissor);
        }

        public void PopScissor()
        {
            m_scissors.Pop();
            if (m_scissors.Count == 0)
                m_scissor = Rect.EmptyScissor;
            else m_scissor = m_scissors.Peek();
        }

        public override void Process(bool clear)
        {
            //GL.CullFace(GL.GL_FRONT);
            base.Process(clear);
            //GL.CullFace(GL.GL_BACK);
        }

        public virtual void DrawRect(Transform transform, Rect rect, Texture texture, Vector4 color)
        {
            if (m_scissor.Width == 0 || m_scissor.Height == 0)
                return;

            transform = Transform.Scale(rect.Width, rect.Height, 1) * Transform.Translation(rect.Left, rect.Top, 0) * transform;

            var p = new MaterialParams();
            p["MainTexture"] = texture;
            p["Color"] = color;

            Draw(m_scissor, transform, rectMesh, TextureMaterial, p);
        }

        public virtual void DrawRect(Transform transform, Rect rect, Texture texture, Material material, MaterialParams mParams, Vector4 color)
        {
            if (material == null)
                throw new System.ArgumentNullException(nameof(material));

            if (m_scissor.Width == 0 || m_scissor.Height == 0)
                return;

            transform = Transform.Scale(rect.Width, rect.Height, 1) * Transform.Translation(rect.Left, rect.Top, 0) * transform;

            if (mParams == null)
                mParams = new MaterialParams();
            else mParams = mParams.Copy();
            mParams["MainTexture"] = texture;
            mParams["Color"] = color;

            Draw(m_scissor, transform, rectMesh, material, mParams);
        }
    }
}
