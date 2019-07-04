using theori.Charting;

namespace NeuroSonic.Charting
{
    public sealed class ButtonObject : ChartObject
    {
        public ButtonObject Head => FirstConnectedOf<ButtonObject>();
        public ButtonObject Tail => LastConnectedOf<ButtonObject>();

        public bool IsChip => IsInstant;
        public bool IsHold => !IsInstant;

        public bool HasSample => Sample != null;

        public string Sample { get; set; }
    }
}
