namespace theori.Audio
{
    public abstract class Dsp
    {
        public int SampleRate { get; }

        public float Mix { get; set; } = 0.5f;

        protected Dsp(int sampleRate)
        {
            SampleRate = sampleRate;
        }

        public virtual void Reset() { }

        public void Process(float[] buffer, int offset, int count)
        {
            ProcessImpl(buffer, offset, count);
        }

        protected abstract void ProcessImpl(float[] buffer, int offset, int count);
    }
}
