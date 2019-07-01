using System;

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

        public override bool Equals(object obj) => obj is ChartInfo info && Equals(info);
        bool IEquatable<ChartInfo>.Equals(ChartInfo other)
        {
            if (other is null) return false;
            return true;
        }

        public override int GetHashCode() => HashCode.For(ID);
    }
}
