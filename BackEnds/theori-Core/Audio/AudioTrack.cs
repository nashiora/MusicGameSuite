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
        public static AudioTrack FromFile(string fileName)
        {
            var fileSource = CodecFactory.Instance.GetCodec(fileName);
            var sampleSource = fileSource.ChangeSampleRate(Application.Mixer.MasterChannel.SampleRate).ToStereo().ToSampleSource();

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

            var sampleSource = source.ChangeSampleRate(Application.Mixer.MasterChannel.SampleRate).ToStereo().ToSampleSource();
            return new AudioTrack(sampleSource);
        }
        
        private ISampleSource Source { get; }

        public override bool CanSeek => Source.CanSeek;
        
        public override int SampleRate => Source.WaveFormat.SampleRate;
        public override int Channels => Source.WaveFormat.Channels;

        internal WaveFormat WaveFormat => Source.WaveFormat;

        private TimeSpan lastSourcePosition;

        private time_t positionCached;
        public override time_t Position
        {
            get
            {
                if (PlaybackState != PlaybackState.Playing && positionCached >= 0)
                    return positionCached;
                return GetPosition().TotalSeconds;
            }

            set
            {
                if (PlaybackState != PlaybackState.Playing)
                    positionCached = value;
                Seek(value); 
            }
        }

        public override time_t Length => GetLength().TotalSeconds;
        
        private MixerChannel channel;
        public MixerChannel Channel
        {
            get => channel;
            set
            {
                if (channel == value) return;
                if (channel != null)
                {
                    channel.RemoveSource(this);
                    channel.OnSampleSourceEnded -= OnRemoveFromMixerChannelEvent;
                }

                channel = value;
                if (channel != null)
                {
                    channel.AddSource(this);
                    channel.OnSampleSourceEnded += OnRemoveFromMixerChannelEvent;
                }
                else Stop();
            }
        }

        public PlaybackState PlaybackState { get; private set; } = PlaybackState.Stopped;

        internal AudioTrack(ISampleSource source)
        {
            Source = source;
        }

        private void OnRemoveFromMixerChannelEvent(AudioSource track) => OnRemoveFromMixerChannel();
        protected virtual void OnRemoveFromMixerChannel()
        {
        }

        public TimeSpan GetPosition() => lastSourcePosition;
        public TimeSpan GetLength() => ((IAudioSource)Source).GetLength();

        public virtual void Play()
        {
            if (PlaybackState == PlaybackState.Playing)
                return;

            positionCached = -1;
            PlaybackState = PlaybackState.Playing;
        }

        public void Stop()
        {
            if (PlaybackState == PlaybackState.Stopped)
                return;

            PlaybackState = PlaybackState.Stopped;
        }

        private long m_realSampleIndex;

        public override int Read(float[] buffer, int offset, int count)
        {
            switch (PlaybackState)
            {
                case PlaybackState.Playing:
                {
                    if (m_realSampleIndex < 0)
                    {
                        lastSourcePosition = TimeSpan.FromSeconds(m_realSampleIndex / (double)(Source.WaveFormat.SampleRate * Source.WaveFormat.Channels));

                        if (-m_realSampleIndex >= count)
                        {
                            for (int i = 0; i < count; i++)
                                buffer[offset + i] = 0;
                            m_realSampleIndex += count;
                            return count;
                        }
                        else
                        {
                            int numEmptySamples = -(int)m_realSampleIndex;
                            for (int i = 0; i < numEmptySamples; i++)
                                buffer[offset + i] = 0;

                            offset += numEmptySamples;
                            count -= numEmptySamples;

                            int result = Source.Read(buffer, offset, count);
                            for (int i = 0; i < result; i++)
                                buffer[offset + i] *= Volume;

                            m_realSampleIndex += result + numEmptySamples;
                            return numEmptySamples + result;
                        }
                    }
                    else
                    {
                        lastSourcePosition = ((IAudioSource)Source).GetPosition();

                        int result = Source.Read(buffer, offset, count);
                        for (int i = 0; i < result; i++)
                            buffer[i + offset] *= Volume;

                        m_realSampleIndex += result;
                        return result;
                    }
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

            lastSourcePosition = TimeSpan.FromSeconds(position.Seconds);
            m_realSampleIndex = (long)(position.Seconds * Source.WaveFormat.SampleRate * Source.WaveFormat.Channels);
        }

        protected override void DisposeManaged()
        {
            Channel = null;
            Source.Dispose();
        }
    }
}
