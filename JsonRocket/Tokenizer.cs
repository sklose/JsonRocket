using System;

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
                    Skip(Literals.TrueSkipLength, Token.True);
                    break;

                case Literals.False0:
                    Skip(Literals.FalseSkipLength, Token.False);
                    break;

                case Literals.Null0:
                    Skip(Literals.NullSkipLength, Token.Null);
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

        public ArraySegment<byte> GetTokenValue()
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

        private void Skip(int count, Token success)
        {
            _current = success;
            _index += count;
            if (_index >= _data.Length)
            {
                _index -= count;
                _current = Token.Error;
            }
        }
        
        private bool ReadString(byte endQuote)
        {
            for (_index = _index + 1; _index < _data.Length; _index++)
            {
                if (_data[_index] == endQuote)
                    return true;

                if (_data[_index] == Literals.EscapeChar)
                    _index++;
            }

            return false;
        }

        private Token ReadNumber()
        {
            bool isFloat = false;
            for (; _index < _data.Length; _index++)
            {
                var e = Literals.NumberElements[_data[_index]];
                isFloat |= e.IsFloat;

                if (e.IsEnd)
                {
                    _index--;
                    break;
                }

                if (e.IsError)
                {
                    _index--;
                    return Token.Error;
                }
            }

            return isFloat ? Token.Float : Token.Integer;
        }
    }
}
