using System;
using System.Collections.Generic;
using FluentAssertions;
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
            var actual = new List<KeyValuePair<string, ArraySegment<byte>>>();
            extractor.ReadFrom(tokenizer, actual);

            actual.Should().HaveCount(2);
            actual[0].Key.Should().Be("Key");
            actual[0].Value.ToValue().Should().Be("123");
            actual[1].Key.Should().Be("Value");
            actual[1].Value.ToValue().Should().Be("5");
        }
    }
}
