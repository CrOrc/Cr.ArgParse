namespace Cr.ArgParse
{
    public class StoreConstAction : ArgumentAction
    {
        private readonly ValueCount valueCount;

        public StoreConstAction(Argument argument)
            : base(argument)
        {
            valueCount = new ValueCount(0);
        }


        public override ValueCount ValueCount
        {
            get { return valueCount; }
        }

        public override void Call(ParseResult parseResult, object values, string optionString)
        {
            parseResult[Destination] = ConstValue;
        }
    }
}