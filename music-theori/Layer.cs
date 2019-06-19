using System.Collections.Generic;
using System.Numerics;
using theori.Graphics;
using theori.Gui;
using theori.IO;

namespace theori
{
    public abstract class Layer
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

            Resume();
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

        public abstract void Init();
        public abstract void Destroy();
        public abstract void Suspended();
        public abstract void Resumed();
        public virtual bool KeyPressed(KeyInfo info) => false;
        public virtual bool KeyReleased(KeyInfo info) => false;
        public virtual bool ButtonPressed(ButtonInfo info) => false;
        public virtual bool ButtonReleased(ButtonInfo info) => false;
        public virtual bool AxisChanged(AnalogInfo info) => false;
        public abstract void Update(float delta, float total);
        public abstract void Render();
    }

    public abstract class Overlay : Layer
    {
        public sealed override int TargetFrameRate => 0;

        public sealed override void Suspended() { }
        public sealed override void Resumed() { }
    }
}
