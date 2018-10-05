using System.IO;

namespace OpenRM.Voltex
{
    public enum MetadataKey : byte
    {
        Title                       = 0x01,
        Artist                      = 0x02,
        Charter                     = 0x03,
        
        AudioFilePaths              = 0x11,
        JacketPath                  = 0x12,
        JacketIllustrator           = 0x13,

        DifficultyName              = 0x21,
        DifficultyLevel             = 0x22,

        ChartOffset                 = 0x31,
        PreviewOffset               = 0x32,
        PreviewLength               = 0x33,
    }

    [System.Flags]
    public enum PlayableFlags : uint
    {
        None = 0,

        Instant                     = 0x00000001,
        ConstEffect                 = 0x00000002,
        LinearEffect                = 0x00000004,
        ChipEffect                  = 0x00000008,
        LaserIsLinear               = 0x00000010,
        LaserExtend                 = 0x00000020,
        UnusedFlag6                 = 0x00000040,
        FlagsAreTwoBytes            = 0x00000080,

        UnusedFlag7                 = 0x00000100,
        UnusedFlag8                 = 0x00000200,
        UnusedFlag9                 = 0x00000400,
        UnusedFlag10                = 0x00000800,
        UnusedFlag11                = 0x00001000,
        UnusedFlag12                = 0x00002000,
        UnusedFlag14                = 0x00004000,
        FlagsAreThreeBytes          = 0x00008000,

        UnusedFlag15                = 0x00010000,
        UnusedFlag16                = 0x00020000,
        UnusedFlag17                = 0x00040000,
        UnusedFlag18                = 0x00080000,
        UnusedFlag19                = 0x00100000,
        UnusedFlag20                = 0x00200000,
        UnusedFlag21                = 0x00400000,
        FlagsAreFourBytes           = 0x00800000,

        UnusedFlag22                = 0x01000000,
        UnusedFlag23                = 0x02000000,
        UnusedFlag24                = 0x04000000,
        UnusedFlag25                = 0x08000000,
        UnusedFlag26                = 0x10000000,
        UnusedFlag27                = 0x20000000,
        UnusedFlag28                = 0x40000000,
        UnusedFlag29                = 0x80000000,
    }

    public class VoltexChartSerializer
    {
        public const uint MAGIC = 0xEFFEC715;

        public VoltexChartSerializer()
        {
        }

        public void Serialize(Chart chart, Stream output)
        {
            // Header Information

            // Metadata

            // Effects

            // Events and Objects
        }
    }
}
