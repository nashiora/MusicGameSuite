using System.Collections.Generic;
using System.Linq;

using theori.IO;

namespace theori.Gui
{
    public class GuiManager
    {
        private Panel root;

        private GuiElement currentHover;

        public GuiManager(Panel root)
        {
            this.root = root;
        }
        
        public void Update()
        {
            var underCursor = new SortedList<float, GuiElement>();
            var mousePos = Mouse.Position;
            
            ScanChildren(root);
            void ScanChildren(Panel panel)
            {
                foreach (var child in panel.Children)
                {
                    if (child.ContainsScreenPoint(mousePos))
                        underCursor.Add(0, child);

                    if (child is Panel childPanel)
                        ScanChildren(childPanel);
                }
            }

            var targetChild = underCursor.FirstOrDefault().Value;
            if (currentHover != targetChild)
            {
                if (currentHover != null)
                    currentHover.OnMouseLeave();
                currentHover = targetChild;
                if (currentHover != null)
                    currentHover.OnMouseEnter();
            }

            // TODO(local): track mouse button press/release and how it should interact with hovering

            if (Mouse.IsPressed(MouseButton.Left))
            {
                if (currentHover != null) currentHover.OnMouseButtonPress(MouseButton.Left);
            }
        }
    }
}
