namespace Cr.ArgParse.Actions
{
    public class CountAction : Action
    {
        private readonly ValueCount valueCount;

        public CountAction(Argument argument, IActionContainer container) : base(argument, container)
        {
            valueCount = new ValueCount(0);
        }

        public override ValueCount ValueCount
        {
            get { return valueCount; }
        }

        public override void Call(ParseResult parseResult, object values, string optionString)
        {
            parseResult[Destination] = parseResult.Get(Destination, 0) + 1;
        }
    }
}