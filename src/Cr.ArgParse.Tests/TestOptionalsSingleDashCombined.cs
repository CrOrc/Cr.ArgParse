using System;

namespace Cr.ArgParse.Tests
{
    public class TestOptionalsSingleDashCombined : ParserTestCase
    {
        public TestOptionalsSingleDashCombined()
        {
            ArgumentSignatures = new[]
            {
                new Argument {OptionStrings = new[] {"-x"}, ActionName = "store_true"},
                new Argument {OptionStrings = new[] {"-yyy"}, ActionName = "store_const", ConstValue = 42},
                new Argument {OptionStrings = new[] {"-z"}}
            };

            Failures = new[]
            {
                "a", "--foo", "-xa", "-x --foo", "-x -z", "-z -x",
                "-yx", "-yz a", "-yyyx", "-yyyza", "-xyza"
            };
            Successes = new[]
            {
                Tuple.Create("", new ParseResult {{"x", false}, {"yyy", null}, {"z", null}}),
                Tuple.Create("-x", new ParseResult {{"x", true}, {"yyy", null}, {"z", null}}),
                Tuple.Create("-za", new ParseResult {{"x", false}, {"yyy", null}, {"z", "a"}}),
                Tuple.Create("-z a", new ParseResult {{"x", false}, {"yyy", null}, {"z", "a"}}),
                Tuple.Create("-xza", new ParseResult {{"x", true}, {"yyy", null}, {"z", "a"}}),
                Tuple.Create("-xz a", new ParseResult {{"x", true}, {"yyy", null}, {"z", "a"}}),
                Tuple.Create("-x -za", new ParseResult {{"x", true}, {"yyy", null}, {"z", "a"}}),
                Tuple.Create("-x -z a", new ParseResult {{"x", true}, {"yyy", null}, {"z", "a"}}),
                Tuple.Create("-y", new ParseResult {{"x", false}, {"yyy", 42}, {"z", null}}),
                Tuple.Create("-yyy", new ParseResult {{"x", false}, {"yyy", 42}, {"z", null}}),
                Tuple.Create("-x -yyy -za", new ParseResult {{"x", true}, {"yyy", 42}, {"z", "a"}}),
                Tuple.Create("-x -yyy -z a", new ParseResult {{"x", true}, {"yyy", 42}, {"z", "a"}})
            };
        }
    }
}