namespace Cr.ArgParse.Tests
{
    public class TestPositionalsChoicesString : ParserTestCase
    {
        public TestPositionalsChoicesString()
        {
            ArgumentSignatures = new[]
            {
                new Argument("spam")
                {
                    Choices = new [] {"a", "b", "c", "d", "e", "f", "g"}
                }
            };
            Failures = new[] {"", "--foo", "h", "42", "ef"};
            Successes = new SuccessCollection
            {
                {"a", new ParseResult {{"spam", "a"}}},
                {"g", new ParseResult {{"spam", "g"}}}
            };
        }
    }
}