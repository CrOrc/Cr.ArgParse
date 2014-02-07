using System;

namespace Cr.ArgParse.Tests
{
    public class TestOptionalsSingleDash : ParserTestCase
    {
        public TestOptionalsSingleDash()
        {
            ArgumentSignatures = new[] {new Argument {OptionStrings = new[] {"-x"}}};
            Failures = new[] {"-x", "a", "--foo", "-x --foo", "-x -y"};
            Successes = new[]
            {
                Tuple.Create("", new ParseResult {{"x", null}}),
                Tuple.Create("-x a", new ParseResult {{"x", "a"}}),
                Tuple.Create("-xa", new ParseResult {{"x", "a"}}),
                Tuple.Create("-x -1", new ParseResult {{"x", "-1"}}),
                Tuple.Create("-x-1", new ParseResult {{"x", "-1"}})
            };
        }
    }
}