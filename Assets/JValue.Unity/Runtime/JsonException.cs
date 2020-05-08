using System;

namespace Halak
{
    public class JsonException : Exception
    {
        public readonly string source;
        public readonly int index;
        public readonly int length;

        public JsonException(string message, JValue jValue) : this(message, jValue.source, jValue.startIndex, jValue.endIndex)
        {
        }

        public JsonException(string message, string source, int index, int length) : base(message)
        {
            this.source = source;
            this.index = index;
            this.length = length;
        }
    }
}
