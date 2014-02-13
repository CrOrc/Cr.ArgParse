namespace Cr.ArgParse.Tests
{
    public class TestNargsRemainder : ParserTestCase
    {
        public TestNargsRemainder()
        {
            ArgumentSignatures = new[] {new Argument("x"), new Argument("y") {IsRemainder = true, ActionName = "append"}, new Argument("-z")};
            Failures = new[] {"", "-z", "-z Z"};
            Successes = new SuccessCollection
            {
                {"X", new ParseResult {{"x", "X"}, {"y", new object[] {}}, {"z", null}}},
                {"-z Z X", new ParseResult {{"x", "X"}, {"y", new object[] {}}, {"z", "Z"}}},
                {"X A B -z Z", new ParseResult {{"x", "X"}, {"y", new[] {"A", "B", "-z", "Z"}}, {"z", null}}},
                {"X Y --foo", new ParseResult {{"x", "X"}, {"y", new[] {"Y", "--foo"}}, {"z", null}}}
            };
        }
    }
}