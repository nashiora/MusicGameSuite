using System.Collections.Generic;
using System.Numerics;

namespace theori.Gui
{
    public class Panel : GuiElement
    {
        private List<GuiElement> children = new List<GuiElement>();

        public Vector2 ChildDrawSize
        {
            get
            {
                if (Parent == null)
                    return Size;

                float sizeX = Size.X;
                float sizeY = Size.Y;
                
                if (RelativeSizeAxes.HasFlag(Axes.X))
                    sizeX = Parent.ChildDrawSize.X * sizeX;
                if (RelativeSizeAxes.HasFlag(Axes.Y))
                    sizeY = Parent.ChildDrawSize.Y * sizeY;

                return new Vector2(sizeX, sizeY);
            }
        }

        public IEnumerable<GuiElement> Children
        {
            set
            {
                foreach (var child in children)
                    RemoveChild(child);
                foreach (var child in value)
                    AddChild(child);
            }
        }

        public void AddChild(GuiElement gui)
        {
            gui._parentBacking = this;
            if (!children.Contains(gui))
                children.Add(gui);
        }

        public void RemoveChild(GuiElement gui)
        {
            gui._parentBacking = null;
            children.Remove(gui);
        }

        public override void Update()
        {
            foreach (var child in children)
                child.Update();
        }

        public override void Render(GuiRenderQueue rq)
        {
            // TODO(local): scissors aren't enough for rotation things
            //rq.PushScissor();
            foreach (var child in children)
                child.Render(rq);
            //rq.PopScissor();
        }
    }
}
