#define USE_MULTIPLE_EFFECT_BUFFERS

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

        #if false
        private double m_playbackSpeed = 1, m_invPlaybackSpeed = 1;
        public double PlaybackSpeed
        {
            get => m_playbackSpeed;
            set => m_invPlaybackSpeed = 1 / (m_playbackSpeed = MathL.Clamp(value, 0.1f, 9999));
        }
        #endif

        private readonly EffectDef[] m_effectDefs;
        private readonly float[] m_effectParameters;
        private readonly float[] m_effectMixes;
        private readonly bool[] m_effectsActive;
        private readonly Dsp[] m_dsps;

        public AudioEffectController(int effectCount, AudioTrack track, bool ownsTrack = true)
        {
            m_effectDefs = new EffectDef[effectCount];
            m_effectParameters = new float[effectCount];
            m_effectMixes = new float[effectCount].Fill(1.0f);
            m_effectsActive = new bool[effectCount].Fill(true);
            m_dsps = new Dsp[effectCount];

            Track = track;
            OwnsTrack = ownsTrack;

            Channel = track.Channel;
            track.Channel = null;
        }

        public void RemoveEffect(int i)
        {
            var f = m_effectDefs[i];
            if (f == null)
                return;
            
            m_effectDefs[i] = null;
            m_dsps[i] = null;
        }

        public void SetEffect(int i, time_t qnDur, EffectDef f, float mix = 1)
        {
            if (f == m_effectDefs[i])
                return;
            if (f == null)
            {
                RemoveEffect(i);
                return;
            }

            RemoveEffect(i);

            m_effectDefs[i] = f;
            m_effectMixes[i] = mix;

            m_dsps[i] = f.CreateEffectDsp(SampleRate);
            UpdateEffect(i, qnDur, 0);
        }

        public void UpdateEffect(int i, time_t qnDur, float alpha)
        {
            alpha = MathL.Clamp(alpha, 0, 1);
            m_effectParameters[i] = alpha;

            var f = m_effectDefs[i];
            if (f == null)
                return;

            f.ApplyToDsp(m_dsps[i], qnDur, alpha);
            m_dsps[i].Mix = m_effectMixes[i] * m_effectDefs[i].Mix.Sample(m_effectParameters[i]);
        }

        public void SetEffectMix(int i, float mix)
        {
            mix = MathL.Clamp(mix, 0, 1);
            m_effectMixes[i] = mix;

            var dsp = m_dsps[i];
            if (dsp == null)
                return;

            dsp.Mix = mix * m_effectDefs[i].Mix.Sample(m_effectParameters[i]);
        }

        public void SetEffectActive(int i, bool active)
        {
            m_effectsActive[i] = active;
        }

        public float GetEffectMix(int i) => m_effectMixes[i];

        public void Play() => Track.Play();
        public void Stop() => Track.Stop();

#if USE_MULTIPLE_EFFECT_BUFFERS
        private float[] m_copyBuffer = new float[2048];
        private float[] m_dummyBuffer = new float[2048];
#endif

        public override int Read(float[] buffer, int offset, int count)
        {
            int result = Track.Read(buffer, offset, count);

#if USE_MULTIPLE_EFFECT_BUFFERS
            // NOTE(local): make sure this doesn't give out garbage?
            if (count > m_copyBuffer.Length)
            {
                m_copyBuffer = new float[count];
                m_dummyBuffer = new float[count];
            }

            Array.Copy(buffer, offset, m_copyBuffer, 0, result);
            Array.Copy(buffer, offset, m_dummyBuffer, 0, result);
#endif

            for (int fxi = 0; fxi < m_dsps.Length; fxi++)
            {
                var effect = m_dsps[fxi];
                if (effect == null)
                    continue;

#if USE_MULTIPLE_EFFECT_BUFFERS
                var dataBuffer = m_effectsActive[fxi] ? m_copyBuffer : m_dummyBuffer;
                effect.Process(dataBuffer, 0, result);

                // Always process the effects to keep timing, but don't always mix them in.
                if (EffectsActive && m_effectsActive[fxi]) Array.Copy(dataBuffer, 0, buffer, offset, result);
#else
                if (EffectsActive && m_effectsActive[fxi]) effect.Process(buffer, offset, result);
#endif
            }

            for (int i = offset; i < offset + count; i++)
                buffer[i] = buffer[i] * Volume;

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
