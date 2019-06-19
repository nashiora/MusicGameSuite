using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using OpenGL;

using theori.IO;
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
            
            public float UserAlpha;
            public float UserBeta;
        }

        private bool m_active;

        private GuiRenderQueue m_grq;
        private Vector2 m_viewportSize;

        private int m_verticalOffset;

        private MenuData m_currentMenu;
        private readonly List<MenuData> m_menus = new List<MenuData>();
        private int m_menuIndex;
        private int m_menuXOffset;
        
        private WindowData m_currentWindow;
        private readonly Dictionary<string, WindowData> m_windows = new Dictionary<string, WindowData>();

        private WidgetId? m_hotWidget;
        private WidgetId? m_activeWidget;

        public void BeforeLayout()
        {
            m_viewportSize = new Vector2(Window.Width, Window.Height);

            // reset values which are changed during layouting
            m_verticalOffset = 0;
            m_menuIndex = 0;
            m_menuXOffset = 0;

            // Mark all items for deletion:
            //  if they get used this frame they'll survive.
            foreach (var menu in m_menus)
                menu.SetUnused();
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

        private bool RegionHovered(WindowData window, int x, int y, int w, int h)
        {
            // if window clipping is a thing we want, clip
            if (window != null)
            {
                if (Mouse.X < window.X || Mouse.X > window.X + window.Width ||
                    Mouse.Y < window.Y || Mouse.Y > window.Y + window.Height)
                {
                    return false;
                }
            }

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

        class MenuData
        {
            public bool UsedThisFrame;
            public bool MarkInactive;
            public Dictionary<WidgetId, MenuItemData> MenuItems = new Dictionary<WidgetId, MenuItemData>();

            public string Title;

            public int Index;
            public int X, Width;

            public int CurrentMenuItemIndex;

            public MenuData(string title)
            {
                Title = title;
            }

            public void SetUnused()
            {
                UsedThisFrame = false;
                foreach (var mpair in MenuItems)
                    mpair.Value.UsedThisFrame = false;
            }

            public void RemoveAllUnused()
            {
                var itemsToRemove = new List<WidgetId>();

                // remove inactive widgets
                foreach (var mpair in MenuItems)
                {
                    if (!mpair.Value.UsedThisFrame)
                        itemsToRemove.Add(mpair.Key);
                }
                foreach (var m in itemsToRemove)
                    MenuItems.Remove(m);
            }
        }

        class MenuItemData
        {
            public bool UsedThisFrame;

            public string Title;

            public int Index;
            public Action Callback;

            public MenuItemData(string title)
            {
                Title = title;
            }
        }

        public bool BeginMenuBar()
        {
            if (m_menuIndex == 0)
            {
                // TODO(local): make this a texture so it's easier duh
                m_grq.DrawRect(Transform.Translation(0, 0, 0),
                    new Rect(0, 0, m_viewportSize.X, 19), Texture.Empty,
                    new Vector4(1, 1, 1, 1));
                
                m_grq.DrawRect(Transform.Translation(0, 19, 0),
                    new Rect(0, 0, m_viewportSize.X, 2), Texture.Empty,
                    new Vector4(0.95f, 0.95f, 0.95f, 1));
                
                m_grq.DrawRect(Transform.Translation(0, 21, 0),
                    new Rect(0, 0, m_viewportSize.X, 1), Texture.Empty,
                    new Vector4(0.63f, 0.63f, 0.63f, 1));

                m_verticalOffset += 22;

                return true;
            }
            return false;
        }

        public void EndMenuBar()
        {
            // TODO(local): end something?
        }

        public bool BeginMenu(string menuTitle)
        {
            MenuData menu = m_menus.Find(m => m.Title == menuTitle);
            if (menu == null)
            {
                menu = new MenuData(menuTitle);
                m_menus.Add(menu);
            }
            
            var id = new WidgetId(menuTitle, WidgetId.ParentKind.MenuBar);
            if (menu.UsedThisFrame) return false;

            menu.UsedThisFrame = true;
            menu.MarkInactive = false;
            m_currentMenu = menu;

            menu.Index = m_menuIndex++;
            menu.X = m_menuXOffset;
            menu.Width = 14 + menuTitle.Length * 6;
            menu.CurrentMenuItemIndex = 0;

            m_menuXOffset += menu.Width;

            bool headerHovered = RegionHovered(null, menu.X, 1, menu.Width, 19);

            if (IsActive(id))
            {
                if (Mouse.IsPressed(MouseButton.Left))
                {
                    //SetNotActive(id);
                    menu.MarkInactive = true;
                }
            }
            else if (IsHot(id))
            {
                if (Mouse.IsPressed(MouseButton.Left))
                    SetActive(id);
            }

            if (headerHovered)
            {
                if (m_activeWidget != null && m_activeWidget.Value.Parent == WidgetId.ParentKind.MenuBar)
                    SetActive(id);
                else SetHot(id);
            }
            else SetNotHot(id);

            var hotColor = new Vector4(229 / 255.0f, 243 / 255.0f, 1, 1);
            var hotBorderColor = new Vector4(204 / 255.0f, 232 / 255.0f, 1, 1);
            
            var activeColor = new Vector4(204 / 255.0f, 232 / 255.0f, 1, 1);
            var activeBorderColor = new Vector4(153 / 255.0f, 209 / 255.0f, 1, 1);

            Vector4 color, borderColor;

            if (IsActive(id))
            {
                color = activeColor;
                borderColor = activeBorderColor;
            }
            else if (IsHot(id))
            {
                color = hotColor;
                borderColor = hotBorderColor;
            }
            else goto draw_text;
            
            m_grq.DrawRect(Transform.Translation(menu.X, 1, 0),
                new Rect(0, 0, menu.Width, 19), Texture.Empty,
                borderColor);

            m_grq.DrawRect(Transform.Translation(menu.X + 1, 2, 0),
                new Rect(0, 0, menu.Width - 2, 17), Texture.Empty,
                color);

        draw_text:
            m_grq.DrawRect(Transform.Translation(menu.X + 7, 22 / 2, 0),
                new Rect(0, 0, menu.Width - 14, 1), Texture.Empty,
                new Vector4(0.15f, 0.15f, 0.15f, 1));

            return true;
        }

        public void EndMenu()
        {
            var id = new WidgetId(m_currentMenu.Title, WidgetId.ParentKind.MenuBar);
            m_currentMenu.RemoveAllUnused();

            if (IsActive(id))
            {
                var items = from item in m_currentMenu.MenuItems.Values
                            orderby item.Index select item;

                int yCur = 0;
                // TODO(local): calculate width
                int width = 200;

                foreach (var item in items)
                {
                    int itemHeight = 22;
                    int y = 23 + yCur;

                    yCur += itemHeight;
                    
                    if (RegionHovered(null, m_currentMenu.X + 3, y, width, itemHeight))
                    {
                        if (Mouse.IsPressed(MouseButton.Left))
                        {
                            item.Callback?.Invoke();
                        }

                        m_grq.DrawRect(Transform.Translation(m_currentMenu.X + 3, y, 2),
                            new Rect(0, 0, width, itemHeight), Texture.Empty,
                            new Vector4(144 / 255.0f, 200 / 255.0f, 246 / 255.0f, 1));
                    }
                }

                m_grq.DrawRect(Transform.Translation(m_currentMenu.X, 20, 1),
                    new Rect(0, 0, width + 6, yCur + 6), Texture.Empty,
                    new Vector4(204 / 255.0f, 204 / 255.0f, 204 / 255.0f, 1));
            
                m_grq.DrawRect(Transform.Translation(m_currentMenu.X + 1, 21, 1),
                    new Rect(0, 0, width + 4, yCur + 4), Texture.Empty,
                    new Vector4(242 / 255.0f, 242 / 255.0f, 242 / 255.0f, 1));
            }
            
            if (m_currentMenu.MarkInactive)
                SetNotActive(id);

            m_currentMenu = null;
        }

        public void MenuItem(string itemText, Action itemSelected)
        {
            if (m_currentMenu == null) return;

            // menu isn't open, don't worry about items
            if (m_activeWidget == null ||
                m_activeWidget.Value != new WidgetId(m_currentMenu.Title, WidgetId.ParentKind.MenuBar))
            {
                return;
            }

            var id = new WidgetId($"{ m_currentMenu.Title }|{ itemText }", WidgetId.ParentKind.MenuBar);
            if (!m_currentMenu.MenuItems.TryGetValue(id, out var item))
            {
                item = new MenuItemData(itemText);
                m_currentMenu.MenuItems[id] = item;
            }

            item.UsedThisFrame = true;
            item.Callback = itemSelected;
            item.Index = m_currentMenu.CurrentMenuItemIndex++;
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

            public int Margin = 10;

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
            win.Y = y + m_verticalOffset;
            win.Width = w;
            win.Height = h;
            
            m_grq.DrawRect(Transform.Translation(win.X, win.Y, 0),
                new Rect(0, 0, win.Width, win.Height), Texture.Empty,
                new Vector4(0.0f, 0.3f, 0.4f, 1));

            m_grq.PushScissor(new Rect(x, y, w, h));
        }

        public void WindowMargin(int margin)
        {
            m_currentWindow.Margin = margin;
        }

        public void EndWindow()
        {
            m_grq.PopScissor();

            m_currentWindow.RemoveAllUnused();
            m_currentWindow = null;
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
            data.X = x + m_currentWindow.X + m_currentWindow.Margin;
            data.Y = y + m_currentWindow.Y + m_currentWindow.Margin;
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

            if (RegionHovered(m_currentWindow, data.X, data.Y, data.Width, data.Height))
                SetHot(id);
            else SetNotHot(id);
            
            Vector4 idleColor = Vector4.One,
                    hoverColor = new Vector4(1, 1, 0, 1);

            if (IsHot(id))
                data.UserAlpha = MathL.Min(1, data.UserAlpha + Time.Delta * 4);
            else data.UserAlpha = MathL.Max(0, data.UserAlpha - Time.Delta * 7);

            if (IsActive(id))
                data.UserBeta = MathL.Min(1, data.UserBeta + Time.Delta * 10);
            else data.UserBeta = MathL.Max(0, data.UserBeta - Time.Delta * 4);

            var color = Vector4.Lerp(idleColor, hoverColor, data.UserAlpha);
            color *= (1 - data.UserBeta * 0.25f);
            color.W = 1;

            m_grq.DrawRect(Transform.Translation(data.X, data.Y, 0),
                new Rect(0, 0, data.Width, data.Height), Texture.Empty, color);

            return clickResult;
        }

        #endregion
    }
}
