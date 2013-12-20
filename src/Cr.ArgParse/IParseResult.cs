namespace Cr.ArgParse
{
    public interface IParseResult
    {
        T GetArgument<T>(string argName, T defaultValue = default (T));
    }
}