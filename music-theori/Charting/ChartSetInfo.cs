using System;

using theori.Database;

namespace theori.Charting
{
    public class ChartSetInfo : IEquatable<ChartSetInfo>, IHasPrimaryKey
    {
        public static bool operator ==(ChartSetInfo a, ChartSetInfo b) => a == null ? b == null : a.Equals(b);
        public static bool operator !=(ChartSetInfo a, ChartSetInfo b) => !(a == b);

        /// <summary>
        /// The database primary key.
        /// </summary>
        public int ID { get; set; }

        private int? m_onlineIDBacking;
        /// <summary>
        /// The ID for the chart set
        /// </summary>
        public int? OnlineID
        {
            get => m_onlineIDBacking;
            set => m_onlineIDBacking = value > 0 ? value : null;
        }

        /// <summary>
        /// Relative to the selected chart storage directory.
        /// </summary>
        public string FilePath { get; set; }

        public override bool Equals(object obj) => obj is ChartSetInfo info && Equals(info);
        bool IEquatable<ChartSetInfo>.Equals(ChartSetInfo other)
        {
            if (other is null) return false;
            return true;
        }

        public override int GetHashCode() => HashCode.For(ID);
    }
}
