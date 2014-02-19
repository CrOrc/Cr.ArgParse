using System;
using System.Collections.Generic;
using Cr.ArgParse.Exceptions;
using NUnit.Framework;

namespace Cr.ArgParse.Tests
{
    [TestFixture] public class ParserTest : BaseTest
    {
        [Test,
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

        [Test] public void PositionalsGroupsTestNonGroupFirst()
        {
            var parser = new Parser();
            parser.AddArgument("foo");
            var group = parser.AddArgumentGroup("g");
            group.AddArgument("bar");
            parser.AddArgument("baz");
            Asserter.AreEqual(new ParseResult {{"foo", "1"}, {"bar", "2"}, {"baz", "3"}},
                parser.ParseArguments("1 2 3".Split(new char[] {})));
        }

        [Test] public void PositionalsGroupsTestGroupFirst()
        {
            var parser = new Parser();
            var group = parser.AddArgumentGroup("xxx");
            group.AddArgument("foo");
            parser.AddArgument("bar");
            parser.AddArgument("baz");
            Asserter.AreEqual(new ParseResult {{"foo", "1"}, {"bar", "2"}, {"baz", "3"}},
                parser.ParseArguments("1 2 3".Split(new char[] {})));
        }

        [Test] public void PositionalsGroupsTestInterleavedGroups()
        {
            var parser = new Parser();
            var group1 = parser.AddArgumentGroup("xxx");
            parser.AddArgument("foo");
            group1.AddArgument("bar");
            parser.AddArgument("baz");
            var group2 = parser.AddArgumentGroup("yyy");
            group2.AddArgument("freil");
            Asserter.AreEqual(new ParseResult {{"foo", "1"}, {"bar", "2"}, {"baz", "3"}, {"freil", "4"}},
                parser.ParseArguments("1 2 3 4".Split(new char[] {})));
        }

        [Test] public void MutuallyExclusiveGroupErrorsInvalidAddArgument()
        {
            var parser = new Parser();
            var group = parser.AddMutuallyExclusiveGroup();
            Assert.Throws<ParserException>(()=>group.AddArgument(new Argument("--foo") {IsRequired = true}));
            Assert.Throws<ParserException>(() => group.AddArgument("bar"));
            Assert.Throws<ParserException>(() => group.AddArgument(new Argument("bar"){ValueCount = new ValueCount("+")}));
            Assert.Throws<ParserException>(() => group.AddArgument(new Argument("bar") { ValueCount = new ValueCount("+") }));
            Assert.Throws<ParserException>(() => group.AddArgument(new Argument("bar") { ValueCount = new ValueCount(1) }));
        }
    }
}