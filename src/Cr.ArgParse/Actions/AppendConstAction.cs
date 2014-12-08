using System.Collections;
using System.Linq;

namespace Cr.ArgParse.Actions
{
    public class AppendConstAction : Action
    {
        private readonly ValueCount valueCount;

        public AppendConstAction(Argument argument, IActionContainer container)
            : base(argument, container)
        {
            valueCount = new ValueCount(0);
        }

        public override ValueCount ValueCount
        {
            get { return valueCount; }
        }

        public override void Call(ParseResult parseResult, object values, string optionString)
        {
            var newValues =
                parseResult.Get<IEnumerable>(Destination, new object[] {})
                    .Cast<object>()
                    .Concat(new[] {ConstValue})
                    .ToArray();
            parseResult[Destination] = newValues;
        }
    }
}