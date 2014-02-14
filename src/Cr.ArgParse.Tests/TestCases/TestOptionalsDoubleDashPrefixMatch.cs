namespace Cr.ArgParse.Tests.TestCases
{
    public class TestOptionalsDoubleDashPrefixMatch : ParserTestCase
    {
        public TestOptionalsDoubleDashPrefixMatch()
        {
            ArgumentSignatures = new[] {new Argument("--badger") {ActionName = "store_true"}, new Argument("--ba")};
            Failures = new[] {"--bar", "--b", "--ba", "--b=2", "--badge 5"};
            Successes = new SuccessCollection
            {
                {"", new ParseResult {{"ba", null}, {"badger", false}}},
                {"--ba X", new ParseResult {{"ba", "X"}, {"badger", false}}},
                {"--ba=X", new ParseResult {{"ba", "X"}, {"badger", false}}},
                {"--bad", new ParseResult {{"ba", null}, {"badger", true}}},
                {"--badg", new ParseResult {{"ba", null}, {"badger", true}}},
                {"--badge", new ParseResult {{"ba", null}, {"badger", true}}},
                {"--badger", new ParseResult {{"ba", null}, {"badger", true}}}
            };
        }
    }
}