namespace OpenRM.Voltex
{
    public enum StreamIndex : int
    {
        #region Playables

        BtA = 0,
        BtB = 1,
        BtC = 2,
        BtD = 3,

        FxL = 4,
        FxR = 5,

        VolL = 6,
        VolR = 7,

        #endregion

        #region Analog

        /// <summary>
        /// Spin, Swing, Wobble
        /// </summary>
        HighwayEffect,

        LaserApplicationKind,
        /// <summary>
        /// Gain, Filter Kind
        /// </summary>
        LaserParams,

        #endregion

        #region Visual Playback

        Stop,
        Reverse,
        Hide,

        #endregion

        #region Camera

        Zoom,
        Pitch,
        Offset,
        Roll,

        #endregion

        #region Stage

        SetBackground,
        SetImage,

        #endregion

        COUNT,
    }

    public enum Damping : byte
    {
        /// <summary>
        /// Immediately applies the target value.
        /// </summary>
        Off = 0,
        /// <summary>
        /// Slowly interpolates towards the target value,
        /// </summary>
        Slow,
        /// <summary>
        /// Quickly interpolates towards the target value.
        /// </summary>
        Fast,
    }

    public enum Decay : byte
    {
        /// <summary>
        /// The function will not decay over time.
        /// </summary>
        Off = 0,
        /// <summary>
        /// The function will decay to half its original
        ///  amplitude by the end of its duration.
        /// </summary>
        OnSlow = 1,
        /// <summary>
        /// The function will decay to 0 amplitude by
        ///  the end of its duration.
        /// </summary>
        On = 2,
    }

    [System.Flags]
    public enum LaserFunction : ushort
    {
        /// <summary>
        /// The input value is discarded entirely.
        /// </summary>
        Zero = 0,

        /// <summary>
        /// Keep the input value as-is.
        /// </summary>
        Source = 0x0001,

        /// <summary>
        /// Negate the input value.
        /// </summary>
        NegativeSource = 0x0002,
        /// <summary>
        /// Subtract the input value from 1.
        /// </summary>
        OneMinusSource = 0x0003,
        
        // TODO(local): figure out the actual values for laser roll amplitudes.

        /// <summary>
        /// Multiply the result by the "normal" laser amplitude.
        /// </summary>
        Normal = 0x1000,
        /// <summary>
        /// Multiply the result value by half of the "normal" laser amplitude.
        /// </summary>
        Smaller = 0x2000,
        /// <summary>
        /// Multiply the result value by 1.5 of the "normal" laser amplitude.
        /// </summary>
        Bigger = 0x3000,
        /// <summary>
        /// Multiply the result value by twice the "normal" laser amplitude.
        /// </summary>
        Biggest = 0x4000,

        FunctionMask = 0x0FFF,
        FlagMask = 0xF000,
    }

    [System.Flags]
    public enum LaserApplication : ushort
    {
        /// <summary>
        /// Ignore the laser inputs.
        /// </summary>
        Zero = 0,

        /// <summary>
        /// Add both processed laser inputs together.
        /// </summary>
        Additive = 0x0001,

        /// <summary>
        /// Take input values from the first non-zero laser input only.
        /// 
        /// For Example:
        /// If the left laser sends non-zero input first, then
        ///  only its values are accepted.
        /// Should the left laser then return to the zero position,
        ///  the right laser could take control instead.
        /// </summary>
        Initial = 0x0002,

        /// <summary>
        /// Selected only the left laser input.
        /// </summary>
        Left = 0x0003,
        
        /// <summary>
        /// Selected only the right laser input.
        /// </summary>
        Right = 0x0004,

        /// <summary>
        /// Keeps the maximum value (read: farthest from zero) of the
        ///  laser output only for the target direction.
        /// If the roll value is negative, then only lesser negative
        ///  values are applied; positive roll values continue with
        ///  greater positive values similarly.
        /// </summary>
        KeepMax = 0x1000,

        /// <summary>
        /// Keeps the minimum value (read: nearest to zero) of the
        ///  laser output only for the target direction.
        /// If the roll value is negative, then only greater negative
        ///  values are applied; positive roll values continue with
        ///  lesser positive values similarly.
        /// </summary>
        KeepMin = 0x2000,
        
        ApplicationMask = 0x0FFF,
        FlagMask = 0xF000,
    }
}
