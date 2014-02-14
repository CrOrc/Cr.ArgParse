using System.Linq;

namespace Cr.ArgParse.Tests.TestCases
{
    public class TestPositionalsChoicesInt : ParserTestCase
    {
        public TestPositionalsChoicesInt()
        {
            ArgumentSignatures = new[] {new Argument("spam") {TypeName = "int", Choices = Enumerable.Range(0,20).Cast<object>().ToList()}};
            Failures = new[] {"", "--foo", "h", "42", "ef"};
            Successes = new SuccessCollection
            {
                {"4", new ParseResult {{"spam", 4}}},
                {"15", new ParseResult {{"spam", 15}}}
            };
        }
    }
}