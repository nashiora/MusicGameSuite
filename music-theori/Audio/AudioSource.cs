using System;

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
            }
        }

        private void OnRemoveFromMixerChannelEvent(AudioSource track) => OnRemoveFromMixerChannel();
        protected virtual void OnRemoveFromMixerChannel()
        {
        }
    }
}
