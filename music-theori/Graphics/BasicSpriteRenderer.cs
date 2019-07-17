using System;
using System.Collections.Generic;
using System.Numerics;

using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using OpenGL;

using theori.Resources;

using static MoonSharp.Interpreter.DynValue;

namespace theori.Graphics
{
    [MoonSharpUserData]
    public sealed class BasicSpriteRenderer : Disposable
    {
        private Vector2? m_viewport;

        private Mesh m_rectMesh = Host.StaticResources.Manage(Mesh.CreatePlane(Vector3.UnitX, Vector3.UnitY, 1, 1, Anchor.TopLeft));

        private ClientResourceManager m_resources;
        private Material m_basicMaterial;

        private Vector4 m_drawColor = Vector4.One, m_imageColor = Vector4.One;
        private Transform m_transform = Transform.Identity;

        private readonly Stack<Transform> m_savedTransforms = new Stack<Transform>();

        private RenderQueue m_queue;
        private Font m_font = Font.Default16;

        private readonly Dictionary<Font, TextRasterizer> m_rasterizers = new Dictionary<Font, TextRasterizer>();

        [MoonSharpHidden]
        public BasicSpriteRenderer(ClientResourceLocator locator = null, Vector2? viewportSize = null)
        {
            m_viewport = viewportSize;
            m_resources = new ClientResourceManager(locator ?? ClientResourceLocator.Default);
            m_basicMaterial = m_resources.AquireMaterial("materials/basic");
        }

        private TextRasterizer GetRasterizerForFont(Font font)
        {
            if (!m_rasterizers.TryGetValue(font, out var rasterizer))
            {
                rasterizer = new TextRasterizer(font, null);
                m_rasterizers[font] = rasterizer;
            }
            return rasterizer;
        }

        protected override void DisposeManaged()
        {
            Flush();

            m_queue?.Dispose();
            m_queue = null;

            m_rectMesh?.Dispose();
            m_rectMesh = null;

            m_resources?.Dispose();
            m_resources = null;

            m_basicMaterial = null;
        }

        #region Lua Bound Functions

        public void Flush() => m_queue?.Process(true);

        public void BeginFrame()
        {
            m_transform = Transform.Identity;
            m_drawColor = Vector4.One;
            m_imageColor = Vector4.One;

            SetFont(null, 16);

            Vector2 viewportSize = m_viewport ?? new Vector2(Window.Width, Window.Height);
            m_queue = new RenderQueue(new RenderState
            {
                ProjectionMatrix = (Transform)Matrix4x4.CreateOrthographicOffCenter(0, viewportSize.X, viewportSize.Y, 0, -10, 10),
                CameraMatrix = Transform.Identity,
                ViewportSize = ((int)viewportSize.X, (int)viewportSize.Y),
            });
        }

        public void EndFrame()
        {
            Flush();

            m_queue.Dispose();
            m_queue = null;

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

        public void ResetTransform()
        {
            m_savedTransforms.Clear();
            m_transform = Transform.Identity;
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

        [MoonSharpVisible(true)]
        private DynValue GetViewportSize()
        {
            if (m_viewport is Vector2 v)
                return NewTuple(NewNumber(v.X), NewNumber(v.Y));
            else return NewTuple(NewNumber(Window.Width), NewNumber(Window.Height));
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

            m_queue.Draw(transform * m_transform, m_rectMesh, m_basicMaterial, p);
        }

        public void SetImageColor(float r, float g, float b) => SetImageColor(r, g, b, 255);

        public void SetImageColor(float r, float g, float b, float a)
        {
            m_imageColor = new Vector4(r, g, b, a) / 255.0f;
        }

        public void Image(Texture texture, float x, float y, float w, float h)
        {
            var transform = Transform.Scale(w, h, 1) * Transform.Translation(x, y, 0);

            var p = new MaterialParams();
            p["MainTexture"] = texture;
            p["Color"] = m_imageColor;

            m_queue.Draw(transform * m_transform, m_rectMesh, m_basicMaterial, p);
        }

        public void SetFont(Font font, int size)
        {
            if (font == null) font = Font.GetDefault(size);
            m_font = font;
        }

        public void Write(string text, float x, float y)
        {
            var rasterizer = GetRasterizerForFont(m_font);
            rasterizer.Text = text;

            var transform = Transform.Scale(rasterizer.Width, rasterizer.Height, 1) * Transform.Translation(x, y, 0);

            var p = new MaterialParams();
            p["MainTexture"] = rasterizer.Texture;
            p["Color"] = m_drawColor;

            m_queue.Draw(transform * m_transform, m_rectMesh, m_basicMaterial, p);
        }

        #endregion
    }
}
