using System;
using OpenRM;

namespace theori.Audio
{
    public abstract class AudioSource : Disposable
    {
        public event Action Finish;

        public abstract bool CanSeek { get; }

        public abstract int SampleRate { get; }
        public abstract int Channels { get; }

        public abstract time_t Position { get; set; }
        public abstract time_t Length { get; }

        protected float volume = 1.0f;
        public virtual float Volume
        {
            get => volume;
            set => volume = MathL.Clamp(value, 0, 1);
        }

        internal void OnFinish() => Finish?.Invoke();

        public virtual bool RemoveFromChannelOnFinish { get; set; } = true;

        public abstract int Read(float[] buffer, int offset, int count);
        public abstract void Seek(time_t position);
    }
}
