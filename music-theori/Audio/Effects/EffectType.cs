namespace theori.Audio.Effects
{
    // TODO(local): Remove this, we don't use it for anything substantial and we can pattern match when we need to
    public enum EffectType
    {
        None = 0,

        Retrigger,
        Flanger,
        Phaser,
        Gate,
        TapeStop,
        BitCrush,
        Wobble,
        SideChain,
        LowPassFilter,
        HighPassFilter,
        PeakingFilter,

        Echo,
        Panning,
        PitchShift,
    }
}
