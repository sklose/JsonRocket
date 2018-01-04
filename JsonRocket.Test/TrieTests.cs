using FluentAssertions;
using System.Text;
using Xunit;

namespace JsonRocket.Test
{
    public class TrieTests
    {
        [Fact]
        public void FindsMatchesFromRoot()
        {
            var trie = new Trie();
            trie.Add("James");
            trie.Add("Mark");

            trie.Find(Buffer("Ja")).IsMatch.Should().BeFalse();
            trie.Find(Buffer("Mark")).IsMatch.Should().BeTrue();
        }

        [Fact]
        public void FindsMatchesFromIntermediateState()
        {
            var trie = new Trie();
            trie.Add("James");
            trie.Add("Mark");

            var result = trie.Find(Buffer("Ja"));
            trie.Find(Buffer("mes"), result).IsMatch.Should().BeTrue();
        }

        private byte[] Buffer(string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }
    }
}