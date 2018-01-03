namespace JsonRocket.Test
{
    internal static class Helper
    {
        public static void MoveToNext(this Tokenizer tokenizer, Token token)
        {
            while (tokenizer.Current != token && tokenizer.Current != Token.Error && tokenizer.MoveNext())
            {
            }
        }
    }
}