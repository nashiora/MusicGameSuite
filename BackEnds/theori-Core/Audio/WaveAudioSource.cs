using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenRM;

namespace theori.Audio
{
    internal sealed class WaveAudioSource : AudioSource
    {
        public override bool CanSeek => throw new NotImplementedException();

        public override int SampleRate => throw new NotImplementedException();

        public override int Channels => throw new NotImplementedException();

        public override time_t Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override time_t LengthMicros => throw new NotImplementedException();

        public override int Read(float[] buffer, int offset, int count) => throw new NotImplementedException();
        public override void Seek(time_t position) => throw new NotImplementedException();
    }
}
