using Xunit;

namespace JsonRocket.Test
{
    public class TokenizerTests
    {
        [Theory]
        [InlineData("{}")]
        [InlineData("{ }")]
        [InlineData(" { } ")]
        public void ReadsEmtpyObject(string input)
        {
            var tokenizer = CreateTokenizer(input);
            tokenizer.AssertSequence(
                Token.Undefined,
                Token.ObjectStart,
                Token.ObjectEnd);
        }

        [Theory]
        [InlineData("{'key':'value'}")]
        [InlineData("{ 'key' : 'value' }")]
        public void ReadsObjectWithSingleKey(string input)
        {
            var tokenizer = CreateTokenizer(input);
            tokenizer.AssertSequence(
                Token.Undefined,
                Token.ObjectStart,
                Token.Key,
                Token.String,
                Token.ObjectEnd);
        }

        [Theory]
        [InlineData("{'key1':'value1', 'key2':'value2'}")]
        public void ReadsObjectWithTwoKeys(string input)
        {
            var tokenizer = CreateTokenizer(input);
            tokenizer.AssertSequence(
                Token.Undefined,
                Token.ObjectStart,
                Token.Key,
                Token.String,
                Token.Key,
                Token.String,
                Token.ObjectEnd);
        }

        [Theory]
        [InlineData("{'key1':'value1', 'key2': { 'nested':'value2' }}")]
        public void ReadsObjectWithNestedObject(string input)
        {
            var tokenizer = CreateTokenizer(input);
            tokenizer.AssertSequence(
                Token.Undefined,
                Token.ObjectStart,
                Token.Key,
                Token.String,
                Token.Key,
                Token.ObjectStart,
                Token.Key,
                Token.String,
                Token.ObjectEnd,
                Token.ObjectEnd);
        }

        [Theory]
        [InlineData("{'a': true, 'b': false, 'c': null}")]
        public void ReadsLiterals(string input)
        {
            var tokenizer = CreateTokenizer(input);
            tokenizer.AssertSequence(
                Token.Undefined,
                Token.ObjectStart,
                Token.Key,
                Token.True,
                Token.Key,
                Token.False,
                Token.Key,
                Token.Null,
                Token.ObjectEnd);
        }

        [Theory]
        [InlineData("{'a': 1, 'b': 123, 'c': 0.1, 'd': -1233, 'e': 3e12}")]
        public void ReadsNumbers(string input)
        {
            var tokenizer = CreateTokenizer(input);
            tokenizer.AssertSequence(
                Token.Undefined,
                Token.ObjectStart,
                Token.Key,
                Token.Integer,
                Token.Key,
                Token.Integer,
                Token.Key,
                Token.Float,
                Token.Key,
                Token.Integer,
                Token.Key,
                Token.Float,
                Token.ObjectEnd);
        }

        [Theory]
        [InlineData("[1, 2, 3, 4]")]
        public void ReadsArray(string input)
        {
            var tokenizer = CreateTokenizer(input);
            tokenizer.AssertSequence(
                Token.Undefined,
                Token.ArrayStart,
                Token.Integer,
                Token.Integer,
                Token.Integer,
                Token.Integer,
                Token.ArrayEnd);
        }

        [Theory]
        [InlineData("[1, 2, 3, ['a', 'b']]")]
        public void ReadsNestedArrays(string input)
        {
            var tokenizer = CreateTokenizer(input);
            tokenizer.AssertSequence(
                Token.Undefined,
                Token.ArrayStart,
                Token.Integer,
                Token.Integer,
                Token.Integer,
                Token.ArrayStart,
                Token.String,
                Token.String,
                Token.ArrayEnd,
                Token.ArrayEnd);
        }

        [Theory]
        [InlineData("[{},{}]")]
        public void ReadsArrayOfObjects(string input)
        {
            var tokenizer = CreateTokenizer(input);
            tokenizer.AssertSequence(
                Token.Undefined,
                Token.ArrayStart,
                Token.ObjectStart,
                Token.ObjectEnd,
                Token.ObjectStart,
                Token.ObjectEnd,
                Token.ArrayEnd);
        }

        [Theory]
        [InlineData("{'a':[1,2,3]}")]
        public void ReadsArrayAsValue(string input)
        {
            var tokenizer = CreateTokenizer(input);
            tokenizer.AssertSequence(
                Token.Undefined,
                Token.ObjectStart,
                Token.Key,
                Token.ArrayStart,
                Token.Integer,
                Token.Integer,
                Token.Integer,
                Token.ArrayEnd,
                Token.ObjectEnd);
        }

        [Theory]
        [InlineData(@"{'a':'\''}")]
        [InlineData(@"{'a':'\t'}")]
        [InlineData(@"{'a':'it\'s'}")]
        public void HonorsEscapeSequences(string input)
        {
            var tokenizer = CreateTokenizer(input);
            tokenizer.AssertSequence(
                Token.Undefined,
                Token.ObjectStart,
                Token.Key,
                Token.String,
                Token.ObjectEnd);
        }

        [Fact]
        public void ProvidesBoundsForStrings()
        {
            var tokenizer = CreateTokenizer("{'a':'b'}");

            tokenizer.MoveToNext(Token.Key);
            tokenizer.AssertValue("a");

            tokenizer.MoveToNext(Token.String);
            tokenizer.AssertValue("b");
        }

        [Fact]
        public void ProvidesBoundsForNumbers()
        {
            var tokenizer = CreateTokenizer("{'a': 1234 }");

            tokenizer.MoveToNext(Token.Key);
            tokenizer.AssertValue("a");

            tokenizer.MoveToNext(Token.Integer);
            tokenizer.AssertValue("1234");
        }

        private Tokenizer CreateTokenizer(string json)
        {
            var tokenizer = new Tokenizer();
            tokenizer.Reset(json.ToInput());
            return tokenizer;
        }
    }
}
