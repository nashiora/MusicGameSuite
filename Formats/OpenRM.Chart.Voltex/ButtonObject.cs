namespace OpenRM.Voltex
{
    public sealed class ButtonObject : Object
    {
        private string m_chipSample;

        public ButtonObject Head => FirstConnectedOf<ButtonObject>();
        public ButtonObject Tail => LastConnectedOf<ButtonObject>();

        public bool IsChip => IsInstant;

        public bool HasChipSample => IsInstant && m_chipSample != null;
    }
}
