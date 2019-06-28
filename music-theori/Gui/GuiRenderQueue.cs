using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;

using OpenGL;

using theori.Graphics;
using theori.Resources;

namespace theori.Gui
{
    public class GuiRenderQueue : RenderQueue
    {
        private static readonly ClientResourceManager resourceManager;

        static GuiRenderQueue()
        {
            resourceManager = new ClientResourceManager(ClientResourceLocator.Default);
        }

        private static Material m_textureMaterialBacking;
        private static Material TextureMaterial
        {
            get
            {
                if (m_textureMaterialBacking == null)
                    m_textureMaterialBacking = resourceManager.AquireMaterial("materials/basic");
                return m_textureMaterialBacking;
            }
        }

        private Stack<Rect> scissors = new Stack<Rect>();
        private Rect scissor = Rect.EmptyScissor;

        private static readonly Mesh rectMesh = Mesh.CreatePlane(Vector3.UnitX, Vector3.UnitY, 1, 1, theori.Anchor.TopLeft);

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
            if (scissors.Count == 0)
                scissor = s;
            else scissor = scissors.Peek().Clamp(s);
            scissors.Push(scissor);
        }

        public void PopScissor()
        {
            scissors.Pop();
            if (scissors.Count == 0)
                scissor = Rect.EmptyScissor;
            else scissor = scissors.Peek();
        }

        public override void Process(bool clear)
        {
            //GL.CullFace(GL.GL_FRONT);
            base.Process(clear);
            //GL.CullFace(GL.GL_BACK);
        }

        public virtual void DrawRect(Transform transform, Rect rect, Texture texture, Vector4 color)
        {
            if (scissor.Width == 0 || scissor.Height == 0)
                return;

            transform = Transform.Scale(rect.Width, rect.Height, 1) * Transform.Translation(rect.Left, rect.Top, 0) * transform;

            var p = new MaterialParams();
            p["MainTexture"] = texture;
            p["Color"] = color;
            
            Draw(scissor, transform, rectMesh, TextureMaterial, p);
        }
    }
}
