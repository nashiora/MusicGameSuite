namespace OpenRM.Voltex
{
    public sealed class ButtonObject : Object
    {
        public ButtonObject Head => FirstConnectedOf<ButtonObject>();
        public ButtonObject Tail => LastConnectedOf<ButtonObject>();

        public bool IsChip => IsInstant;
        public bool IsHold => !IsInstant;

        public bool HasSample => Sample != null;

        private string Sample { get; set; }
    }
}
