namespace Cr.ArgParse
{
    public class CountAction : ArgumentAction
    {
        public CountAction(Argument argument) : base(argument)
        {
        }

        public override void Call(ParseResult parseResult, object values, string optionString)
        {
            var res = parseResult.GetArgument(Destination, 0);
            parseResult.SaveArgument(Destination, res + 1);
        }
    }
}