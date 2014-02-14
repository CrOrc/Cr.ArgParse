namespace Cr.ArgParse.Actions
{
    public interface IAction
    {
        void Call(ParseResult parseResult, object values, string optionString);
    }
}