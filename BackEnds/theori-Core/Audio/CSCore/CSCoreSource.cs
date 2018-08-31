using System;
using System.IO;

using CSCore;
using CSCore.Codecs;
using CSCore.Codecs.WAV;
using OpenRM;

namespace theori.Audio.CSCore
{
    public enum PlaybackState
    {
        Stopped = 0,
        Playing,
    }

    public class CSCoreSource : AudioSource
    {
        public static CSCoreSource FromFile(string fileName)
        {
            var fileSource = CodecFactory.Instance.GetCodec(fileName);
            var sampleSource = fileSource.ChangeSampleRate(Application.Mixer.MasterChannel.SampleRate).ToStereo().ToSampleSource();

            return new CSCoreSource(sampleSource);
        }
        
        public static CSCoreSource FromStream(string ext, Stream stream)
        {
            IWaveSource source;
            switch (ext)
            {
                case ".wav": source = new WaveFileReader(stream); break;
                case ".ogg": source = new NVorbis.NVorbisSource(stream).ToWaveSource(); break;
                default: throw new NotImplementedException();
            }

            var sampleSource = source.ChangeSampleRate(Application.Mixer.MasterChannel.SampleRate).ToStereo().ToSampleSource();
            return new CSCoreSource(sampleSource);
        }
        
        private ISampleSource Source { get; }

        public override bool CanSeek => Source.CanSeek;
        
        public override int SampleRate => Source.WaveFormat.SampleRate;
        public override int Channels => Source.WaveFormat.Channels;

        internal WaveFormat WaveFormat => Source.WaveFormat;

        private TimeSpan lastSourcePosition;

        private time_t positionCached;
        public override time_t PositionMicros
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

        public override time_t LengthMicros => GetLength().TotalSeconds;
        
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

        internal CSCoreSource(ISampleSource source)
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

        public override int Read(float[] buffer, int offset, int count)
        {
            switch (PlaybackState)
            {
                case PlaybackState.Playing:
                    {
                        lastSourcePosition = ((IAudioSource)Source).GetPosition();

                        int result = Source.Read(buffer, offset, count);
                        for (int i = 0; i < result; i++)
                            buffer[i] *= Volume;
                        return result;
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

            ((IAudioSource)Source).SetPosition(lastSourcePosition = TimeSpan.FromSeconds(position.Seconds));
        }

        protected override void DisposeManaged()
        {
            Channel = null;
            Source.Dispose();
        }
    }
}
