namespace Cr.ArgParse
{
    public class CountAction : ArgumentAction
    {
        public CountAction(Argument argument) : base(argument)
        {
        }

        public override void Call(IArgumentParser parser, ParseResult parseResult, object values, string optionString)
        {
            var res = parseResult.GetArgument(Argument.Destination, 0);
            parseResult.SaveArgument(Argument.Destination, res + 1);
        }
    }
}