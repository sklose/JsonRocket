using System.Text;
using FluentAssertions;
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

            trie.Find(Buffer("Ja")).StepsTaken.Should().Be(2);
            trie.Find(Buffer("Ja")).Node.IsMatch.Should().BeFalse();

            trie.Find(Buffer("Mark")).StepsTaken.Should().Be(4);
            trie.Find(Buffer("Mark")).Node.IsMatch.Should().BeTrue();
        }

        [Fact]
        public void FindsMatchesFromIntermediateState()
        {
            var trie = new Trie();
            trie.Add("James");
            trie.Add("Mark");

            var result = trie.Find(Buffer("Ja"));
            trie.Find(Buffer("mes"), result.Node).Node.IsMatch.Should().BeTrue();
        }
        
        private byte[] Buffer(string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }
    }
}
