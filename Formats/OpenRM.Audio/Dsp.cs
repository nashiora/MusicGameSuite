namespace OpenRM.Audio
{
    public abstract class Dsp
    {
        public int SampleRate { get; }

        public bool Enabled { get; set; } = true;
        public float Mix { get; set; } = 0.5f;

        protected Dsp(int sampleRate)
        {
            SampleRate = sampleRate;
        }

        public void Process(float[] buffer, int offset, int count)
        {
            if (!Enabled) return;
            ProcessImpl(buffer, offset, count);
        }

        protected abstract void ProcessImpl(float[] buffer, int offset, int count);
    }
}
