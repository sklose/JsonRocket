namespace JsonRocket
{
    internal struct TokenStack
    {
        private Token[] _stack;
        private int _cursor;
        
        public void Initialize(int capacity = 1024)
        {
            _cursor = -1;
            _stack = new Token[capacity];
        }

        public void Clear()
        {
            _cursor = -1;
        }

        public bool TryPush(Token token)
        {
            if (++_cursor == _stack.Length)
            {
                --_cursor;
                return false;
            }

            _stack[_cursor] = token;
            return true;
        }

        public bool TryPop()
        {
            if (_cursor < 0)
            {
                return false;
            }

            --_cursor;
            return true;
        }

        public bool TryPeek(out Token token)
        {
            if (_cursor < 0)
            {
                token = default(Token);
                return false;
            }

            token = _stack[_cursor];
            return true;
        }
    }
}
