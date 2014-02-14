namespace Cr.ArgParse.Tests.TestCases
{
    public class TestOptionalsDoubleDash : ParserTestCase
    {
        public TestOptionalsDoubleDash()
        {
            ArgumentSignatures = new[] {new Argument("--foo")};
            Failures = new[] {"--foo", "-f", "-f a", "a", "--foo -x", "--foo --bar"};
            Successes = new SuccessCollection
            {
                {"--foo a", new ParseResult {{"foo", "a"}}},
                {"--foo=a", new ParseResult {{"foo", "a"}}},
                {"--foo -2.5", new ParseResult {{"foo", "-2.5"}}},
                {"--foo=-2.5", new ParseResult {{"foo", "-2.5"}}}
            };
        }
    }
}