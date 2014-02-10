    using System;

namespace Cr.ArgParse.Tests
{
    public class TestOptionalsSingleDashLong : ParserTestCase
    {
        public TestOptionalsSingleDashLong()
        {
            ArgumentSignatures = new[] {new Argument {OptionStrings = new[] {"-foo"}}};
            Failures = new[] {"-foo", "a", "--foo", "-foo --foo", "-foo -y", "-fooa"};
            Successes = new[]
            {
                Tuple.Create("", new ParseResult {{"foo", null}}),
                Tuple.Create("-foo a", new ParseResult {{"foo", "a"}}),
                Tuple.Create("-foo -1", new ParseResult {{"foo", "-1"}}),
                Tuple.Create("-fo a", new ParseResult {{"foo", "a"}}),
                Tuple.Create("-f a", new ParseResult {{"foo", "a"}})
            };

        }
    }
}