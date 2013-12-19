namespace Cr.ArgParse
{
    public interface IParseResult
    {
        T GetArgument<T>(string argName);
    }
}