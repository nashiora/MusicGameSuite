using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using theori.Graphics;
using theori.Gui;
using theori.IO;
using theori.Resources;

namespace theori
{
    public abstract class Layer : IAsyncLoadable
    {
        internal enum LayerLifetimeState
        {
            Uninitialized,
            Queued,
            Alive,
            Destroyed,
        }

        internal LayerLifetimeState lifetimeState = LayerLifetimeState.Uninitialized;

        public virtual int TargetFrameRate => 0;

        public virtual bool BlocksParentLayer => true;

        // TODO(local): THIS ISN'T USED YET, BUT WILL BE AFTER THE LAYER SYSTEM IS FIXED UP
        public bool IsSuspended { get; private set; } = false;

        protected Panel ForegroundGui, BackgroundGui;

        internal void DestroyInternal()
        {
            Destroy();

            ForegroundGui?.Dispose();
            BackgroundGui?.Dispose();
        }

        internal void Suspend()
        {
            if (IsSuspended) return;
            IsSuspended = true;

            Suspended();
        }

        internal void Resume()
        {
            if (!IsSuspended) return;
            IsSuspended = false;

            Resumed();
        }

        internal void RenderInternal()
        {
            void DrawGui(Panel gui)
            {
                if (gui == null) return;

                var viewportSize = new Vector2(Window.Width, Window.Height);
                using (var grq = new GuiRenderQueue(viewportSize))
                {
                    gui.Position = Vector2.Zero;
                    gui.RelativeSizeAxes = Axes.None;
                    gui.Size = viewportSize;
                    gui.Rotation = 0;
                    gui.Scale = Vector2.One;
                    gui.Origin = Vector2.Zero;

                    gui.Render(grq);
                }
            }

            DrawGui(BackgroundGui);
            Render();
            DrawGui(ForegroundGui);
            LateRender();
        }

        internal void UpdateInternal(float delta, float total)
        {
            Update(delta, total);

            BackgroundGui?.Update();
            ForegroundGui?.Update();
        }

        /// <summary>
        /// Called whenever the client window size is changed, even if this layer is suspended.
        /// If being suspended is important, check <see cref="IsSuspended"/>.
        /// </summary>
        public virtual void ClientSizeChanged(int width, int height) { }

        public virtual bool AsyncLoad() { return true; }
        public virtual bool AsyncFinalize() { return true; }

        public virtual void Init() { }
        public virtual void Destroy() { }

        public virtual void Suspended() { }
        public virtual void Resumed() { }

        public virtual bool KeyPressed(KeyInfo info) => false;
        public virtual bool KeyReleased(KeyInfo info) => false;

        public virtual bool ButtonPressed(ButtonInfo info) => false;
        public virtual bool ButtonReleased(ButtonInfo info) => false;
        public virtual bool AxisChanged(AnalogInfo info) => false;

        public virtual void Update(float delta, float total) { }
        public virtual void Render() { }
        public virtual void LateRender() { }
    }

    public abstract class Overlay : Layer
    {
        public sealed override int TargetFrameRate => 0;

        public sealed override void Suspended() { }
        public sealed override void Resumed() { }
    }

    public class GenericTransitionLayer : Layer
    {
        private readonly Layer m_nextLayer;
        private readonly BasicSpriteRenderer m_renderer;

        private AsyncLoader m_loader;

        public GenericTransitionLayer(Layer nextLayer, ClientResourceLocator locator)
        {
            m_nextLayer = nextLayer;
            m_renderer = new BasicSpriteRenderer(locator ?? ClientResourceLocator.Default, new Vector2(Window.Width, Window.Height));
        }

        public override void Destroy()
        {
            m_loader = null;
            m_renderer.Dispose();
        }

        public override void Init()
        {
            m_loader = new AsyncLoader();
            m_loader.Add(m_nextLayer);
            m_loader.LoadAll();
        }

        public override void Update(float delta, float total)
        {
            if (m_loader == null) return;

            m_loader.Update();
            if (m_loader.Failed)
                Host.RemoveLayer(this);
            else if (m_loader.IsCompleted)
            {
                if (m_loader.IsFinalizeSuccessful)
                    Host.AddLayerAbove(this, m_nextLayer);
                Host.RemoveLayer(this);
            }
        }

        public override void Render()
        {
            m_renderer.BeginFrame();
            m_renderer.Translate(-50, -50);
            m_renderer.Rotate(Time.Total * 180);
            m_renderer.Translate(Window.Width / 2.0f, Window.Height / 2.0f);
            m_renderer.FillRect(0, 0, 100, 100);
            m_renderer.Flush();
            m_renderer.EndFrame();
        }
    }
}
