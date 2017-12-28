namespace JsonRocket
{
    internal static class Literals
    {
        internal const byte ObjectStart = (byte) '{';
        internal const byte ObjectEnd = (byte) '}';
        internal const byte ArrayStart = (byte) '[';
        internal const byte ArrayEnd = (byte) ']';
        internal const byte Whitespace1 = (byte) ' ';
        internal const byte Whitespace2 = (byte) '\r';
        internal const byte Whitespace3 = (byte) '\n';
        internal const byte Whitespace4 = (byte) '\t';
        internal const byte SingleQuote = (byte) '\'';
        internal const byte DoubleQuote = (byte) '"';
        internal const byte Colon = (byte) ':';
        internal const byte Comma = (byte) ',';
        internal const byte True0 = (byte) 't';
        internal const byte False0 = (byte) 'f';
        internal const byte Null0 = (byte) 'n';
        internal const byte EscapeChar = (byte) '\\';

        internal static readonly byte[] True1P = {(byte) 'r', (byte) 'u', (byte) 'e'};
        internal static readonly byte[] False1P = {(byte) 'a', (byte) 'l', (byte) 's', (byte) 'e'};
        internal static readonly byte[] Null1P = {(byte) 'u', (byte) 'l', (byte) 'l'};
    }
}
