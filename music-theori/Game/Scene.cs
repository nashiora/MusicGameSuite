namespace theori.Game
{
    public abstract class Scene
    {
        public virtual int TargetFrameRate => 0;

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

        public virtual void ClientSizeChanged(int width, int height) { }

        public abstract void Init();
        public abstract void Destroy();
        public abstract void Suspended();
        public abstract void Resumed();
        public virtual void Update() { }
        public virtual void Update(float delta) => Update();
        public virtual void Update(float delta, float total) => Update(delta);
        public abstract void Render();
    }
}
