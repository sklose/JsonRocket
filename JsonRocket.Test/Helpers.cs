using FluentAssertions;
using System;
using System.Text;

namespace JsonRocket.Test
{
    internal static class Helpers
    {
        public static byte[] ToInput(this string json)
        {
            return Encoding.UTF8.GetBytes(json);
        }

        public static string ToValue(this ArraySegment<byte> buffer)
        {
            return Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
        }

        public static void AssertValue(this Tokenizer tokenizer, object expected)
        {
            tokenizer.GetValue().Read().Should().Be(expected);
        }

        public static void AssertSequence(this Tokenizer tokenizer, params Token[] tokens)
        {
            for (int i = 0; i < tokens.Length; i++)
            {
                tokenizer.Current.Should().Be(tokens[i]);
                tokenizer.MoveNext().Should().Be(i + 1 != tokens.Length);
            }
        }

        public static void MoveToNext(this Tokenizer tokenizer, Token token)
        {
            while (tokenizer.Current != token && tokenizer.Current != Token.Error && tokenizer.MoveNext())
            {
            }
        }
    }
}