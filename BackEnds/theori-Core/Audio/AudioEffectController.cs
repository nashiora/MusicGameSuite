using System;

using OpenRM;
using OpenRM.Audio;
using OpenRM.Audio.Effects;

namespace theori.Audio
{
    public class AudioEffectController : AudioSource
    {
        public AudioTrack Track { get; }
        public bool OwnsTrack { get; }

        public PlaybackState PlaybackState => Track.PlaybackState;

        public override bool CanSeek => Track.CanSeek;

        public override int SampleRate => Track.SampleRate;
        public override int Channels => Track.Channels;

        public override time_t Position { get => Track.Position; set => Track.Position = value; }
        public override time_t Length => Track.Length;
        
        private MixerChannel channel;
        public MixerChannel Channel
        {
            get => channel;
            set
            {
                if (channel == value) return;
                if (channel != null)
                    channel.RemoveSource(this);

                channel = value;
                if (channel != null)
                    channel.AddSource(this);
                else Stop();
            }
        }
        
        public bool EffectsActive { get; set; } = true;

        private readonly EffectDef[] effectDefs;
        private readonly Dsp[] dsps;

        public EffectDef this[int i]
        {
            get => effectDefs[i];
            set => SetEffect(i, value);
        }

        public AudioEffectController(int effectCount, AudioTrack track, bool ownsTrack = true)
        {
            effectDefs = new EffectDef[effectCount];
            dsps = new Dsp[effectCount];

            Track = track;
            OwnsTrack = ownsTrack;

            Channel = track.Channel;
            track.Channel = null;
        }

        public void RemoveEffect(int i)
        {
            var f = effectDefs[i];
            if (f == null)
                return;
            
            effectDefs[i] = null;
            dsps[i] = null;
        }

        public void SetEffect(int i, EffectDef f, float mix = 1)
        {
            if (f == effectDefs[i])
                return;

            RemoveEffect(i);

            effectDefs[i] = f;
            dsps[i] = f.CreateEffectDsp(SampleRate);

            SetEffectMix(i, mix);
            UpdateEffect(i);
        }

        public void UpdateEffect(int i, float alpha = 0)
        {
            var f = effectDefs[i];
            if (f == null)
                return;

            alpha = MathL.Clamp(alpha, 0, 1);
            f.ApplyToDsp(dsps[i], alpha);
        }

        public void SetEffectMix(int i, float mix)
        {
            var dsp = dsps[i];
            if (dsp == null)
                return;

            mix = MathL.Clamp(mix, 0, 1);
            dsp.Mix = mix;
        }

        public void Play() => Track.Play();
        public void Stop() => Track.Stop();

        private float[] copyBuffer = new float[1024];

        public override int Read(float[] buffer, int offset, int count)
        {
            if (count > copyBuffer.Length)
                copyBuffer = new float[count];

            int result = Track.Read(buffer, offset, count);
            Array.Copy(buffer, offset, copyBuffer, 0, result);

            foreach (var effect in dsps)
            {
                if (effect == null)
                    continue;

                effect.Process(copyBuffer, offset, result);

                // Always process the effects to keep timing, but don't always mix them in.
                if (EffectsActive)
                {
                    float mix = effect.Mix;
                    for (int i = 0; i < result; i++)
                    {
                        float original = buffer[offset + i];
                        float processed = copyBuffer[i];
                        buffer[offset + i] = original + (processed - original) * mix;
                    }
                
                    Array.Copy(buffer, offset, copyBuffer, 0, result);
                }
            }

            return result;
        }

        public override void Seek(time_t positionMicros) => Track.Seek(positionMicros);

        protected override void DisposeManaged()
        {
            if (OwnsTrack)
                Track.Dispose();
        }
    }
}
