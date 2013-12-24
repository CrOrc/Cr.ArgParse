using System;
using System.Collections.Generic;
using Cr.ArgParse.Tests.Assertions;
using NUnit.Framework;

namespace Cr.ArgParse.Tests
{
    [TestFixture] public class ParseResultTest
    {
        [Test] public void AddResults()
        {
            var parseResult = new ParseResult();
            parseResult["res1"] = 1;
            parseResult["res2"] = 2;
            parseResult["res3"] = new[]{"a","b","c"};
            parseResult["res4"] = "4";
            Assert.That(parseResult.ToDictionary(), new BaseSmartEqualityConstraint(new Dictionary<string,object>(StringComparer.InvariantCultureIgnoreCase)
            {
                {"res1",1},
                {"res2",2},
                {"res3",new[]{"a","b","c", "d"}},
                {"res4","4"}
            }));
        }
    }
}