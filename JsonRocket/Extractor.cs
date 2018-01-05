using System.Collections.Generic;

namespace JsonRocket
{
    public class Extractor
    {
        private readonly Trie _trie;

        public Extractor(Trie trie)
        {
            _trie = trie;
        }

        public void ReadFrom(Tokenizer tokenizer, List<ExtractedValue> result)
        {
            while (tokenizer.MoveNext())
            {
                if (tokenizer.Current == Token.ObjectStart)
                {
                    ReadObject(tokenizer, result, null);
                }
            }
        }

        private void ReadObject(Tokenizer tokenizer, List<ExtractedValue> result, Trie.Node prefix)
        {
            while (tokenizer.MoveNext())
            {
                switch (tokenizer.Current)
                {
                    case Token.ObjectEnd:
                        return;

                    case Token.Key:
                        var currentKey = tokenizer.GetValue();
                        var n = _trie.Find(currentKey.Buffer, prefix);
                        tokenizer.MoveNext();
                        switch (tokenizer.Current)
                        {
                            case Token.ObjectStart:
                                if (n != null)
                                {
                                    n = _trie.Find(Literals.Dot, n);
                                    if (n == null)
                                        return;
                                    ReadObject(tokenizer, result, n);
                                }
                                else
                                {
                                    tokenizer.MoveToEndOfObject();
                                }
                                break;

                            case Token.ArrayStart:
                                tokenizer.MoveToEndOfArray();
                                break;

                            case Token.True:
                            case Token.Null:
                            case Token.False:
                            case Token.Integer:
                            case Token.Float:
                            case Token.String:
                                if (n != null)
                                {
                                    result.Add(new ExtractedValue(n.Value, tokenizer.GetValue()));
                                }
                                break;
                        }
                        break;
                }

                if (result.Count == _trie.Count)
                    break;
            }
        }
    }
}