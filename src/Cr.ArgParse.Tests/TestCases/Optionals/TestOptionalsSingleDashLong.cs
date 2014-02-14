namespace Cr.ArgParse.Tests.TestCases.Optionals
{
    public class TestOptionalsSingleDashLong : ParserTestCase
    {
        public TestOptionalsSingleDashLong()
        {
            ArgumentSignatures = new[] {new Argument("-foo")};
            Failures = new[] {"-foo", "a", "--foo", "-foo --foo", "-foo -y", "-fooa"};
            Successes = new SuccessCollection
            {
                {"", new ParseResult {{"foo", null}}},
                {"-foo a", new ParseResult {{"foo", "a"}}},
                {"-foo -1", new ParseResult {{"foo", "-1"}}},
                {"-fo a", new ParseResult {{"foo", "a"}}},
                {"-f a", new ParseResult {{"foo", "a"}}}
            };
        }
    }
}