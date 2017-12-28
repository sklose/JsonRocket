using System;
using System.Collections.Generic;

namespace JsonRocket
{
    public class Extractor
    {
        private readonly Trie _trie;
        private readonly Stack<Trie.Node> _nesting;

        public Extractor(Trie trie)
        {
            _trie = trie;
            _nesting = new Stack<Trie.Node>(1024);
        }

        public void ReadFrom(Tokenizer tokenizer, List<KeyValuePair<string, ArraySegment<byte>>> result)
        {
            _nesting.Clear();
            while (tokenizer.MoveNext())
            {
                if (tokenizer.Current == Token.Key)
                {
                    var bounds = tokenizer.GetTokenBounds();
                    var r = _trie.Find(bounds);
                    if (r.Node != null && r.Node.IsMatch)
                    {
                        tokenizer.MoveNext();
                        result.Add(new KeyValuePair<string, ArraySegment<byte>>(
                            r.Node.Value, tokenizer.GetTokenBounds()));
                    }
                }
            }
        }
    }
}
