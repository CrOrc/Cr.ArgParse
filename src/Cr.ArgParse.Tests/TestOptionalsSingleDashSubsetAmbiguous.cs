using System;

namespace Cr.ArgParse.Tests
{
    public class TestOptionalsSingleDashSubsetAmbiguous : ParserTestCase
    {
        public TestOptionalsSingleDashSubsetAmbiguous()
        {
            ArgumentSignatures = new[]
            {
                new Argument {OptionStrings = new[] {"-f"}}, new Argument {OptionStrings = new[] {"-foobar"}},
                new Argument {OptionStrings = new[] {"-foorab"}}
            };
            Failures = new[] {"-f", "-foo", "-fo", "-foo b", "-foob", "-fooba", "-foora"};
            Successes = new[]
            {
                Tuple.Create("", new ParseResult {{"f", null}, {"foobar", null}, {"foorab", null}}),
                Tuple.Create("-f a", new ParseResult {{"f", "a"}, {"foobar", null}, {"foorab", null}}),
                Tuple.Create("-fa", new ParseResult {{"f", "a"}, {"foobar", null}, {"foorab", null}}),
                Tuple.Create("-foa", new ParseResult {{"f", "oa"}, {"foobar", null}, {"foorab", null}}),
                Tuple.Create("-fooa", new ParseResult {{"f", "ooa"}, {"foobar", null}, {"foorab", null}}),
                Tuple.Create("-foobar a", new ParseResult {{"f", null}, {"foobar", "a"}, {"foorab", null}}),
                Tuple.Create("-foorab a", new ParseResult {{"f", null}, {"foobar", null}, {"foorab", "a"}})
            };
        }
    }
}