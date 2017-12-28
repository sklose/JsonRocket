using System;

namespace JsonRocket
{
    public struct ExtractedValue
    {
        public ExtractedValue(string path, ArraySegment<byte> value)
        {
            Path = path;
            Value = value;
        }

        public string Path { get; }
        public ArraySegment<byte> Value { get; }
    }
}
