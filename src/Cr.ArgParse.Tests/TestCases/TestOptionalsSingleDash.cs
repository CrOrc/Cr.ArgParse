namespace Cr.ArgParse.Tests.TestCases
{
    public class TestOptionalsSingleDash : ParserTestCase
    {
        public TestOptionalsSingleDash()
        {
            ArgumentSignatures = new[] {new Argument("-x")};
            Failures = new[] {"-x", "a", "--foo", "-x --foo", "-x -y"};
            Successes = new SuccessCollection
            {
                {"", new ParseResult {{"x", null}}},
                {"-x a", new ParseResult {{"x", "a"}}},
                {"-xa", new ParseResult {{"x", "a"}}},
                {"-x -1", new ParseResult {{"x", "-1"}}},
                {"-x-1", new ParseResult {{"x", "-1"}}}
            };
        }
    }
}