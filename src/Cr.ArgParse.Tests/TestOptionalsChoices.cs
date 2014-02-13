namespace Cr.ArgParse.Tests
{
    [IgnoreCase] public class TestOptionalsChoices : ParserTestCase
    {
        public TestOptionalsChoices()
        {
            ArgumentSignatures = new[] {new Argument("-f"), new Argument("-g") {TypeName = "int"}};
            Failures = new[] {"a", "-f d", "-fad", "-ga", "-g 6"};
            Successes = new SuccessCollection
            {
                {"", new ParseResult {{"f", null}, {"g", null}}},
                {"-f a", new ParseResult {{"f", "a"}, {"g", null}}},
                {"-f c", new ParseResult {{"f", "c"}, {"g", null}}},
                {"-g 0", new ParseResult {{"f", null}, {"g", 0}}},
                {"-g 03", new ParseResult {{"f", null}, {"g", 3}}},
                {"-fb -g4", new ParseResult {{"f", "b"}, {"g", 4}}}
            };
        }
    }
}