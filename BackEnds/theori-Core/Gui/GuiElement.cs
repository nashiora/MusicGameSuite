using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace theori.Gui
{
    public abstract class GuiElement
    {
        internal Panel _parentBacking;
        public Panel Parent
        {
            get => _parentBacking;
            set
            {
                if (_parentBacking != null)
                    _parentBacking.RemoveChild(this);
                _parentBacking = value;
                if (value != null)
                    _parentBacking.AddChild(this);
            }
        }

        public Axes RelativePositionAxes;
        public Vector2 Position;

        public Axes RelativeSizeAxes;
        public Vector2 Size;

        public float Rotation;
        public Vector2 Scale = Vector2.One;
        public Vector2 Origin;

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
                           * Transform.Translation(Position.X, Position.Y, 0);

                if (Parent != null)
                    result = result * Parent.CompleteTransform;
                return result;
            }
        }

        public Vector2 ScreenToLocal(Vector2 screen)
        {
            var transform = Matrix3x2.CreateTranslation(-Origin) *
                Matrix3x2.CreateRotation(MathL.ToRadians(Rotation)) *
                Matrix3x2.CreateScale(Scale);

            Matrix3x2.Invert(transform, out transform);

            screen -= Position;
            screen = Vector2.Transform(screen, transform);

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
    }

    [Flags]
    public enum Axes
    {
        None = 0,
        X = 1, Y = 2,
        Both = X | Y,
    }
}
