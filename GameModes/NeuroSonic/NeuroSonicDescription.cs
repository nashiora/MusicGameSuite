using theori.GameModes;

namespace NeuroSonic
{
    public sealed class NeuroSonicDescription : GameModeDescription
    {
        public override bool SupportsStandaloneStartup => true;

        internal NeuroSonicDescription()
            : base("NeuroSonic")
        {
        }
    }
}
