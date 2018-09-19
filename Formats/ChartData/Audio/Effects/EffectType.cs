namespace OpenRM.Audio.Effects
{
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
        Echo,
        Panning,
        PitchShift,
        LowPassFilter,
        HighPassFilter,
        PeakingFilter,
        
        UserDefined0 = 0x40,
        UserDefined1,
        UserDefined2,
        UserDefined3,
        UserDefined4,
        UserDefined5,
        UserDefined6,
        UserDefined7,
        UserDefined8,
        UserDefined9,
    }
}
