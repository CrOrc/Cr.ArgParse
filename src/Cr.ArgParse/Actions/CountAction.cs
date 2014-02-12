namespace Cr.ArgParse.Actions
{
    public class CountAction : ArgumentAction
    {
        private readonly ValueCount valueCount;

        public CountAction(Argument argument) : base(argument)
        {
            valueCount = new ValueCount(0);
        }

        public override ValueCount ValueCount
        {
            get { return valueCount; }
        }

        public override void Call(ParseResult parseResult, object values, string optionString)
        {
            parseResult[Destination] = parseResult.GetArgument(Destination, 0) + 1;
        }
    }
}