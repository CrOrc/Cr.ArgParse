namespace Cr.ArgParse
{
    public interface IArgumentAction
    {
        Argument Argument { get; }
        ArgumentActionContainer Container { get; set; }
        void Call(IArgumentParser parser, ParseResult parseResult, object values, string optionString);
    }
}