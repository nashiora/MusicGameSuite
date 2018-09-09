using OpenRM.Audio.Effects;

namespace OpenRM.Voltex
{
    public sealed class ButtonObject : Object
    {
        public ButtonObject Head => FirstConnectedOf<ButtonObject>();
        public ButtonObject Tail => LastConnectedOf<ButtonObject>();

        public bool IsChip => IsInstant;
        public bool IsHold => !IsInstant;

        public bool HasSample => Sample != null;
        public bool HasEffect => Effect != null;

        public EffectDef Effect { get; set; }
        private string Sample { get; set; }
    }
}
