using theori;
using theori.Game.Scenes;

namespace NeuroSonic.GamePlay
{
    public sealed class GameLayer : Layer
    {
        public override int TargetFrameRate => 288;

        public override bool BlocksParentLayer => true;

        internal GameLayer(bool invokedFromStandalone)
        {
        }

        public override void ClientSizeChanged(int width, int height)
        {
        }

        public override void Init()
        {
            Host.PushLayer(new VoltexEdit());
        }

        public override void Destroy()
        {
        }

        public override void Suspended()
        {
        }

        public override void Resumed()
        {
        }

        public override void Update(float delta, float total)
        {
        }

        public override void Render()
        {
        }
    }
}
