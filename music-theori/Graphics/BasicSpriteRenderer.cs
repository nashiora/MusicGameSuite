using System.Collections.Generic;
using System.Numerics;

using MoonSharp.Interpreter;

using OpenGL;

using theori.Resources;

namespace theori.Graphics
{
    [MoonSharpUserData]
    public sealed class BasicSpriteRenderer : RenderQueue
    {
        private readonly Mesh m_rectMesh = Host.StaticResources.Manage(Mesh.CreatePlane(Vector3.UnitX, Vector3.UnitY, 1, 1, Anchor.TopLeft));

        private readonly ClientResourceManager m_resources;
        private readonly Material m_basicMaterial;

        private Vector4 m_drawColor = Vector4.One;
        private Transform m_transform = Transform.Identity;

        private readonly Stack<Transform> m_savedTransforms = new Stack<Transform>();

        [MoonSharpHidden]
        public BasicSpriteRenderer(ClientResourceLocator locator, Vector2 viewportSize)
            : base(new RenderState
            {
                ProjectionMatrix = (Transform)Matrix4x4.CreateOrthographicOffCenter(0, viewportSize.X, viewportSize.Y, 0, -10, 10),
                CameraMatrix = Transform.Identity,
                ViewportSize = ((int)viewportSize.X, (int)viewportSize.Y),
            })
        {
            m_resources = new ClientResourceManager(locator);
            m_basicMaterial = m_resources.AquireMaterial("materials/basic");
        }

        [MoonSharpHidden]
        public override void Process(bool clear)
        {
            base.Process(clear);
        }

        protected override void DisposeManaged()
        {
            base.DisposeManaged();

            m_rectMesh.Dispose();
            m_resources.Dispose();
        }

        #region Lua Bound Functions

        //public Texture LoadTextureAsync(string resourcePath) => m_resources.QueueTextureLoad(resourcePath);

        public void Flush() => Process(true);

        public void BeginFrame()
        {
            m_transform = Transform.Identity;
        }

        public void EndFrame()
        {
            Flush();

            m_savedTransforms.Clear();
        }

        public void SaveTransform()
        {
            m_savedTransforms.Push(m_transform);
        }

        public void RestoreTransform()
        {
            if (m_savedTransforms.Count == 0) return;
            m_transform = m_savedTransforms.Pop();
        }

        public void Translate(float x, float y)
        {
            m_transform = m_transform * Transform.Translation(x, y, 0);
        }

        public void Rotate(float rDeg)
        {
            m_transform = m_transform * Transform.RotationZ(rDeg);
        }

        public void Scale(float s) => Scale(s, s);
        public void Scale(float sx, float sy)
        {
            m_transform = m_transform * Transform.Scale(sx, sy, 1);
        }

        public void Shear(float sx, float sy)
        {
            var shear = Matrix4x4.Identity;
            shear.M21 = sx;
            shear.M12 = sy;

            m_transform = m_transform * new Transform(shear);
        }

        public void SetColor(float r, float g, float b) => SetColor(r, g, b, 255);

        public void SetColor(float r, float g, float b, float a)
        {
            m_drawColor = new Vector4(r, g, b, a) / 255.0f;
        }

        public void FillRect(float x, float y, float w, float h)
        {
            var transform = Transform.Scale(w, h, 1) * Transform.Translation(x, y, 0);

            var p = new MaterialParams();
            p["MainTexture"] = Texture.Empty;
            p["Color"] = m_drawColor;

            Draw(transform * m_transform, m_rectMesh, m_basicMaterial, p);
        }

        public void Image(Texture texture, float x, float y, float w, float h)
        {
            var transform = Transform.Scale(w, h, 1) * Transform.Translation(x, y, 0);

            var p = new MaterialParams();
            p["MainTexture"] = texture;
            p["Color"] = m_drawColor;

            Draw(transform * m_transform, m_rectMesh, m_basicMaterial, p);
        }

        #endregion
    }
}
