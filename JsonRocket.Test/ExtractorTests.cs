using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace JsonRocket.Test
{
    public class ExtractorTests
    {
        [Fact]
        public void RetrievesTopLevelValues()
        {
            var trie = new Trie();
            trie.Add("Key");
            trie.Add("Value");

            var tokenizer = new Tokenizer();
            tokenizer.Reset("{'Key': 123, 'Value': 5}".ToInput());

            var extractor = new Extractor(trie);
            var actual = new List<ExtractedValue>();
            extractor.ReadFrom(tokenizer, actual);

            actual.Should().HaveCount(2);
            actual[0].Path.Should().Be("Key");
            actual[0].Value.ReadInt32().Should().Be(123);
            actual[1].Path.Should().Be("Value");
            actual[1].Value.ReadInt32().Should().Be(5);
        }

        [Fact]
        public void RetrievesSecondLevelValues()
        {
            var trie = new Trie();
            trie.Add("Value.Key");

            var tokenizer = new Tokenizer();
            tokenizer.Reset("{'Key': 123, 'Value': { 'Key': 987 }}".ToInput());

            var extractor = new Extractor(trie);
            var actual = new List<ExtractedValue>();
            extractor.ReadFrom(tokenizer, actual);

            actual.Should().HaveCount(1);
            actual[0].Path.Should().Be("Value.Key");
            actual[0].Value.ReadInt32().Should().Be(987);
        }

        [Fact]
        public void SkipsIntermediateArrays()
        {
            var trie = new Trie();
            trie.Add("Key");
            trie.Add("Value.Key");

            var tokenizer = new Tokenizer();
            tokenizer.Reset("{'Key': true, 'Array': [ 1, 2 ], 'Value': { 'Key': 987 }}".ToInput());

            var extractor = new Extractor(trie);
            var actual = new List<ExtractedValue>();
            extractor.ReadFrom(tokenizer, actual);

            actual.Should().HaveCount(2);
            actual[0].Path.Should().Be("Key");
            actual[0].Value.ReadBoolean().Should().BeTrue();
            actual[1].Path.Should().Be("Value.Key");
            actual[1].Value.ReadInt32().Should().Be(987);
        }
    }
}