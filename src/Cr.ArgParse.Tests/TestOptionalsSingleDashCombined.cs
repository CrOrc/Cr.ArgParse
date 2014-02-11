using System;

namespace Cr.ArgParse.Tests
{
    public class TestOptionalsSingleDashCombined : ParserTestCase
    {
        public TestOptionalsSingleDashCombined()
        {
            ArgumentSignatures = new[]
            {
                new Argument("-x") {ActionName = "store_true"},
                new Argument("-yyy") {ActionName = "store_const", ConstValue = 42},
                new Argument("-z")
            };

            Failures = new[]
            {
                "a", "--foo", "-xa", "-x --foo", "-x -z", "-z -x",
                "-yx", "-yz a", "-yyyx", "-yyyza", "-xyza"
            };
            Successes = new SuccessCollection
            {
                {"", new ParseResult {{"x", false}, {"yyy", null}, {"z", null}}},
                {"-x", new ParseResult {{"x", true}, {"yyy", null}, {"z", null}}},
                {"-za", new ParseResult {{"x", false}, {"yyy", null}, {"z", "a"}}},
                {"-z a", new ParseResult {{"x", false}, {"yyy", null}, {"z", "a"}}},
                {"-xza", new ParseResult {{"x", true}, {"yyy", null}, {"z", "a"}}},
                {"-xz a", new ParseResult {{"x", true}, {"yyy", null}, {"z", "a"}}},
                {"-x -za", new ParseResult {{"x", true}, {"yyy", null}, {"z", "a"}}},
                {"-x -z a", new ParseResult {{"x", true}, {"yyy", null}, {"z", "a"}}},
                {"-y", new ParseResult {{"x", false}, {"yyy", 42}, {"z", null}}},
                {"-yyy", new ParseResult {{"x", false}, {"yyy", 42}, {"z", null}}},
                {"-x -yyy -za", new ParseResult {{"x", true}, {"yyy", 42}, {"z", "a"}}},
                {"-x -yyy -z a", new ParseResult {{"x", true}, {"yyy", 42}, {"z", "a"}}}
            };
        }
    }
}