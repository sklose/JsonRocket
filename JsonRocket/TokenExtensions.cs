using System;

namespace JsonRocket
{
    public static class TokenExtensions
    {
        private static readonly bool[] HasValueMap;

        static TokenExtensions()
        {
            var values = Enum.GetValues(typeof(Token));
            HasValueMap = new bool[values.Length];
            HasValueMap[(int)Token.Float] = true;
            HasValueMap[(int)Token.Integer] = true;
            HasValueMap[(int)Token.Key] = true;
            HasValueMap[(int)Token.Null] = true;
            HasValueMap[(int)Token.String] = true;
            HasValueMap[(int)Token.True] = true;
            HasValueMap[(int)Token.False] = true;
        }

        public static bool HasValue(this Token token)
        {
            return HasValueMap[(int)token];
        }
    }
}