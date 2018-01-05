namespace JsonRocket
{
    public struct ExtractedValue
    {
        public ExtractedValue(string path, Value value)
        {
            Path = path;
            Value = value;
        }

        public string Path { get; }
        public Value Value { get; }
    }
}