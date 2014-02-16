using System.Collections;
using System.Linq;

namespace Cr.ArgParse.Actions
{
    public class AppendAction : Action
    {
        public AppendAction(Argument argument)
            : base(argument)
        {
        }

        public override void Call(ParseResult parseResult, object values, string optionString)
        {
            var newValues =
                parseResult.GetArgument<IEnumerable>(Destination, new object[] {})
                    .Cast<object>()
                    .Concat(new[] {values})
                    .ToArray();
            parseResult[Destination] = newValues;
        }
    }
}