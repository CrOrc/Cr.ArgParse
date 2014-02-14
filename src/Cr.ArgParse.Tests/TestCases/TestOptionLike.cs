namespace Cr.ArgParse.Tests.TestCases
{
    public class TestOptionLike : ParserTestCase
    {
        public TestOptionLike()
        {
            ArgumentSignatures = new[]
            {
                new Argument("-x") {TypeName = "float"}, new Argument("-3") {Destination = "y", TypeName = "float"},
                new Argument("z") {ValueCount = new ValueCount("*")}
            };
            Failures = new[]
            {
                "-x", "-y2.5", "-xa", "-x -a", "-x -3", "-x -3.5", "-3 -3.5", "-x -2.5", "-x -2.5 a", "-3 -.5", "a x -1",
                "-x -1 a", "-3 -1 a"
            };
            Successes = new SuccessCollection
            {
                {"", new ParseResult {{"x", null}, {"y", null}, {"z", new object[] {}}}},
                {"-x 2.5", new ParseResult {{"x", 2.5}, {"y", null}, {"z", new object[] {}}}},
                {"-x 2.5 a", new ParseResult {{"x", 2.5}, {"y", null}, {"z", new[] {"a"}}}},
                {"-3.5", new ParseResult {{"x", null}, {"y", 0.5}, {"z", new object[] {}}}},
                {"-3-.5", new ParseResult {{"x", null}, {"y", -0.5}, {"z", new object[] {}}}},
                {"-3 .5", new ParseResult {{"x", null}, {"y", 0.5}, {"z", new object[] {}}}},
                {"a -3.5", new ParseResult {{"x", null}, {"y", 0.5}, {"z", new[] {"a"}}}},
                {"a", new ParseResult {{"x", null}, {"y", null}, {"z", new[] {"a"}}}},
                {"a -x 1", new ParseResult {{"x", 1.0}, {"y", null}, {"z", new[] {"a"}}}},
                {"-x 1 a", new ParseResult {{"x", 1.0}, {"y", null}, {"z", new[] {"a"}}}},
                {"-3 1 a", new ParseResult {{"x", null}, {"y", 1.0}, {"z", new[] {"a"}}}}
            };
        }
    }
}