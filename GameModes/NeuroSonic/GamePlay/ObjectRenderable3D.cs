using System.Diagnostics;
using System.Numerics;

using theori;
using theori.Charting;
using theori.Graphics;
using theori.Resources;

using NeuroSonic.Charting;

namespace NeuroSonic.GamePlay
{
    internal abstract class ObjectRenderable3D
    {
        public readonly ChartObject Object;

        protected ObjectRenderable3D(ChartObject obj)
        {
            Object = obj;
        }

        public abstract void Destroy();
        public abstract void Render(RenderQueue rq, Transform world);
    }

    internal abstract class GlowingRenderState3D : ObjectRenderable3D
    {
        private float m_glow = -1.0f;
        private int m_glowState = -1;

        protected GlowingRenderState3D(ChartObject obj)
            : base(obj)
        {
        }

        public float Glow
        {
            get => m_glow;
            set => SetGlow(m_glow = value);
        }
        public int GlowState
        {
            get => m_glowState;
            set => SetGlowState(m_glowState = value);
        }

        protected abstract void SetGlow(float glow);
        protected abstract void SetGlowState(int glow);
    }

    internal class ButtonChipRenderState3D : ObjectRenderable3D
    {
        private static readonly Mesh chipMesh = Mesh.CreatePlane(Vector3.UnitX, Vector3.UnitZ, 1.0f / 6, 0.1f, Anchor.BottomCenter);

        public new ButtonObject Object => (ButtonObject)base.Object;

        private int m_width = 1;

        private Transform m_transform = Transform.Identity;
        private readonly Drawable3D m_drawable;

        public ButtonChipRenderState3D(ButtonObject obj, ClientResourceManager skin)
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
                Texture = skin.GetTexture(textureName),
                Material = skin.GetMaterial("materials/chip"),
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

    internal class ButtonHoldRenderState3D : GlowingRenderState3D
    {
        private const float ENTRY_LENGTH = 0.1f;
        private const float EXIT_LENGTH = ENTRY_LENGTH * 0.5f;

        private static readonly Mesh holdMesh = Mesh.CreatePlane(Vector3.UnitX, Vector3.UnitZ, 1.0f / 6, 1.0f, Anchor.BottomCenter);

        public new ButtonObject Object => (ButtonObject)base.Object;

        private Transform m_entryTransform = Transform.Scale(1, 1, ENTRY_LENGTH);
        private Transform m_exitTransform = Transform.Scale(1, 1, EXIT_LENGTH);
        private Transform m_holdTransform = Transform.Identity;

        private int m_width = 1;

        private readonly Drawable3D[] m_drawables;

        public ButtonHoldRenderState3D(ButtonObject obj, float len, ClientResourceManager skin)
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
                    Texture = skin.GetTexture(holdTextureName),
                    Material = skin.GetMaterial("materials/hold"),
                    Mesh = holdMesh,
                },

                new Drawable3D()
                {
                    Texture = skin.GetTexture(holdTextureName + "_entry"),
                    Material = skin.GetMaterial("materials/basic"),
                    Mesh = holdMesh,
                },

                new Drawable3D()
                {
                    Texture = skin.GetTexture(holdTextureName + "_exit"),
                    Material = skin.GetMaterial("materials/hold"),
                    Mesh = holdMesh,
                },
            };

            m_drawables[1].Params["Color"] = Vector4.One;

            m_entryTransform = Transform.Scale(m_width, 1, ENTRY_LENGTH);
            m_holdTransform = Transform.Scale(m_width, 1, len - EXIT_LENGTH - ENTRY_LENGTH) * Transform.Translation(0, 0, -ENTRY_LENGTH);
            m_exitTransform = Transform.Scale(m_width, 1, EXIT_LENGTH) * Transform.Translation(0, 0, -len + EXIT_LENGTH);

            Glow = 0.0f;
            GlowState = 1;
        }

        protected override void SetGlow(float glow)
        {
            foreach (var d in m_drawables)
                d.Params["GlowState"] = glow;
        }

        protected override void SetGlowState(int glowState)
        {
            foreach (var d in m_drawables)
                d.Params["GlowState"] = glowState;
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

    internal class SlamRenderState3D : GlowingRenderState3D
    {
        private const float LASER_WIDTH = 2.0f;

        public new AnalogObject Object => (AnalogObject)base.Object;
        
        private Transform m_transform = Transform.Identity;
        private readonly Drawable3D m_drawable;

        public SlamRenderState3D(AnalogObject obj, float len, Vector3 color, ClientResourceManager skin)
            : base(obj)
        {
            Debug.Assert(obj.IsInstant, "Analog for slam render state wasn't a slam");
            
            float range = 5 / 6.0f * (obj.RangeExtended ? 2 : 1);
            
            m_transform = Transform.Scale(1, 1, len);
            var mesh = new Mesh();

            const float W = LASER_WIDTH / 6.0f;

            float il  = range * (obj.InitialValue - 0.5f) - W / 2;
            float ilh = range * (obj.InitialValue - 0.5f) - W / 4;
            float ir  = range * (obj.InitialValue - 0.5f) + W / 2;
            float irh = range * (obj.InitialValue - 0.5f) + W / 4;

            float fl  = range * (obj.FinalValue - 0.5f) - W / 2;
            float flh = range * (obj.FinalValue - 0.5f) - W / 4;
            float fr  = range * (obj.FinalValue - 0.5f) + W / 2;
            float frh = range * (obj.FinalValue - 0.5f) + W / 4;

            if (obj.InitialValue < obj.FinalValue)
            {

#if false
                ushort[] indices = new ushort[] { 0, 1, 2, 1, 5, 2, 2, 5, 4, 3, 4, 5 };
                VertexP3T2[] vertices = new VertexP3T2[6]
                {
                    new VertexP3T2(new Vector3(il, 0, 0), new Vector2(0, 1)),
                    new VertexP3T2(new Vector3(il, 0, -1), new Vector2(0, 0)),
                    new VertexP3T2(new Vector3(ir, 0, 0), new Vector2(1, 1)),

                    new VertexP3T2(new Vector3(fr, 0, -1), new Vector2(1, 0)),
                    new VertexP3T2(new Vector3(fr, 0, 0), new Vector2(1, 1)),
                    new VertexP3T2(new Vector3(fl, 0, -1), new Vector2(0, 1)),
                };
#else
                ushort[] indices = new ushort[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17 };
                VertexP3T2[] vertices = new VertexP3T2[]
                {
                    new VertexP3T2(new Vector3(il , 0,  0.0f), new Vector2(0.00f, 0.875f)),
                    new VertexP3T2(new Vector3(il , 0, -1.5f), new Vector2(0.00f, 0.500f)),
                    new VertexP3T2(new Vector3(irh, 0,  0.0f), new Vector2(0.75f, 0.875f)),

                    new VertexP3T2(new Vector3(ir , 0,  0.5f), new Vector2(1.00f, 0.000f)),
                    new VertexP3T2(new Vector3(il , 0, -1.5f), new Vector2(0.00f, 0.500f)),
                    new VertexP3T2(new Vector3(ir , 0, -1.5f), new Vector2(0.00f, 0.000f)),


                    new VertexP3T2(new Vector3(ir , 0,  0.5f), new Vector2(1.00f, 1.000f)),
                    new VertexP3T2(new Vector3(ir , 0, -1.5f), new Vector2(0.00f, 1.000f)),
                    new VertexP3T2(new Vector3(fl , 0, -1.5f), new Vector2(0.00f, 0.000f)),

                    new VertexP3T2(new Vector3(ir , 0,  0.5f), new Vector2(1.00f, 1.000f)),
                    new VertexP3T2(new Vector3(fl , 0, -1.5f), new Vector2(0.00f, 0.000f)),
                    new VertexP3T2(new Vector3(fl , 0,  0.5f), new Vector2(1.00f, 0.000f)),


                    new VertexP3T2(new Vector3(fl , 0,  0.5f), new Vector2(1.00f, 1.000f)),
                    new VertexP3T2(new Vector3(fl , 0, -1.5f), new Vector2(0.00f, 1.000f)),
                    new VertexP3T2(new Vector3(fr , 0,  0.5f), new Vector2(1.00f, 0.500f)),

                    new VertexP3T2(new Vector3(fr , 0,  0.5f), new Vector2(1.00f, 0.500f)),
                    new VertexP3T2(new Vector3(flh, 0, -1.0f), new Vector2(0.25f, 0.125f)),
                    new VertexP3T2(new Vector3(fr , 0, -1.0f), new Vector2(1.00f, 0.125f)),
                };
#endif

                mesh.SetIndices(indices);
                mesh.SetVertices(vertices);
            }
            else
            {
#if false
                ushort[] indices = new ushort[] { 0, 1, 2, 4, 1, 0, 4, 0, 5, 3, 4, 5 };
                VertexP3T2[] vertices = new VertexP3T2[6]
                {
                    new VertexP3T2(new Vector3(il, 0, 0), new Vector2(0, 1)),
                    new VertexP3T2(new Vector3(ir, 0, -1), new Vector2(1, 0)),
                    new VertexP3T2(new Vector3(ir, 0, 0), new Vector2(1, 1)),

                    new VertexP3T2(new Vector3(fl, 0, -1), new Vector2(0, 0)),
                    new VertexP3T2(new Vector3(fr, 0, -1), new Vector2(1, 0)),
                    new VertexP3T2(new Vector3(fl, 0, 0), new Vector2(0, 1)),
                };
#else
                ushort[] indices = new ushort[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17 };
                VertexP3T2[] vertices = new VertexP3T2[]
                {
                    new VertexP3T2(new Vector3(il , 0,  0.5f), new Vector2(0.00f, 0.000f)),
                    new VertexP3T2(new Vector3(il , 0, -1.5f), new Vector2(1.00f, 0.000f)),
                    new VertexP3T2(new Vector3(ir , 0, -1.5f), new Vector2(1.00f, 0.500f)),

                    new VertexP3T2(new Vector3(ilh, 0,  0.0f), new Vector2(0.25f, 0.875f)),
                    new VertexP3T2(new Vector3(ir , 0, -1.5f), new Vector2(1.00f, 0.500f)),
                    new VertexP3T2(new Vector3(ir , 0,  0.0f), new Vector2(1.00f, 0.875f)),


                    new VertexP3T2(new Vector3(il , 0,  0.5f), new Vector2(0.00f, 1.000f)),
                    new VertexP3T2(new Vector3(fr , 0, -1.5f), new Vector2(1.00f, 0.000f)),
                    new VertexP3T2(new Vector3(il , 0, -1.5f), new Vector2(1.00f, 1.000f)),

                    new VertexP3T2(new Vector3(il , 0,  0.5f), new Vector2(0.00f, 1.000f)),
                    new VertexP3T2(new Vector3(fr , 0,  0.5f), new Vector2(0.00f, 0.000f)),
                    new VertexP3T2(new Vector3(fr , 0, -1.5f), new Vector2(1.00f, 0.000f)),


                    new VertexP3T2(new Vector3(fl , 0,  0.5f), new Vector2(0.00f, 0.500f)),
                    new VertexP3T2(new Vector3(fr , 0, -1.5f), new Vector2(1.00f, 1.000f)),
                    new VertexP3T2(new Vector3(fr , 0,  0.5f), new Vector2(0.00f, 1.000f)),

                    new VertexP3T2(new Vector3(fl , 0,  0.5f), new Vector2(0.00f, 0.500f)),
                    new VertexP3T2(new Vector3(fl , 0, -1.0f), new Vector2(0.00f, 0.125f)),
                    new VertexP3T2(new Vector3(frh, 0, -1.0f), new Vector2(0.75f, 0.125f)),
                };
#endif

                mesh.SetIndices(indices);
                mesh.SetVertices(vertices);
            }

            m_drawable = new Drawable3D()
            {
                Texture = skin.GetTexture("textures/laser"),
                Material = skin.GetMaterial("materials/laser"),
                Mesh = mesh,
            };

            m_drawable.Material.BlendMode = BlendMode.Additive;

            m_drawable.Params["LaserColor"] = color;
            m_drawable.Params["HiliteColor"] = new Vector3(1, 1, 0);

            Glow = 0.0f;
            GlowState = 1;
        }

        protected override void SetGlow(float glow)
        {
            m_drawable.Params["GlowState"] = glow;
        }

        protected override void SetGlowState(int glowState)
        {
            m_drawable.Params["GlowState"] = glowState;
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

    internal class LaserRenderState3D : GlowingRenderState3D
    {
        private const float LASER_WIDTH = 2.0f;

        public new AnalogObject Object => (AnalogObject)base.Object;
        
        private Transform m_transform = Transform.Identity;
        private readonly Drawable3D m_drawable;

        public LaserRenderState3D(AnalogObject obj, float len, Vector3 color, ClientResourceManager skin)
            : base(obj)
        {
            Debug.Assert(!obj.IsInstant, "analog for segment render state was a slam");

            m_transform = Transform.Scale(1, 1, len);
            var mesh = new Mesh();

            const float W = LASER_WIDTH / 6.0f;

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
                Texture = skin.GetTexture("textures/laser"),
                Material = skin.GetMaterial("materials/laser"),
                Mesh = mesh,
            };

            m_drawable.Material.BlendMode = BlendMode.Additive;

            m_drawable.Params["LaserColor"] = color;
            m_drawable.Params["HiliteColor"] = new Vector3(1, 1, 0);

            Glow = 0.0f;
            GlowState = 1;
        }

        protected override void SetGlow(float glow)
        {
            m_drawable.Params["GlowState"] = glow;
        }

        protected override void SetGlowState(int glowState)
        {
            m_drawable.Params["GlowState"] = glowState;
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
