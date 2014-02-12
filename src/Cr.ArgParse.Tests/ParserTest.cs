using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Cr.ArgParse.Tests
{
    [TestFixture] public class ParserTest : BaseTest
    {
        [Test,
        /*TestCaseSource(typeof (TestOptionalsSingleDash)),
         TestCaseSource(typeof (TestOptionalsSingleDashCombined)),
         TestCaseSource(typeof (TestOptionalsSingleDashLong)),
         TestCaseSource(typeof (TestOptionalsSingleDashSubsetAmbiguous)),
         TestCaseSource(typeof (TestOptionalsSingleDashAmbiguous)),
         TestCaseSource(typeof (TestOptionalsNumeric)),
         TestCaseSource(typeof (TestOptionalsDoubleDash)),
         TestCaseSource(typeof (TestOptionalsDoubleDashPartialMatch)),
         TestCaseSource(typeof (TestOptionalsDoubleDashPrefixMatch)),
         TestCaseSource(typeof (TestOptionalsSingleDoubleDash))*/
         TestCaseSource(typeof (SimpleParserTestCases))
        ] public void ParseArgumentsTest(Func<Parser> parserFactory,
            IEnumerable<string> args, ParseResult expectedResult, Type expectedExceptionType)
        {
            var parser = parserFactory();
            ParseResult res;
            Exception occuredException;
            try
            {
                res = parser.ParseArguments(args);
                occuredException = null;
            }
            catch (Exception err)
            {
                res = null;
                occuredException = err;
                if (expectedExceptionType == null)
                    throw;
            }
            if (expectedExceptionType != null)
                Assert.That(occuredException, Is.InstanceOf(expectedExceptionType));
            else
                Asserter.AreEqual(expectedResult, res);
        }
    }
}