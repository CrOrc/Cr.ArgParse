namespace Cr.ArgParse.Tests.TestCases
{
    public class TestEmptyAndSpaceContainingArguments : ParserTestCase
    {
        public TestEmptyAndSpaceContainingArguments()
        {
            ArgumentSignatures = new[]
            {new Argument("x") {ValueCount = new ValueCount("?")}, new Argument("-y", "--yyy") {Destination = "y"}};
            Failures = new[] {"-y"};
            Successes = new SuccessCollection
            {
                {new[] {""}, new ParseResult {{"x", ""}, {"y", null}}},
                {new[] {"a badger"}, new ParseResult {{"x", "a badger"}, {"y", null}}},
                {new[] {"-a badger"}, new ParseResult {{"x", "-a badger"}, {"y", null}}},
                {new[] {"-y", ""}, new ParseResult {{"x", null}, {"y", ""}}},
                {new[] {"-y", "a badger"}, new ParseResult {{"x", null}, {"y", "a badger"}}},
                {new[] {"-y", "-a badger"}, new ParseResult {{"x", null}, {"y", "-a badger"}}},
                {new[] {"--yyy=a badger"}, new ParseResult {{"x", null}, {"y", "a badger"}}},
                {new[] {"--yyy=-a badger"}, new ParseResult {{"x", null}, {"y", "-a badger"}}}
            };
        }
    }
}