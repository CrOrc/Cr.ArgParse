using System;
using System.Linq;

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
                    Choices = "abcdefg".Split(new string[] {}, StringSplitOptions.None).Cast<object>().ToArray()
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