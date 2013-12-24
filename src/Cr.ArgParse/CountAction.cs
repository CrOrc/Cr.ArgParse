namespace Cr.ArgParse
{
    public class CountAction : ArgumentAction
    {
        public CountAction(Argument argument) : base(argument)
        {
        }

        public override void Call(ParseResult parseResult, object values, string optionString)
        {
            parseResult[Destination]= parseResult.GetArgument(Destination, 0) + 1;
        }
    }
}