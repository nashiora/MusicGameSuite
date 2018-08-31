using System;
using System.Collections.Generic;

using CSCore;
using OpenRM;

namespace theori.Audio
{
    public class MixerChannel : AudioSource
    {
        private readonly object lockObj = new object();
        private readonly List<AudioSource> sources = new List<AudioSource>();

        private float[] mixerBuffer;

        public string Name { get; }

        public override int Channels { get; }
        public override int SampleRate { get; }

        public bool DivideResult { get; set; } = false;

        public WaveFormat WaveFormat { get; }

        public override bool CanSeek => false;

        public override time_t LengthMicros => 0;
        public override time_t PositionMicros { get => 0; set => Seek(value); }

        public event Action<AudioSource> OnSampleSourceEnded;

        public MixerChannel(string name, int channelCount, int sampleRate)
        {
            Name = name;
            Channels = channelCount;
            SampleRate = sampleRate;

            WaveFormat = new WaveFormat(sampleRate, 32, channelCount, AudioEncoding.IeeeFloat);
        }

        internal void AddSource(AudioSource source)
        {
            lock (lockObj)
            {
                if (!Contains(source)) sources.Add(source);
            }
        }
        
        internal void RemoveSource(AudioSource source)
        {
            lock (lockObj)
            {
                if (Contains(source)) sources.Remove(source);
            }
        }

        internal bool Contains(AudioSource source)
        {
            if (source == null) return false;
            return sources.Contains(source);
        }

        public override int Read(float[] buffer, int offset, int count)
        {
            int numStoredSamples = 0;
            
            if (count > 0 && sources.Count > 0)
            {
                lock (lockObj)
                {
                    mixerBuffer = mixerBuffer.CheckBuffer(count);
                    var numReadSamples = new List<int>();

                    for (int m = sources.Count - 1; m >=0; m--)
                    {
                        var source = sources[m];

                        int read = source.Read(mixerBuffer, 0, count);
                        for (int i = offset, n = 0; n < read; i++, n++)
                        {
                            if (numStoredSamples <= i)
                                buffer[i] = mixerBuffer[n];
                            else buffer[i] += mixerBuffer[n];
                        }

                        if (read > numStoredSamples)
                            numStoredSamples = read;

                        if (read > 0)
                            numReadSamples.Add(read);
                        else
                        {
                            source.OnFinish();
                            OnSampleSourceEnded?.Invoke(source);
                            if (source.RemoveFromChannelOnFinish)
                                RemoveSource(source);
                        }
                    }

                    if (DivideResult)
                    {
                        numReadSamples.Sort();

                        int currentOffset = offset;
                        int remainingSources = numReadSamples.Count;

                        foreach (int readSamples in numReadSamples)
                        {
                            if (remainingSources == 0)
                                break;

                            while (currentOffset < offset + readSamples)
                            {
                                buffer[currentOffset] /= remainingSources;
                                buffer[currentOffset] = Math.Max(-1, Math.Min(1, buffer[currentOffset]));
                                currentOffset++;
                            }

                            remainingSources--;
                        }
                    }
                }
            }

            float vol = Volume;
            for (int i = 0; i < numStoredSamples; i++)
                buffer[i] *= vol;

            if (numStoredSamples != count)
            {
                Array.Clear(buffer, Math.Max(offset + numStoredSamples - 1, 0), count - numStoredSamples);
                return count;
            }

            return numStoredSamples;
        }

        public override void Seek(time_t positionMicros) => throw new NotImplementedException("cannot seek");

        protected override void DisposeManaged()
        {
            lock (lockObj)
            {
                foreach (var sampleSource in sources.ToArray())
                {
                    sampleSource.Dispose();
                    sources.Remove(sampleSource);
                }
            }
        }
    }
}
