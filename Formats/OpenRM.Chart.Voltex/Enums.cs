namespace OpenRM.Voltex
{
    internal enum EnumStreamIndex : int
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
        
        LaserParams,
        LaserApplicationKind,
        LaserFilterGain,
        LaserFilterKind,
        SlamVolume,

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

    public static class StreamIndex
    {
        #region Playables

        public const int BtA = (int)EnumStreamIndex.BtA;
        public const int BtB = (int)EnumStreamIndex.BtB;
        public const int BtC = (int)EnumStreamIndex.BtC;
        public const int BtD = (int)EnumStreamIndex.BtD;

        public const int FxL = (int)EnumStreamIndex.FxL;
        public const int FxR = (int)EnumStreamIndex.FxR;

        public const int VolL = (int)EnumStreamIndex.VolL;
        public const int VolR = (int)EnumStreamIndex.VolR;

        #endregion

        #region Analog

        /// <summary>
        /// Spin; Swing; Wobble
        /// </summary>
        public const int HighwayEffect = (int)EnumStreamIndex.HighwayEffect;
        
        public const int LaserParams = (int)EnumStreamIndex.LaserParams;
        public const int LaserApplicationKind = (int)EnumStreamIndex.LaserApplicationKind;
        public const int LaserFilterGain = (int)EnumStreamIndex.LaserFilterKind;
        public const int LaserFilterKind = (int)EnumStreamIndex.LaserFilterGain;
        public const int SlamVolume = (int)EnumStreamIndex.SlamVolume;

        #endregion

        #region Visual Playback

        public const int Stop = (int)EnumStreamIndex.Stop;
        public const int Reverse = (int)EnumStreamIndex.Reverse;
        public const int Hide = (int)EnumStreamIndex.Hide;

        #endregion

        #region Camera

        public const int Zoom = (int)EnumStreamIndex.Zoom;
        public const int Pitch = (int)EnumStreamIndex.Pitch;
        public const int Offset = (int)EnumStreamIndex.Offset;
        public const int Roll = (int)EnumStreamIndex.Roll;

        #endregion

        #region Stage

        public const int SetBackground = (int)EnumStreamIndex.SetBackground;
        public const int SetImage = (int)EnumStreamIndex.SetImage;

        #endregion

        public const int COUNT = (int)EnumStreamIndex.COUNT;
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

    public enum LaserFunction : byte
    {
        /// <summary>
        /// Keep the input value as-is.
        /// </summary>
        Source,
        /// <summary>
        /// The input value is discarded entirely.
        /// </summary>
        Zero,
        /// <summary>
        /// Negate the input value.
        /// </summary>
        NegativeSource,
        /// <summary>
        /// Subtract the input value from 1.
        /// </summary>
        OneMinusSource,
    }

    public enum LaserScale : byte
    {
        /// <summary>
        /// Multiply the result by the "normal" laser amplitude.
        /// </summary>
        Normal,
        /// <summary>
        /// Multiply the result value by half of the "normal" laser amplitude.
        /// </summary>
        Smaller,
        /// <summary>
        /// Multiply the result value by 1.5 of the "normal" laser amplitude.
        /// </summary>
        Bigger,
        /// <summary>
        /// Multiply the result value by twice the "normal" laser amplitude.
        /// </summary>
        Biggest,
    }
    
    [System.Flags]
    public enum LaserIndex
    {
        Neither = 0x00,

        Left = 0x01,
        Right = 0x02,

        Both = Left | Right,
    }
}
