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
        public virtual void Update() { }
        public virtual void Update(float delta) => Update();
        public virtual void Update(float delta, float total) => Update(delta);
        public abstract void Render();
    }

    public abstract class Overlay : Layer
    {
        public sealed override int TargetFrameRate => 0;

        public sealed override void Suspended() { }
        public sealed override void Resumed() { }
    }
}
