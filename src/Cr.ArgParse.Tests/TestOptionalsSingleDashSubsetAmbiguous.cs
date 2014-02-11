using System;

namespace Cr.ArgParse.Tests
{
    public class TestOptionalsSingleDashSubsetAmbiguous : ParserTestCase
    {
        public TestOptionalsSingleDashSubsetAmbiguous()
        {
            ArgumentSignatures = new[]
            {
                new Argument("-f"), new Argument("-foobar"), new Argument("-foorab")
            };
            Failures = new[] {"-f", "-foo", "-fo", "-foo b", "-foob", "-fooba", "-foora"};
            Successes = new SuccessCollection
            {
                {"", new ParseResult {{"f", null}, {"foobar", null}, {"foorab", null}}},
                {"-f a", new ParseResult {{"f", "a"}, {"foobar", null}, {"foorab", null}}},
                {"-fa", new ParseResult {{"f", "a"}, {"foobar", null}, {"foorab", null}}},
                {"-foa", new ParseResult {{"f", "oa"}, {"foobar", null}, {"foorab", null}}},
                {"-fooa", new ParseResult {{"f", "ooa"}, {"foobar", null}, {"foorab", null}}},
                {"-foobar a", new ParseResult {{"f", null}, {"foobar", "a"}, {"foorab", null}}},
                {"-foorab a", new ParseResult {{"f", null}, {"foobar", null}, {"foorab", "a"}}}
            };
        }
    }
}