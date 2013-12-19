namespace Cr.ArgParse
{
    public class ParseResult : IParseResult
    {
        public T GetArgument<T>(string argName)
        {
            return default(T);
        }
    }
}