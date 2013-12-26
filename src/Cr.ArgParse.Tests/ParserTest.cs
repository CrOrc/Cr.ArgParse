using NUnit.Framework;

namespace Cr.ArgParse.Tests
{
    [TestFixture] public class ParserTest : BaseTest
    {
        [Test]
        public void CreateOptionalDefault()
        {
            var parser = new Parser();
            parser.AddArgument(new Argument { OptionStrings = new[] { "-e", "--example" }, DefaultValue = "example value"});
            var res = parser.ParseArguments(new string[] {});
            var expectedResult = new ParseResult {{"example", "example value"}};
            Asserter.AreEqual(expectedResult,res);
        }

    }
}