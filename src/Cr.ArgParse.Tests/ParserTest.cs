using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Cr.ArgParse.Tests
{
    [TestFixture] public class ParserTest : BaseTest
    {
        [Test,
         TestCaseSource(typeof (TestOptionalsSingleDash), "TestCases")] public void ParseArgumentsTest(Parser parser,
             IEnumerable<string> args, ParseResult expectedResult, Type expectedExceptionType)
        {
            ParseResult res;
            try
            {
                res = parser.ParseArguments(args);
            }
            catch (Exception err)
            {
                res = null;
                if (expectedExceptionType != null)
                    Assert.That(err, Is.InstanceOf(expectedExceptionType));
                else
                    throw;
            }
            Asserter.AreEqual(expectedResult, res);
        }
    }
}