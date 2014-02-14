namespace Cr.ArgParse.Tests.TestCases.Optionals
{
    public class TestOptionalsSingleDoubleDash : ParserTestCase
    {
        public TestOptionalsSingleDoubleDash()
        {
            ArgumentSignatures = new[]
            {
                new Argument("-f") {ActionName = "store_true"}, new Argument("--bar"),
                new Argument("-baz") {ActionName = "store_const", ConstValue = 42}
            };
            Failures = new[] {"--bar", "-fbar", "-fbaz", "-bazf", "-b B", "B"};
            Successes = new SuccessCollection
            {
                {"", new ParseResult {{"bar", null}, {"baz", null}, {"f", false}}},
                {"-f", new ParseResult {{"bar", null}, {"baz", null}, {"f", true}}},
                {"--ba B", new ParseResult {{"bar", "B"}, {"baz", null}, {"f", false}}},
                {"-f --bar B", new ParseResult {{"bar", "B"}, {"baz", null}, {"f", true}}},
                {"-f -b", new ParseResult {{"bar", null}, {"baz", 42}, {"f", true}}},
                {"-ba -f", new ParseResult {{"bar", null}, {"baz", 42}, {"f", true}}}
            };
        }
    }
}