using System.Collections;
using System.Linq;

namespace Cr.ArgParse.Actions
{
    public class AppendAction : ArgumentAction
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
                    .ToList();
            parseResult[Destination] = newValues;
        }
    }
}