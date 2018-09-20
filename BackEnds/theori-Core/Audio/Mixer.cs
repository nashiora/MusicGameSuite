using System;
using System.Collections.Generic;

using CSCore;
using CSCore.DSP;
using CSCore.SoundOut;

namespace theori.Audio
{
    internal class MixerChannelToCSCore : ISampleSource
    {
        private MixerChannel Channel { get; }

        public bool CanSeek => false;

        public WaveFormat WaveFormat => Channel.WaveFormat;

        public long Position { get => 0; set => throw new NotImplementedException(); }
        public long Length => 0;

        public MixerChannelToCSCore(MixerChannel channel)
        {
            Channel = channel;
        }

        public void Dispose() => Channel.Dispose();
        public int Read(float[] buffer, int offset, int count) => Channel.Read(buffer, offset, count);
    }

    public sealed class Mixer
    {
        private readonly object lockObj = new object();
        private readonly WasapiOut output;
        private readonly List<MixerChannel> channels = new List<MixerChannel>();

        public int OutputLatencyMillis => output.Latency;

        public MixerChannel MasterChannel { get; }

        public Mixer(int channelCount)
        {
            output = new WasapiOut() { Latency = 1 };
            MasterChannel = new MixerChannel("Master", channelCount, output.Device.DeviceFormat.SampleRate);

            var w = new MixerChannelToCSCore(MasterChannel).ToWaveSource();
            output.Initialize(w);
            output.Play();
        }

        public void AddChannel(MixerChannel channel)
        {
            lock (lockObj)
            {
                if (!Contains(channel))
                {
                    channels.Add(channel);
                    MasterChannel.AddSource(channel);
                }
            }
        }
        
        public void RemoveChannel(MixerChannel channel)
        {
            lock (lockObj)
            {
                if (Contains(channel))
                {
                    channels.Remove(channel);
                    MasterChannel.RemoveSource(channel);
                }
            }
        }

        public bool Contains(MixerChannel channel)
        {
            if (channel == null) return false;
            return channels.Contains(channel);
        }

        public void Dispose()
        {
            foreach (var c in channels)
                c.Dispose();
            MasterChannel.Dispose();
            output.Dispose();
        }
    }
}
