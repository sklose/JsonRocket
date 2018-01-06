using System;

namespace JsonRocket
{
    public static class TokenExtensions
    {
        private static readonly bool[] HasValueMap;
        private static readonly ValueType[] ValueTypeMap;

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

            ValueTypeMap = new ValueType[values.Length];
            ValueTypeMap[(int)Token.Float] = ValueType.Float;
            ValueTypeMap[(int)Token.Integer] = ValueType.Integer;
            ValueTypeMap[(int)Token.Key] = ValueType.String;
            ValueTypeMap[(int)Token.Null] = ValueType.Null;
            ValueTypeMap[(int)Token.String] = ValueType.String;
            ValueTypeMap[(int)Token.True] = ValueType.True;
            ValueTypeMap[(int)Token.False] = ValueType.False;
        }

        public static bool HasValue(this Token token)
        {
            return HasValueMap[(int)token];
        }

        internal static ValueType GetValueType(this Token token)
        {
            return ValueTypeMap[(int)token];
        }
    }
}