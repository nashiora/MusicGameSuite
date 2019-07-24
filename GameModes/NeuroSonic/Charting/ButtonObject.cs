using theori.Charting;

namespace NeuroSonic.Charting
{
    [EntityType("Button")]
    public sealed class ButtonObject : Entity
    {
        public ButtonObject Head => FirstConnectedOf<ButtonObject>();
        public ButtonObject Tail => LastConnectedOf<ButtonObject>();

        public bool IsChip => IsInstant;
        public bool IsHold => !IsInstant;

        public bool HasSample => Sample != null;

        [TheoriIgnoreDefault]
        [TheoriProperty("sample")]
        public string Sample { get; set; }
    }
}
