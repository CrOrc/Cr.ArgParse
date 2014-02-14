namespace Cr.ArgParse.Tests
{
    public class TestNargsZeroOrMore : ParserTestCase
    {
        public TestNargsZeroOrMore()
        {
            ArgumentSignatures = new[]
            {
                new Argument("-x") {ValueCount = new ValueCount("*")},
                new Argument("y") {ValueCount = new ValueCount("*")}
            };

            Successes = new SuccessCollection
            {
                {"", new ParseResult {{"x", null}, {"y", new object[] {}}}},
                {"-x", new ParseResult {{"x", new object[] {}}, {"y", new object[] {}}}},
                {"-x a", new ParseResult {{"x", new object[] {"a"}}, {"y", new object[] {}}}},
                {"-x a -- b", new ParseResult {{"x", new object[] {"a"}}, {"y", new object[] {"b"}}}},
                {"a", new ParseResult {{"x", null}, {"y", new object[] {"a"}}}},
                {"a -x", new ParseResult {{"x", new object[] {}}, {"y", new object[] {"a"}}}},
                {"a -x b", new ParseResult {{"x", new object[] {"b"}}, {"y", new object[] {"a"}}}}
            };
        }
    }
}