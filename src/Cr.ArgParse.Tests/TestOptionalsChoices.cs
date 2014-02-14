using System.Linq;

namespace Cr.ArgParse.Tests
{
    public class TestOptionalsChoices : ParserTestCase
    {
        public TestOptionalsChoices()
        {
            ArgumentSignatures = new[]
            {
                new Argument("-f") {Choices = new [] {"a", "b", "c"}},
                new Argument("-g") {TypeName = "int", Choices = Enumerable.Range(0, 5).Cast<object>().ToArray()}
            };
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