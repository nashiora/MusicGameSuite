using System;
using System.Numerics;

using theori;
using theori.Graphics;

using OpenRM;
using System.Diagnostics;
using theori.Resources;

namespace NeuroSonic.GamePlay
{
    internal abstract class ObjectRenderable3D
    {
        public readonly OpenRM.Object Object;

        protected ObjectRenderable3D(OpenRM.Object obj)
        {
            Object = obj;
        }

        public abstract void Destroy();
        public abstract void Render(RenderQueue rq, Transform world);
    }

    internal class ButtonChipRenderState3D : ObjectRenderable3D
    {
        private static readonly Mesh chipMesh = Mesh.CreatePlane(Vector3.UnitX, Vector3.UnitZ, 1.0f / 6, 0.1f, Anchor.BottomCenter);

        public new OpenRM.Voltex.ButtonObject Object => (OpenRM.Voltex.ButtonObject)base.Object;

        private int m_width = 1;

        private Transform m_transform = Transform.Identity;
        private readonly Drawable3D m_drawable;

        public ButtonChipRenderState3D(OpenRM.Voltex.ButtonObject obj, ClientResourceManager skin)
            : base(obj)
        {
            Debug.Assert(obj.IsChip, "Hold object passed to render state which expects a chip");

            string textureName;
            if (obj.Stream < 4)
            {
                if (obj.HasSample)
                    textureName = "textures/bt_chip_sample";
                else textureName = "textures/bt_chip";
            }
            else
            {
                m_width = 2;
                if (obj.HasSample)
                    textureName = "textures/fx_chip_sample";
                else textureName = "textures/fx_chip";
            }

            m_drawable = new Drawable3D()
            {
                Texture = skin.AquireTexture(textureName),
                Material = skin.AquireMaterial("materials/chip"),
                Mesh = chipMesh,
            };

            m_transform = Transform.Scale(m_width, 1, 1);
        }

        public override void Destroy()
        {
        }

        public override void Render(RenderQueue rq, Transform world)
        {
            m_drawable.DrawToQueue(rq, m_transform * world);
        }
    }

    internal class ButtonHoldRenderState3D : ObjectRenderable3D
    {
        private const float ENTRY_LENGTH = 0.1f;
        private const float EXIT_LENGTH = ENTRY_LENGTH * 0.5f;

        private static readonly Mesh holdMesh = Mesh.CreatePlane(Vector3.UnitX, Vector3.UnitZ, 1.0f / 6, 1.0f, Anchor.BottomCenter);

        public new OpenRM.Voltex.ButtonObject Object => (OpenRM.Voltex.ButtonObject)base.Object;

        private Transform m_entryTransform = Transform.Scale(1, 1, ENTRY_LENGTH);
        private Transform m_exitTransform = Transform.Scale(1, 1, EXIT_LENGTH);
        private Transform m_holdTransform = Transform.Identity;

        private int m_width = 1;

        private float m_glow = -1.0f;
        private int m_glowState = -1;

        public float Glow
        {
            get => m_glow;
            set
            {
                if (value == m_glow) return;

                m_glow = value;
                foreach (var d in m_drawables)
                    d.Params["Glow"] = value;
            }
        }
        public int GlowState
        {
            get => m_glowState;
            set
            {
                if (value == m_glowState) return;

                m_glowState = value;
                foreach (var d in m_drawables)
                    d.Params["GlowState"] = value;
            }
        }

        private readonly Drawable3D[] m_drawables;

        public ButtonHoldRenderState3D(OpenRM.Voltex.ButtonObject obj, float len, ClientResourceManager skin)
            : base(obj)
        {
            Debug.Assert(obj.IsHold, "Chip object passed to render state which expects a hold");

            string holdTextureName;
            if (obj.Stream < 4)
                holdTextureName = "textures/bt_hold";
            else
            {
                holdTextureName = "textures/fx_hold";
                m_width = 2;
            }

            m_drawables = new Drawable3D[3]
            {
                new Drawable3D()
                {
                    Texture = skin.AquireTexture(holdTextureName),
                    Material = skin.AquireMaterial("materials/hold"),
                    Mesh = holdMesh,
                },

                new Drawable3D()
                {
                    Texture = skin.AquireTexture(holdTextureName + "_entry"),
                    Material = skin.AquireMaterial("materials/basic"),
                    Mesh = holdMesh,
                },

                new Drawable3D()
                {
                    Texture = skin.AquireTexture(holdTextureName + "_exit"),
                    Material = skin.AquireMaterial("materials/hold"),
                    Mesh = holdMesh,
                },
            };

            Glow = 1.0f;
            GlowState = 1;

            m_drawables[1].Params["Color"] = Vector4.One;

            m_entryTransform = Transform.Scale(m_width, 1, ENTRY_LENGTH);
            m_holdTransform = Transform.Scale(m_width, 1, len - EXIT_LENGTH - ENTRY_LENGTH) * Transform.Translation(0, 0, -ENTRY_LENGTH);
            m_exitTransform = Transform.Scale(m_width, 1, EXIT_LENGTH) * Transform.Translation(0, 0, -len + EXIT_LENGTH);
        }

        public override void Destroy()
        {
        }

        public override void Render(RenderQueue rq, Transform world)
        {
            m_drawables[0].DrawToQueue(rq, m_holdTransform * world);
            m_drawables[1].DrawToQueue(rq, m_entryTransform * world);
            m_drawables[2].DrawToQueue(rq, m_exitTransform * world);
        }
    }

    internal class SlamRenderState3D : ObjectRenderable3D
    {
        public new OpenRM.Voltex.AnalogObject Object => (OpenRM.Voltex.AnalogObject)base.Object;
        
        private Transform m_transform = Transform.Identity;
        private readonly Drawable3D m_drawable;

        public SlamRenderState3D(OpenRM.Voltex.AnalogObject obj, float len, Vector3 color, ClientResourceManager skin)
            : base(obj)
        {
            Debug.Assert(obj.IsInstant, "Analog for slam render state wasn't a slam");
            
            float range = 5 / 6.0f * (obj.RangeExtended ? 2 : 1);

            m_transform = Transform.Scale(1, 1, len);
            var mesh = new Mesh();

            const float W = 1 / 6.0f;
            
            float il = range * (obj.InitialValue - 0.5f) - W / 2;
            float ir = range * (obj.InitialValue - 0.5f) + W / 2;
            
            float fl = range * (obj.FinalValue - 0.5f) - W / 2;
            float fr = range * (obj.FinalValue - 0.5f) + W / 2;

            if (obj.InitialValue < obj.FinalValue)
            {
                ushort[] indices = new ushort[] { 0, 1, 2, 1, 5, 2, 2, 5, 4, 3, 4, 5 };
                mesh.SetIndices(indices);
            
                VertexP3T2[] vertices = new VertexP3T2[6]
                {
                    new VertexP3T2(new Vector3(il, 0, 0), new Vector2(0, 1)),
                    new VertexP3T2(new Vector3(il, 0, -1), new Vector2(0, 0)),
                    new VertexP3T2(new Vector3(ir, 0, 0), new Vector2(1, 1)),

                    new VertexP3T2(new Vector3(fr, 0, -1), new Vector2(1, 0)),
                    new VertexP3T2(new Vector3(fr, 0, 0), new Vector2(1, 1)),
                    new VertexP3T2(new Vector3(fl, 0, -1), new Vector2(0, 1)),
                };
                mesh.SetVertices(vertices);
            }
            else
            {
                ushort[] indices = new ushort[] { 0, 1, 2, 4, 1, 0, 4, 0, 5, 3, 4, 5 };
                mesh.SetIndices(indices);
            
                VertexP3T2[] vertices = new VertexP3T2[6]
                {
                    new VertexP3T2(new Vector3(il, 0, 0), new Vector2(0, 1)),
                    new VertexP3T2(new Vector3(ir, 0, -1), new Vector2(1, 0)),
                    new VertexP3T2(new Vector3(ir, 0, 0), new Vector2(1, 1)),

                    new VertexP3T2(new Vector3(fl, 0, -1), new Vector2(0, 0)),
                    new VertexP3T2(new Vector3(fr, 0, -1), new Vector2(1, 0)),
                    new VertexP3T2(new Vector3(fl, 0, 0), new Vector2(0, 1)),
                };
                mesh.SetVertices(vertices);
            }

            m_drawable = new Drawable3D()
            {
                Texture = skin.AquireTexture("textures/laser"),
                Material = skin.AquireMaterial("materials/laser"),
                Mesh = mesh,
            };

            m_drawable.Material.BlendMode = BlendMode.Additive;

            m_drawable.Params["LaserColor"] = color;
            m_drawable.Params["HiliteColor"] = new Vector3(1, 1, 0);
        }

        public override void Destroy()
        {
            m_drawable.Mesh.Dispose();
        }

        public override void Render(RenderQueue rq, Transform world)
        {
            m_drawable.DrawToQueue(rq, m_transform * world);
        }
    }

    internal class LaserRenderState3D : ObjectRenderable3D
    {
        public new OpenRM.Voltex.AnalogObject Object => (OpenRM.Voltex.AnalogObject)base.Object;
        
        private Transform m_transform = Transform.Identity;
        private readonly Drawable3D m_drawable;

        public LaserRenderState3D(OpenRM.Voltex.AnalogObject obj, float len, Vector3 color, ClientResourceManager skin)
            : base(obj)
        {
            Debug.Assert(!obj.IsInstant, "analog for segment render state was a slam");

            m_transform = Transform.Scale(1, 1, len);
            var mesh = new Mesh();

            const float W = 1 / 6.0f;

            ushort[] indices = new ushort[] { 0, 1, 2, 0, 2, 3, };
            mesh.SetIndices(indices);
            
            float range = 5 / 6.0f * (obj.RangeExtended ? 2 : 1);

            float il = range * (obj.InitialValue - 0.5f) - W / 2;
            float ir = range * (obj.InitialValue - 0.5f) + W / 2;
            
            float fl = range * (obj.FinalValue - 0.5f) - W / 2;
            float fr = range * (obj.FinalValue - 0.5f) + W / 2;

            VertexP3T2[] vertices = new VertexP3T2[4]
            {
                new VertexP3T2(new Vector3(il, 0, 0), new Vector2(0, 1)),
                new VertexP3T2(new Vector3(fl, 0, -1), new Vector2(0, 0)),
                new VertexP3T2(new Vector3(fr, 0, -1), new Vector2(1, 0)),
                new VertexP3T2(new Vector3(ir, 0, 0), new Vector2(1, 1)),
            };

            mesh.SetVertices(vertices);

            m_drawable = new Drawable3D()
            {
                Texture = skin.AquireTexture("textures/laser"),
                Material = skin.AquireMaterial("materials/laser"),
                Mesh = mesh,
            };

            m_drawable.Material.BlendMode = BlendMode.Additive;

            m_drawable.Params["LaserColor"] = color;
            m_drawable.Params["HiliteColor"] = new Vector3(1, 1, 0);
        }

        public override void Destroy()
        {
            m_drawable.Mesh.Dispose();
        }

        public override void Render(RenderQueue rq, Transform world)
        {
            m_drawable.DrawToQueue(rq, m_transform * world);
        }
    }
}
