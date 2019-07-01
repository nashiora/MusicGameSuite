using System;
using System.Numerics;
using theori.Database;

namespace theori.Charting
{
    // TODO(local): Figure out how to do difficulties

    public class ChartInfo : IEquatable<ChartInfo>, IHasPrimaryKey
    {
        public static bool operator ==(ChartInfo a, ChartInfo b) => a == null ? b == null : a.Equals(b);
        public static bool operator !=(ChartInfo a, ChartInfo b) => !(a == b);

        /// <summary>
        /// The database primary key.
        /// </summary>
        public int ID { get; set; }

        public int SetID => Set.ID;
        public ChartSetInfo Set { get; set; }

        /// <summary>
        /// The name of the chart file inside of the Set directory.
        /// </summary>
        public string FileName { get; set; }

        private string m_charterBacking = "Unknown";
        public string Charter
        {
            get => m_charterBacking;
            set => m_charterBacking = value ?? "Unknown";
        }

        public string JacketFileName { get; set; } = null;
        public string JacketArtist { get; set; } = null;

        public string BackgroundFileName { get; set; } = null;
        public string BackgroundArtist { get; set; } = null;

        public double DifficultyLevel { get; set; } = 1.0;
        public int? DifficultyIndex { get; set; } = null;

        public string DifficultyName { get; set; } = null;
        public string DifficultyNameShort { get; set; } = null;

        public Vector4? DifficultyColor { get; set; } = null;

        public time_t ChartDuration { get; set; } = 0;
        
        public override bool Equals(object obj) => obj is ChartInfo info && Equals(info);
        bool IEquatable<ChartInfo>.Equals(ChartInfo other)
        {
            if (other is null) return false;
            // TODO(local): This isn't TECHNICALLY true
            //  but should it be considered true?
            return ID == other.ID;
        }

        public override int GetHashCode() => HashCode.For(ID);
    }
}
