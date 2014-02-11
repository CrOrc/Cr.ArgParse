namespace Cr.ArgParse.Tests
{
    public class TestOptionalsNumeric : ParserTestCase
    {
        public TestOptionalsNumeric()
        {
            ArgumentSignatures = new[] {new Argument("-1") {Destination = "one"}};
            Failures = new[] {"-1", "a", "-1 --foo", "-1 -y", "-1 -1", "-1 -2"};
            Successes = new SuccessCollection
            {
                {"-1 a", new ParseResult {{"one", "a"}}},
                {"-1a", new ParseResult {{"one", "a"}}},
                {"-1-2", new ParseResult {{"one", "-2"}}},
            };
        }
    }
}