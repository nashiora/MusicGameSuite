using System;
using System.Numerics;

using theori.IO;

namespace theori.Gui
{
    public abstract class GuiElement : Disposable
    {
        internal Panel m_parentBacking;
        public Panel Parent
        {
            get => m_parentBacking;
            set
            {
                if (m_parentBacking != null)
                    m_parentBacking.RemoveChild(this);
                m_parentBacking = value;
                if (value != null)
                    m_parentBacking.AddChild(this);
            }
        }

        public Axes RelativePositionAxes;
        public Vector2 Position;

        public Axes RelativeSizeAxes;
        public Vector2 Size;

        public float Rotation;
        public Vector2 Scale = Vector2.One;
        public Vector2 Origin;

        public Vector2 DrawPosition
        {
            get
            {
                float posX = Position.X;
                float posY = Position.Y;

                Vector2 ds = default;
                if (RelativePositionAxes != Axes.None)
                {
                    if (Parent == null)
                        throw new Exception("Cannot use relative axes without a parent.");
                    ds = Parent.ChildDrawSize;
                }
                
                if (RelativePositionAxes.HasFlag(Axes.X))
                    posX = ds.X * posX;
                if (RelativePositionAxes.HasFlag(Axes.Y))
                    posY = ds.Y * posY;

                return new Vector2(posX, posY);
            }
        }

        public Vector2 DrawSize
        {
            get
            {
                float sizeX = Size.X;
                float sizeY = Size.Y;

                Vector2 ds = default;
                if (RelativeSizeAxes != Axes.None)
                {
                    if (Parent == null)
                        throw new Exception("Cannot use relative axes without a parent.");
                    ds = Parent.ChildDrawSize;
                }
                
                if (RelativeSizeAxes.HasFlag(Axes.X))
                    sizeX = ds.X * sizeX;
                if (RelativeSizeAxes.HasFlag(Axes.Y))
                    sizeY = ds.Y * sizeY;

                return new Vector2(sizeX, sizeY);
            }
        }

        public Transform CompleteTransform
        {
            get
            {
                float posX = Position.X;
                float posY = Position.Y;
                
                float sizeX = Size.X;
                float sizeY = Size.Y;

                Vector2 ds = default;
                if (RelativePositionAxes != Axes.None || RelativeSizeAxes != Axes.None)
                {
                    if (Parent == null)
                        throw new Exception("Cannot use relative axes without a parent.");
                    ds = Parent.ChildDrawSize;
                }
                
                if (RelativePositionAxes.HasFlag(Axes.X))
                    posX = ds.X * posX;
                if (RelativePositionAxes.HasFlag(Axes.Y))
                    posY = ds.Y * posY;
                
                if (RelativeSizeAxes.HasFlag(Axes.X))
                    sizeX = ds.X * sizeX;
                if (RelativeSizeAxes.HasFlag(Axes.Y))
                    sizeY = ds.Y * sizeY;

                var result = Transform.Translation(-Origin.X, -Origin.Y, 0)
                           * Transform.Scale(Scale.X, Scale.Y, 0)
                           * Transform.RotationZ(Rotation)
                           * Transform.Translation(posX, posY, 0);

                if (Parent != null)
                    result = result * Parent.CompleteTransform;
                return result;
            }
        }

        public Vector2 ScreenToLocal(Vector2 screen)
        {
            var t = Transform.Translation(-new Vector3(Origin, 0))
                  * Transform.RotationZ(Rotation)
                  * Transform.Scale(new Vector3(Scale, 1));

            var transform = Matrix3x2.CreateTranslation(-Origin) *
                Matrix3x2.CreateRotation(MathL.ToRadians(Rotation)) *
                Matrix3x2.CreateScale(Scale);

            Matrix4x4.Invert(t.Matrix, out var tMat);
            Matrix3x2.Invert(transform, out transform);

            screen -= DrawPosition;
            screen = Vector2.Transform(screen, tMat);

            return screen;
        }

        public bool ContainsScreenPoint(Vector2 screen) =>
            ContainsLocalPoint(ScreenToLocal(screen));

        public bool ContainsLocalPoint(Vector2 local) =>
            local.X >= 0 && local.Y >= 0 && local.X <= Size.X && local.Y <= Size.Y;

        public virtual void Update()
        {
        }

        public virtual void Render(GuiRenderQueue rq)
        {
        }
        
        public virtual bool OnMouseEnter() { return false; }
        public virtual bool OnMouseLeave() { return false; }

        public virtual bool OnMouseButtonPress(MouseButton button) { return false; }
    }

    [Flags]
    public enum Axes
    {
        None = 0,
        X = 1, Y = 2,
        Both = X | Y,
    }
}
