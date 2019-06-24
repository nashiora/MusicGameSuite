using System;
using System.IO;

using CSCore;
using CSCore.Codecs;
using CSCore.Codecs.WAV;

using theori.Audio.NVorbis;

namespace theori.Audio
{
    public class AudioSample : AudioTrack
    {
        internal static new AudioSample CreateUninitialized()
        {
            return new AudioSample();
        }

        public new static AudioSample FromFile(string fileName)
        {
            var fileSource = CodecFactory.Instance.GetCodec(fileName);
            var sampleSource = fileSource.ChangeSampleRate(Host.Mixer.MasterChannel.SampleRate).ToStereo().ToSampleSource();

            return new AudioSample(sampleSource);
        }
        
        public new static AudioSample FromStream(string ext, Stream stream)
        {
            IWaveSource source;
            switch (ext)
            {
                case ".wav": source = new WaveFileReader(stream); break;
                case ".ogg": source = new NVorbisSource(stream).ToWaveSource(); break;
                default: throw new NotImplementedException();
            }

            var sampleSource = source.ChangeSampleRate(Host.Mixer.MasterChannel.SampleRate).ToStereo().ToSampleSource();
            return new AudioSample(sampleSource);
        }

        private AudioSample()
            : base()
        {
            RemoveFromChannelOnFinish = false;
        }

        internal AudioSample(ISampleSource source)
            : base(source)
        {
            RemoveFromChannelOnFinish = false;
        }

        public override void Play()
        {
            Stop();
            Seek(0);
            base.Play();
        }

        protected override void OnRemoveFromMixerChannel()
        {
            Stop();
            Seek(0);
            base.OnRemoveFromMixerChannel();
        }
    }
}
