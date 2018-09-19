using System;
using System.Numerics;
using OpenRM;
using theori.Graphics;

namespace theori.Game
{
    internal abstract class ObjectRenderable3D : Disposable
    {
        public readonly OpenRM.Object Object;
        
        public abstract Transform Transform { get; }

        private time_t m_vd;
        public time_t ViewDuration
        {
            get => m_vd;
            set
            {
                m_vd = value;
                UpdateDuration((float)(Object.AbsoluteDuration / m_vd));
            }
        }

        public Mesh Mesh { get; protected set; }

        protected ObjectRenderable3D(OpenRM.Object obj, float lenPerc)
        {
            Object = obj;
            obj.PropertyChanged += UpdateRenderState_Base;
        }

        public abstract void UpdateRenderState(OpenRM.Object.PropertyChangedEventArgs args);
        public abstract void UpdateDuration(float len);

        private void UpdateRenderState_Base(OpenRM.Object obj, OpenRM.Object.PropertyChangedEventArgs args)
        {
            UpdateRenderState(args);
        }

        protected override void DisposeManaged()
        {
            Object.PropertyChanged -= UpdateRenderState_Base;
        }
    }

    internal class ButtonRenderState3D : ObjectRenderable3D
    {
        public new OpenRM.Voltex.ButtonObject Object => (OpenRM.Voltex.ButtonObject)base.Object;
        
        private Transform m_transform = Transform.Identity;
        public override Transform Transform => m_transform;

        public ButtonRenderState3D(OpenRM.Voltex.ButtonObject obj, Mesh mesh, float len)
            : base(obj, len)
        {
            if (!Object.IsInstant) m_transform = Transform.Scale(1, 1, len);
            Mesh = mesh;
        }

        public override void UpdateRenderState(OpenRM.Object.PropertyChangedEventArgs args)
        {
        }

        public override void UpdateDuration(float len)
        {
            if (!Object.IsInstant) m_transform = Transform.Scale(1, 1, len);
        }
    }

    internal class SlamRenderState3D : ObjectRenderable3D
    {
        public new OpenRM.Voltex.AnalogObject Object => (OpenRM.Voltex.AnalogObject)base.Object;
        
        private Transform m_transform = Transform.Identity;
        public override Transform Transform => m_transform;

        public SlamRenderState3D(OpenRM.Voltex.AnalogObject obj, float len)
            : base(obj, len)
        {
            System.Diagnostics.Debug.Assert(obj.IsInstant, "analog for slam render state wasn't a slam");
            
            float range = 5 / 6.0f * (obj.RangeExtended ? 2 : 1);

            float min = MathL.Min(obj.InitialValue, obj.FinalValue) * range;
            
            m_transform = Transform.Scale(1, 1, len);
            //Mesh = Mesh.CreatePlane(Vector3.UnitX, Vector3.UnitZ, width, 1, Anchor.BottomCenter);
            Mesh = new Mesh();

            const float W = 1 / 6.0f;
            
            float il = range * (obj.InitialValue - 0.5f) - W / 2;
            float ir = range * (obj.InitialValue - 0.5f) + W / 2;
            
            float fl = range * (obj.FinalValue - 0.5f) - W / 2;
            float fr = range * (obj.FinalValue - 0.5f) + W / 2;

            if (obj.InitialValue < obj.FinalValue)
            {
                ushort[] indices = new ushort[] { 0, 1, 2, 1, 5, 2, 2, 5, 4, 3, 4, 5 };
                Mesh.SetIndices(indices);
            
                VertexP3T2[] vertices = new VertexP3T2[6]
                {
                    new VertexP3T2(new Vector3(il, 0, 0), new Vector2(0, 1)),
                    new VertexP3T2(new Vector3(il, 0, -1), new Vector2(0, 0)),
                    new VertexP3T2(new Vector3(ir, 0, 0), new Vector2(1, 1)),

                    new VertexP3T2(new Vector3(fr, 0, -1), new Vector2(1, 0)),
                    new VertexP3T2(new Vector3(fr, 0, 0), new Vector2(1, 1)),
                    new VertexP3T2(new Vector3(fl, 0, -1), new Vector2(0, 1)),
                };
                Mesh.SetVertices(vertices);
            }
            else
            {
            {
                ushort[] indices = new ushort[] { 0, 1, 2, 4, 1, 0, 4, 0, 5, 3, 4, 5 };
                Mesh.SetIndices(indices);
            
                VertexP3T2[] vertices = new VertexP3T2[6]
                {
                    new VertexP3T2(new Vector3(il, 0, 0), new Vector2(0, 1)),
                    new VertexP3T2(new Vector3(ir, 0, -1), new Vector2(1, 0)),
                    new VertexP3T2(new Vector3(ir, 0, 0), new Vector2(1, 1)),

                    new VertexP3T2(new Vector3(fl, 0, -1), new Vector2(0, 0)),
                    new VertexP3T2(new Vector3(fr, 0, -1), new Vector2(1, 0)),
                    new VertexP3T2(new Vector3(fl, 0, 0), new Vector2(0, 1)),
                };
                Mesh.SetVertices(vertices);
            }
            }
        }

        public override void UpdateRenderState(OpenRM.Object.PropertyChangedEventArgs args)
        {
        }

        public override void UpdateDuration(float len)
        {
        }
    }

    internal class LaserRenderState3D : ObjectRenderable3D
    {
        public new OpenRM.Voltex.AnalogObject Object => (OpenRM.Voltex.AnalogObject)base.Object;
        
        private Transform m_transform = Transform.Identity;
        public override Transform Transform => m_transform;

        public LaserRenderState3D(OpenRM.Voltex.AnalogObject obj, float len)
            : base(obj, 0)
        {
            System.Diagnostics.Debug.Assert(!obj.IsInstant, "analog for segment render state was a slam");

            m_transform = Transform.Scale(1, 1, len);
            Mesh = new Mesh();

            const float W = 1 / 6.0f;

            ushort[] indices = new ushort[] { 0, 1, 2, 0, 2, 3, };
            Mesh.SetIndices(indices);
            
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

            Mesh.SetVertices(vertices);
        }

        public override void UpdateRenderState(OpenRM.Object.PropertyChangedEventArgs args)
        {
        }

        public override void UpdateDuration(float len)
        {
        }
    }
}
