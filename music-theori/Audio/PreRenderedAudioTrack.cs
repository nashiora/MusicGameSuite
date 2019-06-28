using System;

namespace theori.Audio
{
    public sealed class PreRenderedAudioTrack : AudioSource
    {
        private readonly float[] m_sourceData;
        private readonly float[] m_data;
        private int m_sampleIndex;

        private time_t m_position;
        
        public override bool CanSeek => true;

        public override int SampleRate { get; }
        public override int Channels { get; } 

        public override time_t Position { get => m_position; set => Seek(value); }
        public override time_t Length { get; }

        public bool IsPlaying { get; private set; } = false;

        public PreRenderedAudioTrack(AudioTrack source)
        {
            SampleRate = source.SampleRate;
            Channels = source.Channels;

            Length = source.Length;

            int dataCount = (int)((double)source.Length * SampleRate) * Channels;
            Logger.Log(dataCount);

            m_sourceData = new float[dataCount];

            source.Play();
            source.Read(m_sourceData, 0, dataCount);

            m_data = new float[dataCount];

            // temp copy into the data array
            Array.Copy(m_sourceData, m_data, dataCount);
        }

        public void Play()
        {
            IsPlaying = true;
        }
        
        public override void Seek(time_t position)
        {
            m_sampleIndex = (int)((Channels * SampleRate) * (double)position);
        }

        public override int Read(float[] buffer, int offset, int count)
        {
            if (!IsPlaying)
            {
                for (int i = offset; i < offset + count; i++)
                    buffer[i] = 0;
                return count;
            }

            count = Math.Min(count, m_data.Length - m_sampleIndex);
            if (count <= 0) return 0;

            for (int i = 0; i < count; i++)
                buffer[i + offset] = m_data[i + m_sampleIndex];

            m_sampleIndex += count;
            m_position = (double)m_sampleIndex / (Channels * SampleRate);

            return count;
        }
    }
}
