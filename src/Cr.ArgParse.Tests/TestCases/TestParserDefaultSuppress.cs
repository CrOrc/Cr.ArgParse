namespace Cr.ArgParse.Tests.TestCases
{
    public class TestParserDefaultSuppress : ParserTestCase
    {
        public TestParserDefaultSuppress()
        {
            ArgumentSignatures = new[]
            {
                new Argument("foo") {ValueCount = new ValueCount("?")},
                new Argument("bar") {ValueCount = new ValueCount("*")},
                new Argument("--baz") {ActionName = "store_true"}
            };
            Failures = new[] {"-x"};
            Successes = new SuccessCollection
            {
                {"", new ParseResult {}},
                {"a", new ParseResult {{"foo", "a"}}},
                {"a b", new ParseResult {{"bar", new[] {"b"}}, {"foo", "a"}}},
                {"--baz", new ParseResult {{"baz", true}}},
                {"a --baz", new ParseResult {{"baz", true}, {"foo", "a"}}},
                {"--baz a b", new ParseResult {{"bar", new[] {"b"}}, {"baz", true}, {"foo", "a"}}}
            };
        }
    }

}