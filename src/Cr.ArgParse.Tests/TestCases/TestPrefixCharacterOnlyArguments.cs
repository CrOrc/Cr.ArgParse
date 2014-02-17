using System;
using System.Collections.Generic;

namespace Cr.ArgParse.Tests
{
    public class TestPrefixCharacterOnlyArguments : ParserTestCase
    {
        public TestPrefixCharacterOnlyArguments()
        {
            ParserSignature = new Tuple<string, IList<string>, string>("",new []{"-","+"},"resolve");
            ArgumentSignatures = new[]
            {
                new Argument("-") {ConstValue = "badger", Destination = "x", ValueCount = new ValueCount("?")},
                new Argument("+") {Destination = "y", DefaultValue = 42, Type = typeof(int)},
                new Argument("-+-") {ActionName = "store_true", Destination = "z"}
            };
            Failures = new[] {"-y", "+ -"};
            Successes = new SuccessCollection
            {
                {"", new ParseResult {{"x", null}, {"y", 42}, {"z", false}}},
                {"-", new ParseResult {{"x", "badger"}, {"y", 42}, {"z", false}}},
                {"- X", new ParseResult {{"x", "X"}, {"y", 42}, {"z", false}}},
                {"+ -3", new ParseResult {{"x", null}, {"y", -3}, {"z", false}}},
                {"-+-", new ParseResult {{"x", null}, {"y", 42}, {"z", true}}},
                {"- ===", new ParseResult {{"x", "==="}, {"y", 42}, {"z", false}}}
            };
        }
    }
}