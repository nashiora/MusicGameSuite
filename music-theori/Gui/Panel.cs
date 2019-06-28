using System.Collections.Generic;
using System.Numerics;

namespace theori.Gui
{
    public class Panel : GuiElement
    {
        private List<GuiElement> m_children = new List<GuiElement>();

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
                foreach (var child in m_children)
                    RemoveChild(child);
                foreach (var child in value)
                    AddChild(child);
            }

            get => m_children;
        }

        public void AddChild(GuiElement gui)
        {
            gui.m_parentBacking = this;
            if (!m_children.Contains(gui))
                m_children.Add(gui);
        }

        public void RemoveChild(GuiElement gui)
        {
            gui.m_parentBacking = null;
            m_children.Remove(gui);
        }

        protected override void DisposeManaged()
        {
            foreach (var child in m_children)
                child.Dispose();
        }

        public override void Update()
        {
            foreach (var child in m_children)
                child.Update();
        }

        public override void Render(GuiRenderQueue rq)
        {
            // TODO(local): scissors aren't enough for rotation things
            //rq.PushScissor();
            foreach (var child in m_children)
                child.Render(rq);
            //rq.PopScissor();
        }
    }
}
