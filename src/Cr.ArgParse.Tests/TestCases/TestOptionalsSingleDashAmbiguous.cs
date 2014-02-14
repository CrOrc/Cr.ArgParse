namespace Cr.ArgParse.Tests.TestCases
{
    public class TestOptionalsSingleDashAmbiguous : ParserTestCase
    {
        public TestOptionalsSingleDashAmbiguous()
        {
            ArgumentSignatures = new[]
            {
                new Argument("-foobar"),
                new Argument("-foorab")
            };

            Failures = new[] {"-f", "-f a", "-fa", "-foa", "-foo", "-fo", "-foo b"};

            Successes = new SuccessCollection
            {
                {"", new ParseResult {{"foobar", null}, {"foorab", null}}},
                {"-foob a", new ParseResult {{"foobar", "a"}, {"foorab", null}}},
                {"-foor a", new ParseResult {{"foobar", null}, {"foorab", "a"}}},
                {"-fooba a", new ParseResult {{"foobar", "a"}, {"foorab", null}}},
                {"-foora a", new ParseResult {{"foobar", null}, {"foorab", "a"}}},
                {"-foobar a", new ParseResult {{"foobar", "a"}, {"foorab", null}}},
                {"-foorab a", new ParseResult {{"foobar", null}, {"foorab", "a"}}}
            };
        }
    }
}