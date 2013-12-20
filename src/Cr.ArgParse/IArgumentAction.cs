namespace Cr.ArgParse
{
    public interface IArgumentAction
    {
        Argument Argument { get; }
        void Call(IArgumentParser parser, ParseResult parseResult, object values, string optionString);
    }
}