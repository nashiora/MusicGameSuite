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
        public new static AudioSample FromFile(string fileName)
        {
            var fileSource = CodecFactory.Instance.GetCodec(fileName);
            var sampleSource = fileSource.ChangeSampleRate(Application.Mixer.MasterChannel.SampleRate).ToStereo().ToSampleSource();

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

            var sampleSource = source.ChangeSampleRate(Application.Mixer.MasterChannel.SampleRate).ToStereo().ToSampleSource();
            return new AudioSample(sampleSource);
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
