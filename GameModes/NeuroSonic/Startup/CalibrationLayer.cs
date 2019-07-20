using System;

using theori;
using theori.Audio;
using theori.IO;

namespace NeuroSonic.Startup
{
    public class CalibrationLayer : NscLayer
    {
        class ClickTrack : AudioSource
        {
            private const double CLICK_DURATION_SECONDS = 0.075;

            public override bool CanSeek => false;

            private readonly int m_sampleRate;
            public override int SampleRate => m_sampleRate;

            private readonly int m_channels;
            public override int Channels => m_channels;

            private time_t m_position;
            public override time_t Position { get => m_position; set => throw new NotImplementedException(); }

            public override time_t Length => throw new NotImplementedException();

            private readonly double m_frequency;
            private readonly double m_samplesPerBeat, m_clickDurationSamples;
            private long m_singleChannelSampleIndex = 0;

            public bool Silenced = false;

            public ClickTrack(int sampleRate, int channels, double frequency, double beatsPerMinute)
            {
                m_sampleRate = sampleRate;
                m_channels = channels;
                m_frequency = frequency;

                m_samplesPerBeat = 60 * sampleRate / beatsPerMinute;
                m_clickDurationSamples = sampleRate * CLICK_DURATION_SECONDS;
            }

            public override int Read(float[] buffer, int offset, int count)
            {
                int sampleRate = m_sampleRate, channels = m_channels;
                m_position = time_t.FromSeconds(m_singleChannelSampleIndex / (double)sampleRate);

                double vol = Volume;
                double attack = 0.0005 * sampleRate;

                int numSamples = count / channels;
                if (Silenced) goto end;

                // do this kinda naive first
                for (int i = 0; i < numSamples; i++)
                {
                    long sampleIndex = m_singleChannelSampleIndex + i;
                    double relativeSample = sampleIndex % m_samplesPerBeat;

                    double amp;
                    if (relativeSample < attack)
                        amp = vol * (relativeSample / attack);
                    else amp = vol * Math.Max(0, 1 - ((relativeSample - attack) / (m_clickDurationSamples - attack)));

                    double value = 0;
                    if (amp > 0)
                        value = MathL.Sin(sampleIndex * m_frequency * MathL.TwoPi / m_sampleRate);

                    buffer[offset + i * channels + 0] = (float)(value * amp);
                    buffer[offset + i * channels + 1] = (float)(value * amp);
                }

            end:
                m_singleChannelSampleIndex += numSamples;
                return count;
            }

            public override void Seek(time_t position) => throw new NotImplementedException();
        }

        private string Title => "Calibration";

        private ClickTrack m_click;

        public override void Destroy()
        {
            base.Destroy();

            m_click.Channel = null;
            m_click = null;
        }

        public override void Init()
        {
            base.Init();

            var master = Host.Mixer.MasterChannel;
            int sampleRate = master.SampleRate;
            int channels = master.Channels;
            double frequency = 432;
            double bpm = 90;

            m_click = new ClickTrack(sampleRate, channels, frequency, bpm);
            m_click.Channel = master;
        }

        public override bool KeyPressed(KeyInfo key)
        {
            switch (key.KeyCode)
            {
                case KeyCode.ESCAPE: Host.PopToParent(this); break;
            }

            return false;
        }
    }
}
