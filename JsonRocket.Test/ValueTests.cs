using System;
using FluentAssertions;
using Xunit;

namespace JsonRocket.Test
{
    public class ValueTests
    {
        [Theory]
        [InlineData("42", 42)]
        [InlineData("-100", -100)]
        [InlineData("0", 0)]
        [InlineData("173823920", 173823920)]
        [InlineData("00001", 1)]
        public void Reads32BitIntegers(string input, int expected)
        {
            var buffer = new ArraySegment<byte>(input.ToInput());
            var value = new Value(buffer, ValueType.Integer);
            value.ReadInt32().Should().Be(expected);
        }

        [Fact]
        public void ReadsTrueAsBoolean()
        {
            var buffer = new ArraySegment<byte>("true".ToInput());
            var value = new Value(buffer, ValueType.True);
            value.ReadBoolean().Should().BeTrue();
        }

        [Fact]
        public void ReadsFalseAsBoolean()
        {
            var buffer = new ArraySegment<byte>("false".ToInput());
            var value = new Value(buffer, ValueType.False);
            value.ReadBoolean().Should().BeFalse();
        }

        [Theory]
        [InlineData("Test", "Test")]
        [InlineData("Test\\\"", "Test\"")]
        [InlineData("Test\\n", "Test\n")]
        [InlineData("\\nTest\\n\\r", "\nTest\n\r")]
        [InlineData("15\\uc2b0C", "15°C")]
        public void ReadsString(string input, string expected)
        {
            var buffer = new ArraySegment<byte>(input.ToInput());
            var value = new Value(buffer, ValueType.String, input.Contains("\\"));
            value.ReadString().Should().Be(expected);
        }

        [Theory]
        [InlineData("23.1", 23.1f)]
        [InlineData("-23.1", -23.1f)]
        [InlineData(".1", .1f)]
        [InlineData("193", 193f)]
        public void ReadsSingle(string input, float expected)
        {
            var buffer = new ArraySegment<byte>(input.ToInput());
            var value = new Value(buffer, ValueType.Float);
            value.ReadSingle().Should().Be(expected);
        }

        [Theory]
        [InlineData("23.1", 23.1d)]
        [InlineData("-23.1", -23.1d)]
        [InlineData(".1", .1d)]
        [InlineData("193", 193d)]
        [InlineData("3.141592653589793238", 3.141592653589793238d)]
        public void ReadsDouble(string input, double expected)
        {
            var buffer = new ArraySegment<byte>(input.ToInput());
            var value = new Value(buffer, ValueType.Float);
            value.ReadDouble().Should().Be(expected);
        }
    }
}