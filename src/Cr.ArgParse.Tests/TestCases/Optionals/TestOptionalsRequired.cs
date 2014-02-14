namespace Cr.ArgParse.Tests.TestCases.Optionals
{
    public class TestOptionalsRequired : ParserTestCase
    {
        public TestOptionalsRequired()
        {
            ArgumentSignatures = new[] {new Argument("-x") {TypeName = "int", IsRequired = true}};
            Failures = new[] {"a", ""};
            Successes = new SuccessCollection
            {
                {"-x 1", new ParseResult {{"x", 1}}},
                {"-x42", new ParseResult {{"x", 42}}}
            };
        }
    }
}