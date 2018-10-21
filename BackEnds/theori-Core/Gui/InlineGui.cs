using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using OpenGL;
using theori.Graphics;

namespace theori.Gui
{
    public class InlineGui
    {
        struct WidgetId
        {
            public static bool operator !=(WidgetId a, WidgetId b) => !(a == b);
            public static bool operator ==(WidgetId a, WidgetId b)
            {
                return a.UniqueId == b.UniqueId && a.Parent == b.Parent && a.Window == b.Window;
            }

            public enum ParentKind
            {
                MenuBar, ToolBar, Window
            }

            public object UniqueId;

            public ParentKind Parent;
            public WindowData Window;

            public WidgetId(object uid, ParentKind parent, WindowData window = null)
            {
                UniqueId = uid;

                Parent = parent;
                Window = window;
            }
        }

        class WidgetData
        {
            public bool UsedThisFrame;

            public int X, Y;
            public int Width, Height;
        }

        private bool m_active;
        private GuiRenderQueue m_grq;

        private Vector2 m_viewportSize;

        private readonly Dictionary<string, WindowData> m_windows = new Dictionary<string, WindowData>();

        private WindowData m_currentWindow;

        private WidgetId? m_hotWidget;
        private WidgetId? m_activeWidget;

        public void BeforeLayout()
        {
            m_viewportSize = new Vector2(Window.Width, Window.Height);

            // Mark all items for deletion:
            //  if they get used this frame they'll survive.
            foreach (var wpair in m_windows)
                wpair.Value.SetUnused();

            m_grq = new GuiRenderQueue(m_viewportSize);
            m_active = true;
        }

        public void AfterLayout()
        {
            var windowsToRemove = new List<string>();

            // remove inactive windows
            foreach (var wpair in m_windows)
            {
                if (!wpair.Value.UsedThisFrame)
                    windowsToRemove.Add(wpair.Key);
            }
            foreach (string w in windowsToRemove)
                m_windows.Remove(w);

            m_active = false;
        }

        public void Render()
        {
            m_grq.Process(true);
            m_grq = null;
        }

        private bool RegionHovered(int x, int y, int w, int h)
        {
            return Mouse.X >= x && Mouse.X <= x + w && Mouse.Y >= y && Mouse.Y <= y + h;
        }

        #region Widget Activity

        private void SetActive(WidgetId id)
        {
            m_activeWidget = id;
        }

        private void SetNotActive(WidgetId id)
        {
            if (m_activeWidget.HasValue && m_activeWidget.Value == id)
                m_activeWidget = null;
        }

        private void SetHot(WidgetId id)
        {
            // don't set something as hot if there's an active widget.
            if (m_activeWidget != null) return;
            m_hotWidget = id;
        }

        private void SetNotHot(WidgetId id)
        {
            if (m_hotWidget.HasValue && m_hotWidget.Value == id)
                m_hotWidget = null;
        }

        private bool IsHot(WidgetId id)
        {
            if (m_hotWidget == null) return false;
            return m_hotWidget.Value == id;
        }

        private bool IsActive(WidgetId id)
        {
            if (m_activeWidget == null) return false;
            return m_activeWidget.Value == id;
        }

        #endregion

        #region Menu Bar

        public bool BeginMenuBar()
        {
            return true;
        }

        public void EndMenuBar()
        {
        }

        public bool BeginMenu(string menuName)
        {
            return false;
        }

        public void EndMenu()
        {
        }

        public void MenuItem(string itemText, Action itemSelected)
        {
        }

        public bool MenuItem(string itemText)
        {
            return false;
        }

        #endregion

        #region Tool Bar

        public bool BeginToolBar()
        {
            return true;
        }

        public void EndToolBar()
        {
        }

        public void ToolSeparator()
        {
        }

        public bool ToolButton(string id)
        {
            return false;
        }

        #endregion

        #region Dialog Windows

        class WindowData
        {
            public bool UsedThisFrame;
            public Dictionary<WidgetId, WidgetData> Widgets = new Dictionary<WidgetId, WidgetData>();

            public string WindowTitle;
            
            public int X, Y;
            public int Width, Height;

            public WindowData(string windowTitle)
            {
                WindowTitle = windowTitle;
            }

            public void SetUnused()
            {
                UsedThisFrame = false;
                foreach (var wpair in Widgets)
                    wpair.Value.UsedThisFrame = false;
            }

            public void RemoveAllUnused()
            {
                var widgetsToRemove = new List<WidgetId>();

                // remove inactive widgets
                foreach (var wpair in Widgets)
                {
                    if (!wpair.Value.UsedThisFrame)
                        widgetsToRemove.Add(wpair.Key);
                }
                foreach (var w in widgetsToRemove)
                    Widgets.Remove(w);
            }
        }

        public void BeginWindow(string windowTitle, int x, int y, int w, int h)
        {
            if (!m_windows.TryGetValue(windowTitle, out var win))
            {
                win = m_windows[windowTitle] = new WindowData(windowTitle);
            }

            win.UsedThisFrame = true;
            m_currentWindow = win;

            win.X = x;
            win.Y = y;
            win.Width = w;
            win.Height = h;
            
            m_grq.DrawRect(Transform.Translation(x, y, 0),
                new Rect(0, 0, w, h), Texture.Empty, Vector4.One / 2);
        }

        public void EndWindow()
        {
        }

        public bool Button(string buttonText, int x, int y, int w, int h)
        {
            var id = new WidgetId(buttonText, WidgetId.ParentKind.Window, m_currentWindow);
            if (!m_currentWindow.Widgets.TryGetValue(id, out var data))
            {
                data = new WidgetData();
                m_currentWindow.Widgets[id] = data;
            }

            data.UsedThisFrame = true;
            data.X = x + m_currentWindow.X;
            data.Y = y + m_currentWindow.Y;
            data.Width = w;
            data.Height = h;

            bool clickResult = false;

            if (IsActive(id))
            {
                if (Mouse.IsReleased(MouseButton.Left))
                {
                    if (IsHot(id)) clickResult = true;
                    SetNotActive(id);
                }
            }
            else if (IsHot(id))
            {
                if (Mouse.IsPressed(MouseButton.Left))
                    SetActive(id);
            }

            if (RegionHovered(data.X, data.Y, data.Width, data.Height))
                SetHot(id);
            else SetNotHot(id);
            
            var color = Vector4.One;
            if (IsHot(id)) color = new Vector4(1, 1, 0, 1);

            m_grq.DrawRect(Transform.Translation(m_currentWindow.X, m_currentWindow.Y, 0),
                new Rect(data.X, data.Y, data.Width, data.Height), Texture.Empty, color);

            return clickResult;
        }

        #endregion
    }
}
