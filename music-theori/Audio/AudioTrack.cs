using System;
using System.IO;

using CSCore;
using CSCore.Codecs;
using CSCore.Codecs.WAV;
using OpenRM;

namespace theori.Audio
{
    public enum PlaybackState
    {
        Stopped = 0,
        Playing,
    }

    public class AudioTrack : AudioSource
    {
        internal static AudioTrack CreateUninitialized()
        {
            return new AudioTrack();
        }

        public static AudioTrack FromFile(string fileName)
        {
            var fileSource = CodecFactory.Instance.GetCodec(fileName);
            var sampleSource = fileSource.ChangeSampleRate(Host.Mixer.MasterChannel.SampleRate).ToStereo().ToSampleSource();

            return new AudioTrack(sampleSource);
        }
        
        public static AudioTrack FromStream(string ext, Stream stream)
        {
            IWaveSource source;
            switch (ext)
            {
                case ".wav": source = new WaveFileReader(stream); break;
                case ".ogg": source = new NVorbis.NVorbisSource(stream).ToWaveSource(); break;
                default: throw new NotImplementedException();
            }

            var sampleSource = source.ChangeSampleRate(Host.Mixer.MasterChannel.SampleRate).ToStereo().ToSampleSource();
            return new AudioTrack(sampleSource);
        }
        
        protected internal ISampleSource Source { get; private set; }

        public override bool CanSeek => Source.CanSeek;
        
        public override int SampleRate => Source.WaveFormat.SampleRate;
        public override int Channels => Source.WaveFormat.Channels;

        internal WaveFormat WaveFormat => Source.WaveFormat;

        private TimeSpan m_lastSourcePosition;

        private time_t m_positionCached;
        public override time_t Position
        {
            get
            {
                if (PlaybackState != PlaybackState.Playing && m_positionCached >= 0)
                    return m_positionCached;
                return GetPosition().TotalSeconds;
            }

            set
            {
                if (PlaybackState != PlaybackState.Playing)
                    m_positionCached = value;
                Seek(value); 
            }
        }

        public override time_t Length => GetLength().TotalSeconds;

        public PlaybackState PlaybackState { get; private set; } = PlaybackState.Stopped;

        private float m_playbackSpeed = 1, m_invPlaybackSpeed = 1;
        public float PlaybackSpeed
        {
            get => m_playbackSpeed;
            set => m_invPlaybackSpeed = 1 / (m_playbackSpeed = MathL.Clamp(value, 0.1f, 9999));
        }

        internal AudioTrack()
        {
        }

        internal AudioTrack(ISampleSource source)
        {
            Source = source;
        }

        /// <summary>
        /// If this track already has a source, it will dispose of the input source for
        ///  you since it will not take ownership.
        /// I guess that IS still taking ownership, you just don't need to worry about it.
        /// </summary>
        internal void SetSourceFromStream(Stream stream, string ext)
        {
            if (Source != null)
            {
                // kill it sorry
                stream.Dispose();
                return;
            }

            IWaveSource source;
            switch (ext)
            {
                case ".wav": source = new WaveFileReader(stream); break;
                case ".ogg": source = new NVorbis.NVorbisSource(stream).ToWaveSource(); break;
                default: throw new NotImplementedException();
            }

            Source = source.ChangeSampleRate(Host.Mixer.MasterChannel.SampleRate).ToStereo().ToSampleSource();
        }

        public TimeSpan GetPosition() => m_lastSourcePosition;
        public TimeSpan GetLength() => ((IAudioSource)Source).GetLength();

        public virtual void Play()
        {
            if (PlaybackState == PlaybackState.Playing)
                return;

            m_positionCached = -1;
            PlaybackState = PlaybackState.Playing;
        }

        public virtual void Stop()
        {
            if (PlaybackState == PlaybackState.Stopped)
                return;

            PlaybackState = PlaybackState.Stopped;
        }

        private long m_realSampleIndex;
        private float[] m_resampleBuffer = new float[2048];

        public override int Read(float[] buffer, int offset, int count)
        {
            switch (PlaybackState)
            {
                case PlaybackState.Playing:
                {
                    m_lastSourcePosition = TimeSpan.FromSeconds(m_realSampleIndex / (double)(Source.WaveFormat.SampleRate * Source.WaveFormat.Channels));

                    int realSampleCount = (int)(count * m_playbackSpeed);
                    if (m_resampleBuffer.Length < realSampleCount)
                    {
                        int newLen = m_resampleBuffer.Length;
                        while (newLen < realSampleCount)
                            newLen *= 2;
                        Array.Resize(ref m_resampleBuffer, newLen);
                    }
                    
                    float LerpSample(float[] arr, double index)
                    {
                        index = MathL.Clamp(index, 0, arr.Length);
                        if (index == 0) return arr[0];
                        if (index == arr.Length) return arr[arr.Length - 1];
                        int min = (int)index, max = min + 1;
                        return MathL.Lerp(arr[min], arr[max], (float)(index - min));
                    }

                    int numEmptySamples = (int)(MathL.Clamp(-(int)m_realSampleIndex, 0, count) * m_playbackSpeed);
                    for (int e = 0; e < numEmptySamples; e++)
                        m_resampleBuffer[e] = 0;

                    int numReadSamples = Source.Read(m_resampleBuffer, numEmptySamples, realSampleCount - numEmptySamples);
                    int totalSamplesRead = numReadSamples + numEmptySamples;

                    int numSamplesToWrite = (int)(totalSamplesRead * m_invPlaybackSpeed);
                    for (int i = 0; i < numSamplesToWrite; i++)
                        buffer[i] = LerpSample(m_resampleBuffer, i * m_playbackSpeed) * Volume;

                    m_realSampleIndex += totalSamplesRead;
                    return numSamplesToWrite;
                }

                case PlaybackState.Stopped:
                    for (int i = 0; i < count; i++)
                        buffer[offset + i] = 0;
                    return count;

                default: return 0;
            }
        }

        public override void Seek(time_t position)
        {
            if (!CanSeek) throw new InvalidOperationException("cannot seek");
            
            double posSeconds = MathL.Max(0, position.Seconds);
            ((IAudioSource)Source).SetPosition(TimeSpan.FromSeconds(posSeconds));

            m_lastSourcePosition = TimeSpan.FromSeconds(position.Seconds);
            m_realSampleIndex = (long)(position.Seconds * Source.WaveFormat.SampleRate * Source.WaveFormat.Channels);
        }

        protected override void DisposeManaged()
        {
            Channel = null;
            Source.Dispose();
        }
    }
}
