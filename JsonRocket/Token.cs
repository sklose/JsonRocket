namespace JsonRocket
{
    public enum Token
    {
        Error = 0,
        Undefined,
        ObjectStart,
        ObjectEnd,
        ArrayStart,
        ArrayEnd,
        Key,
        String,
        Float,
        Integer,
        True,
        False,
        Null
    }
}