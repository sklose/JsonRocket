using System;

namespace JsonRocket
{
    internal static class Literals
    {
        static Literals()
        {
            DotBuffer = new[] { Dot };
            ArrayStartBuffer = new[] { ArrayStart };
            ArrayEndBuffer = new[] { ArrayEnd };

            NumberElements = new NumberElement[byte.MaxValue];
            for (int i = 0; i < NumberElements.Length; i++)
                NumberElements[i].IsError = true;
            for (int i = Number0; i <= Number9; i++)
                NumberElements[i].IsError = false;

            NumberElements[Minus].IsError = false;
            NumberElements[Plus].IsError = false;
            NumberElements[Plus].IsFloat = true;
            NumberElements[Dot].IsError = false;
            NumberElements[Dot].IsFloat = true;
            NumberElements[LowerE].IsError = false;
            NumberElements[LowerE].IsFloat = true;
            NumberElements[UpperE].IsError = false;
            NumberElements[UpperE].IsFloat = true;

            NumberElements[ArrayEnd].IsEnd = true;
            NumberElements[ArrayEnd].IsError = false;
            NumberElements[ObjectEnd].IsEnd = true;
            NumberElements[ObjectEnd].IsError = false;
            NumberElements[Comma].IsEnd = true;
            NumberElements[Comma].IsError = false;
            NumberElements[Whitespace1].IsEnd = true;
            NumberElements[Whitespace1].IsError = false;
            NumberElements[Whitespace2].IsEnd = true;
            NumberElements[Whitespace2].IsError = false;
            NumberElements[Whitespace3].IsEnd = true;
            NumberElements[Whitespace3].IsError = false;
            NumberElements[Whitespace4].IsEnd = true;
            NumberElements[Whitespace4].IsError = false;
        }

        internal const byte ObjectStart = (byte)'{';
        internal const byte ObjectEnd = (byte)'}';
        internal const byte ArrayStart = (byte)'[';
        internal const byte ArrayEnd = (byte)']';
        internal const byte Whitespace1 = (byte)' ';
        internal const byte Whitespace2 = (byte)'\r';
        internal const byte Whitespace3 = (byte)'\n';
        internal const byte Whitespace4 = (byte)'\t';
        internal const byte SingleQuote = (byte)'\'';
        internal const byte DoubleQuote = (byte)'"';
        internal const byte Colon = (byte)':';
        internal const byte Comma = (byte)',';
        internal const byte True0 = (byte)'t';
        internal const int TrueSkipLength = 3;
        internal const byte False0 = (byte)'f';
        internal const int FalseSkipLength = 4;
        internal const byte Null0 = (byte)'n';
        internal const int NullSkipLength = 3;
        internal const byte EscapeChar = (byte)'\\';
        internal const byte Dot = (byte)'.';
        internal const byte Minus = (byte)'-';
        internal const byte Plus = (byte)'+';
        internal const byte LowerE = (byte)'e';
        internal const byte UpperE = (byte)'E';
        internal const byte Number0 = (byte)'0';
        internal const byte Number9 = (byte)'9';

        internal static readonly NumberElement[] NumberElements;
        internal static readonly byte[] DotBuffer;
        internal static readonly byte[] ArrayStartBuffer;
        internal static readonly byte[] ArrayEndBuffer;
    }
}