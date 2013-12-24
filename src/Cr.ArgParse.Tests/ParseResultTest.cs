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
            var parseResult = new ParseResult
            {
                {"res1", 1},
                {"res2", 2},
                {"res3", new[] {"a", "b", "c"}},
                {"res4", "4"}
            };
            Assert.That(parseResult.ToDictionary(),
                new BaseSmartEqualityConstraint(new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase)
                {
                    {"res1", 1},
                    {"res2", 2},
                    {"res3", new[] {"a", "b", "c"}},
                    {"res4", "4"}
                }));
        }

        [Test] public void AddResultsNested()
        {
            var parseResult = new ParseResult
            {
                {"res1", 1},
                {"res2", 2},
                {"res3", new[] {"a", "b", "c"}},
                {"res4", "4"},
                {
                    "res5",
                    new ParseResult
                    {
                        {"res1", 5},
                        {"res2", 6},
                        {"res3", "7"}
                    }
                }
            };
            Assert.That(parseResult.ToDictionary(),
                new BaseSmartEqualityConstraint(new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase)
                {
                    {"res1", 1},
                    {"res2", 2},
                    {"res3", new[] {"a", "b", "c"}},
                    {"res4", "4"},
                    {
                        "res5",
                        new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase)
                        {
                            {"res1", 5},
                            {"res2", 6},
                            {"res3", "7"}
                        }
                    }
                }));
        }
    }
}