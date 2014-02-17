namespace Cr.ArgParse.Tests.TestCases
{
    public class TestOptionalsAlternatePrefixCharsMultipleShortArgs : ParserTestCase
    {
        public TestOptionalsAlternatePrefixCharsMultipleShortArgs()
        {
            ParserSignature = new ParserSettings {Prefixes = new[] {"+", "-"}};
            ArgumentSignatures = new[]
            {
                new Argument("-x") {ActionName = "store_true"}, new Argument("+y") {ActionName = "store_true"},
                new Argument("+z") {ActionName = "store_true"}
            };
            Failures = new[] {"-w", "-xyz", "+x", "-y", "+xyz"};
            Successes = new SuccessCollection
            {
                {"", new ParseResult {{"x", false}, {"y", false}, {"z", false}}},
                {"-x", new ParseResult {{"x", true}, {"y", false}, {"z", false}}},
                {"+y -x", new ParseResult {{"x", true}, {"y", true}, {"z", false}}},
                {"+yz -x", new ParseResult {{"x", true}, {"y", true}, {"z", true}}}
            };
        }
    }
}