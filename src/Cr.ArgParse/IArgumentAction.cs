namespace Cr.ArgParse
{
    public interface IArgumentAction
    {
        void Call(IArgumentParser parser, ParseResult parseResult, object values, string optionString);
    }
}