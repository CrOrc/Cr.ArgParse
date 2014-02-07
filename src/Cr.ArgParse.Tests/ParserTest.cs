using System.Collections.Generic;
using NUnit.Framework;

namespace Cr.ArgParse.Tests
{
    [TestFixture] public class ParserTest : BaseTest
    {
        [Test,
         TestCaseSource(typeof (TestOptionalsSingleDash), "SuccessCases"),
         TestCaseSource(typeof (TestOptionalsSingleDash), "FailureCases")] public void ParseArgumentsTest(Parser parser,
             IEnumerable<string> args, ParseResult expectedResult)
        {
            var res = parser.ParseArguments(args);
            Asserter.AreEqual(expectedResult, res);
        }
    }
}