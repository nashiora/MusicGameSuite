using theori;
using theori.GameModes;

using NeuroSonic.GamePlay;

namespace NeuroSonic
{
    public sealed class NeuroSonicDescription : GameModeDescription
    {
        public static GameModeDescription Instance { get; } = new NeuroSonicDescription();

        public override bool SupportsStandaloneUsage => true;
        public override bool SupportsSharedUsage => true;

        public NeuroSonicDescription()
            : base("NeuroSonic")
        {
        }

        public override void InvokeStandalone(string[] args) => Plugin.NSC_Main(args);
        public override Layer CreateSharedGameLayer() => new GameLayer(AutoPlay.None);
    }
}
