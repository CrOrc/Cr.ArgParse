using System;

namespace Cr.ArgParse.Tests
{
    public class TestOptionalsSingleDashAmbiguous : ParserTestCase
    {
        public TestOptionalsSingleDashAmbiguous()
        {
            ArgumentSignatures = new[]
            {
                new Argument {OptionStrings = new[] {"-foobar"}},
                new Argument {OptionStrings = new[] {"-foorab"}}
            };

            Failures = new[] {"-f", "-f a", "-fa", "-foa", "-foo", "-fo", "-foo b"};

            Successes = new[]
            {
                Tuple.Create("", new ParseResult {{"foobar", null}, {"foorab", null}}),
                Tuple.Create("-foob a", new ParseResult {{"foobar", "a"}, {"foorab", null}}),
                Tuple.Create("-foor a", new ParseResult {{"foobar", null}, {"foorab", "a"}}),
                Tuple.Create("-fooba a", new ParseResult {{"foobar", "a"}, {"foorab", null}}),
                Tuple.Create("-foora a", new ParseResult {{"foobar", null}, {"foorab", "a"}}),
                Tuple.Create("-foobar a", new ParseResult {{"foobar", "a"}, {"foorab", null}}),
                Tuple.Create("-foorab a", new ParseResult {{"foobar", null}, {"foorab", "a"}})
            };
        }
    }
}