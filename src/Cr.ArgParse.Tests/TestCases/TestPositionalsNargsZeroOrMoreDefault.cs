namespace Cr.ArgParse.Tests.TestCases
{
    public class TestPositionalsNargsZeroOrMoreDefault : ParserTestCase
    {
        public TestPositionalsNargsZeroOrMoreDefault()
        {
            ArgumentSignatures = new[] {new Argument("foo") {DefaultValue = "bar", ValueCount = new ValueCount("*")}};
            Failures = new[] {"-x"};
            Successes = new SuccessCollection
            {
                {"", new ParseResult {{"foo", "bar"}}},
                {"a", new ParseResult {{"foo", new[] {"a"}}}},
                {"a b", new ParseResult {{"foo", new[] {"a", "b"}}}}
            };
        }
    }
}