using System;
using System.IO;

namespace JsonRocket
{
    public class Tokenizer
    {
        private TokenStack _parents;
        private byte[] _data;
        private Token _current;
        private int _index, _start, _end;

        public Tokenizer()
        {
            _data = new byte[] { };
            _index = -1;
            _current = Token.Undefined;
            _parents = new TokenStack();
            _parents.Initialize();
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
                    if (!_parents.TryPush(_current = Token.ObjectStart))
                    {
                        _current = Token.Error;
                    }
                    break;

                case Literals.ObjectEnd:
                    _current = _parents.TryPop() ? Token.ObjectEnd : Token.Error;
                    break;

                case Literals.ArrayStart:
                    if (!_parents.TryPush(_current = Token.ArrayStart))
                    {
                        _current = Token.Error;
                    }
                    break;

                case Literals.ArrayEnd:
                    _current = _parents.TryPop() ? Token.ArrayEnd : Token.Error;
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

                case Literals.Comma:
                    if (!_parents.TryPeek(out _current))
                    {
                        _current = Token.Error;
                    }
                    goto skip;

                case Literals.Colon:
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
        
        private bool ReadString(byte endQuote)
        {
            for (_index = _index + 1; _index < _data.Length; _index++)
            {
                if (_data[_index] == endQuote && _data[_index - 1] != Literals.EscapeChar)
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
