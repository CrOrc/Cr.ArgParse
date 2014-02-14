using System;
using System.Collections.Generic;

namespace Cr.ArgParse.Tests.TestCases
{
    public class TestOptionalsAlternatePrefixCharsMultipleShortArgs : ParserTestCase
    {
        public TestOptionalsAlternatePrefixCharsMultipleShortArgs()
        {
            ParserSignature = new Tuple<string, IList<string>, string>("", new[] { "+", "-"}, "resolve");
            ArgumentSignatures = new[]
            {
                new Argument("-x") {ActionName = "store_true"}, new Argument("+y") {ActionName = "store_true"},
                new Argument("+z") {ActionName = "store_true"}
            };
            Failures = new[] {"-w", "-xyz", "+x", "-y", "+xyz"};
            Successes = new SuccessCollection
            {
                {"", new ParseResult {{"x", false}, {"y", false}, {"z", false}}},
                {"-x", new ParseResult {{"x", true}, {"y", false}, {"z", false}}},
                {"+y -x", new ParseResult {{"x", true}, {"y", true}, {"z", false}}},
                {"+yz -x", new ParseResult {{"x", true}, {"y", true}, {"z", true}}}
            };
        }
    }
}