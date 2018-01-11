using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

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

                if (result.Count == _trie.Count)
                    break;
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
                                    n = _trie.Find(Literals.DotBuffer, n);
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
                                if (n != null)
                                {
                                    n = _trie.Find(Literals.ArrayStartBuffer, n);
                                    if (n == null)
                                        return;
                                    ReadArray(tokenizer, result, n);
                                }
                                else
                                {
                                    tokenizer.MoveToEndOfArray();
                                }
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

        private void ReadArray(Tokenizer tokenizer, List<ExtractedValue> result, Trie.Node prefix)
        {
            int arrayIndex = 0;
            while (tokenizer.MoveNext())
            {
                switch (tokenizer.Current)
                {
                    case Token.ArrayEnd:
                        return;

                    case Token.ObjectStart:

                        var n = FindByArrayIndex(arrayIndex, prefix);
                        if (n != null)
                        {
                            ReadObject(tokenizer, result, n);
                        }
                        else
                        {
                            tokenizer.MoveToEndOfObject();
                        }

                        arrayIndex++;

                        break;
                }

                if (result.Count == _trie.Count)
                    break;
            }
        }

        private Trie.Node FindByArrayIndex(int value, Trie.Node n)
        {
            int digits = value == 0 ? 1 : (int)Math.Floor(Math.Log10(Math.Abs(value)) + 1);
            int currentIndex = 0;

            while (currentIndex < digits)
            {
                int divisor = (int)Math.Pow(10, digits - currentIndex - 1);

                var digitValue = (value / divisor) + 48;
                n = _trie.Find((byte)digitValue, n);
                if (n == null)
                    return null;

                currentIndex++;
                value -= divisor * digitValue;
            }

            n = _trie.Find(Literals.ArrayEndBuffer, n);
            if (n == null)
                return null;

            n = _trie.Find(Literals.DotBuffer, n);

            return n;
        }
    }
}