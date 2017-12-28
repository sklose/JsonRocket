using System;
using System.Collections.Generic;

namespace JsonRocket
{
    public class Tokenizer
    {
        private readonly Stack<Token> _parents;
        private byte[] _data;
        private Token _current;
        private int _index, _start, _end;

        public Tokenizer()
        {
            _data = new byte[] { };
            _index = -1;
            _current = Token.Undefined;
            _parents = new Stack<Token>(1024);
        }

        public Token Current => _current;

        public void Reset(byte[] data)
        {
            _data = data;
            _index = -1;
            _current = Token.Undefined;
            _parents.Clear();
        }

        public void MoveToNext(Token token)
        {
            while (_current != token && _current != Token.Error && MoveNext())
            {

            }
        }

        public bool MoveNext()
        {
            skip:
            _index++;
            if (_index == _data.Length)
            {
                return false;
            }

            var b = _data[_index];
            switch (b)
            {
                case Literals.ObjectStart:
                    _parents.Push(_current = Token.ObjectStart);
                    break;

                case Literals.ObjectEnd:
                    _current = VerifyParentIs(Token.ObjectStart, Token.ObjectEnd);
                    break;

                case Literals.ArrayStart:
                    _parents.Push(_current = Token.ArrayStart);
                    break;

                case Literals.ArrayEnd:
                    _current = VerifyParentIs(Token.ArrayStart, Token.ArrayEnd);
                    break;

                case Literals.SingleQuote:
                case Literals.DoubleQuote:
                    _start = _index + 1;
                    if (ReadString(b))
                    {
                        _end = _index - 1;
                        _current = _current == Token.ObjectStart 
                            ? Token.Key 
                            : Token.String;
                    }
                    else
                    {
                        _current = Token.Error;
                    }
                    break;

                case Literals.True0:
                    _index += Literals.True1P.Length;
                    _current = Token.True;
                    break;

                case Literals.False0:
                    _index += Literals.False1P.Length;
                    _current = Token.False;
                    break;

                case Literals.Null0:
                    _index += Literals.Null1P.Length;
                    _current = Token.Null;
                    break;

                case Literals.Colon:
                    if (_current != Token.Key)
                    {
                        _current = Token.Error;
                    }
                    else
                    {
                        goto skip;
                    }
                    break;

                case Literals.Comma:
                    if (_parents.Count > 0)
                    {
                        _current = _parents.Peek();
                        goto skip;
                    }
                    else
                    {
                        _current = Token.Error;
                    }
                    break;

                case Literals.Whitespace1:
                case Literals.Whitespace2:
                case Literals.Whitespace3:
                case Literals.Whitespace4:
                    goto skip;

                default:
                    _start = _index;
                    _current = ReadNumber();
                    _end = _index;
                    break;
            }

            return _current != Token.Error;
        }

        public ArraySegment<byte> GetTokenBounds()
        {
            if (_current != Token.Float &&
                _current != Token.Integer &&
                _current != Token.Key &&
                _current != Token.String)
            {
                throw new InvalidOperationException();
            }

            return new ArraySegment<byte>(_data, _start, _end - _start + 1);
        }

        private Token VerifyParentIs(Token token, Token next)
        {
            if (_parents.Count == 0) return Token.Error;
            return _parents.Pop() == token ? next : Token.Error;
        }

        private bool ReadString(byte endQuote)
        {
            while (++_index != _data.Length)
            {
                var b = _data[_index];
                if (b == Literals.EscapeChar)
                {
                    _index++;
                    continue;
                }

                if (b == endQuote)
                    return true;
            }

            return false;
        }

        private Token ReadNumber()
        {
            var result = Token.Integer;
            do
            {
                switch (_data[_index])
                {
                    case (byte) '-':
                    case (byte) '0':
                    case (byte) '1':
                    case (byte) '2':
                    case (byte) '3':
                    case (byte) '4':
                    case (byte) '5':
                    case (byte) '6':
                    case (byte) '7':
                    case (byte) '8':
                    case (byte) '9':
                        break;

                    case (byte) '+':
                    case (byte) '.':
                    case (byte) 'e':
                    case (byte) 'E':
                        result = Token.Float;
                        break;

                    case Literals.Whitespace1:
                    case Literals.Whitespace2:
                    case Literals.Whitespace3:
                    case Literals.Whitespace4:
                    case Literals.Comma:
                    case Literals.ArrayEnd:
                    case Literals.ObjectEnd:
                        _index--;
                        return result;

                    default:
                        _index--;
                        return Token.Error;
                }
            } while (++_index < _data.Length);

            return Token.Error;
        }
    }
}
