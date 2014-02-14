namespace Cr.ArgParse.Actions
{
    public interface IArgumentAction
    {
        void Call(ParseResult parseResult, object values, string optionString);
    }
}