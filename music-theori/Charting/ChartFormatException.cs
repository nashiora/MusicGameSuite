using System;
using System.Runtime.Serialization;

namespace theori.Charting
{
    [Serializable]
    public class ChartFormatException : Exception
    {
        public ChartFormatException() : base() { }

        public ChartFormatException(string message)
            : base(message) { }

        public ChartFormatException(string message, Exception inner)
            : base(message, inner) { }

        protected ChartFormatException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
