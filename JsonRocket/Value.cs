using System;
using System.Text;

namespace JsonRocket
{
    public struct Value
    {
        private const byte NumberOffset = (byte)'0';
        private readonly bool _foundEscapeSequence;

        public ValueType Type { get; }
        public ArraySegment<byte> Buffer { get; }

        public Value(ArraySegment<byte> buffer, ValueType type, bool foundEscapeSequence = false)
        {
            _foundEscapeSequence = foundEscapeSequence;
            Type = type;
            Buffer = buffer;
        }

        public string ReadString()
        {
            return ReadString(Encoding.UTF8);
        }

        public string ReadString(Encoding encoding)
        {
            if (_foundEscapeSequence)
            {
                byte[] buffer = new byte[Buffer.Count];
                int index = 0;
                for (int i = 0; i < Buffer.Count; i++)
                {
                    var b = Buffer.Array[Buffer.Offset + i];
                    if (b != Literals.EscapeChar)
                    {
                        buffer[index++] = b;
                        continue;
                    }

                    i++;
                    if (i >= Buffer.Count)
                    {
                        throw new InvalidOperationException("detected malformed string");
                    }

                    var c = Buffer.Array[Buffer.Offset + i];
                    switch (c)
                    {
                        case (byte)'r':
                            buffer[index++] = (byte)'\r';
                            break;

                        case (byte)'f':
                            buffer[index++] = (byte)'\f';
                            break;

                        case (byte)'n':
                            buffer[index++] = (byte)'\n';
                            break;

                        case (byte)'t':
                            buffer[index++] = (byte)'\t';
                            break;

                        case (byte)'b':
                            buffer[index++] = (byte)'\b';
                            break;

                        case (byte)'"':
                            buffer[index++] = (byte)'"';
                            break;

                        case (byte)'\'':
                            buffer[index++] = (byte)'\'';
                            break;

                        case (byte)'/':
                            buffer[index++] = (byte)'/';
                            break;

                        case (byte)'u':
                            if (i + 4 >= Buffer.Count)
                            {
                                throw new InvalidOperationException("detected malformed string");
                            }

                            var b1 = Buffer.Array[Buffer.Offset + ++i];
                            var b2 = Buffer.Array[Buffer.Offset + ++i];
                            var b3 = Buffer.Array[Buffer.Offset + ++i];
                            var b4 = Buffer.Array[Buffer.Offset + ++i];

                            buffer[index] = (byte)(HexToByte(b1) << 4);
                            buffer[index++] += HexToByte(b2);
                            buffer[index] = (byte)(HexToByte(b3) << 4);
                            buffer[index++] += HexToByte(b4);
                            break;

                        default:
                            throw new InvalidOperationException($"detected unsupported escape sequence \\{b}");
                    }
                }

                return encoding.GetString(buffer, 0, index);
            }

            return encoding.GetString(Buffer.Array, Buffer.Offset, Buffer.Count);
        }

        private static byte HexToByte(byte hex)
        {
            switch (hex)
            {
                case (byte)'0':
                case (byte)'1':
                case (byte)'2':
                case (byte)'3':
                case (byte)'4':
                case (byte)'5':
                case (byte)'6':
                case (byte)'7':
                case (byte)'8':
                case (byte)'9':
                    return (byte)(hex - NumberOffset);

                case (byte)'a':
                case (byte)'A':
                    return 10;

                case (byte)'b':
                case (byte)'B':
                    return 11;

                case (byte)'c':
                case (byte)'C':
                    return 12;

                case (byte)'d':
                case (byte)'D':
                    return 13;

                case (byte)'e':
                case (byte)'E':
                    return 14;

                case (byte)'f':
                case (byte)'F':
                    return 15;
            }

            throw new ArgumentException($"Invalid hex code '{(char)hex}'");
        }

        public int ReadInt32()
        {
            if (Type != ValueType.Integer)
            {
                throw new InvalidOperationException($"Cannot read value of type {Type} as Int32");
            }

            checked
            {
                return (int)ReadInt64();
            }
        }

        public long ReadInt64()
        {
            if (Type != ValueType.Integer)
            {
                throw new InvalidOperationException($"Cannot read value of type {Type} as Int64");
            }

            long value = 0, multiplier = 1;
            checked
            {
                for (int i = Buffer.Count - 1; i >= 0; i--)
                {
                    var b = Buffer.Array[Buffer.Offset + i];
                    if (b == '-')
                    {
                        value *= -1;
                    }
                    else
                    {
                        int digit = b - NumberOffset;
                        value += digit * multiplier;
                        multiplier *= 10;
                    }
                }
            }

            return value;
        }

        public bool ReadBoolean()
        {
            if (Type == ValueType.False) return false;
            if (Type == ValueType.True) return true;
            throw new InvalidOperationException($"Cannot read value of type {Type} as Boolean");
        }

        public object Read()
        {
            switch (Type)
            {
                case ValueType.String:
                    return ReadString();

                case ValueType.Integer:
                    return ReadInt32();

                case ValueType.Float:
                    break;

                case ValueType.True:
                case ValueType.False:
                    return ReadBoolean();
            }

            return null;
        }
    }
}