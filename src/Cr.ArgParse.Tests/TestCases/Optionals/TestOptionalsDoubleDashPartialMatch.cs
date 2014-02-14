namespace Cr.ArgParse.Tests.TestCases.Optionals
{
    public class TestOptionalsDoubleDashPartialMatch : ParserTestCase
    {
        public TestOptionalsDoubleDashPartialMatch()
        {
            ArgumentSignatures = new[] {new Argument("--badger") {ActionName = "store_true"}, new Argument("--bat")};
            Failures = new[] {"--bar", "--b", "--ba", "--b=2", "--ba=4", "--badge 5"};
            Successes = new SuccessCollection
            {
                {"", new ParseResult {{"badger", false}, {"bat", null}}},
                {"--bat X", new ParseResult {{"badger", false}, {"bat", "X"}}},
                {"--bad", new ParseResult {{"badger", true}, {"bat", null}}},
                {"--badg", new ParseResult {{"badger", true}, {"bat", null}}},
                {"--badge", new ParseResult {{"badger", true}, {"bat", null}}},
                {"--badger", new ParseResult {{"badger", true}, {"bat", null}}}
            };
        }
    }
}