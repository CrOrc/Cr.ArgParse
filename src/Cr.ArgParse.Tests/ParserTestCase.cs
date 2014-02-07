using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Cr.ArgParse.Tests
{
    public class ParserTestCase
    {
        public IList<Argument> ArgumentSignatures { get; set; }
        public IList<string> Failures { get; set; }
        public IList<Tuple<string, ParseResult>> Successes { get; set; }

        private Parser CreateParser()
        {
            var parser = new Parser();
            if (ArgumentSignatures == null) return parser;
            foreach (var argumentSignature in ArgumentSignatures)
                parser.AddArgument(argumentSignature);
            return parser;
        }

        private string FormatTestCaseName(string argsStr, string format = "{0}")
        {
            return string.Format(format, string.Format("args: {0}", argsStr));
        }

        public IEnumerable SuccessCases
        {
            get
            {
                var parser = CreateParser();
                return
                    Successes.Select(
                        success =>
                            new TestCaseData(parser,
                                success.Item1.Split(new char[] {}, StringSplitOptions.RemoveEmptyEntries), success.Item2)
                                .SetName(FormatTestCaseName(success.Item1, "Success({0})")));
            }
        }

        public IEnumerable FailureCases
        {
            get
            {
                var parser = CreateParser();
                return
                    Failures.Select(
                        argsStr =>
                            new TestCaseData(parser, argsStr.Split(new char[] {}, StringSplitOptions.RemoveEmptyEntries),
                                null).SetName(FormatTestCaseName(argsStr, "Fail({0})"))
                                .Throws(typeof (UnrecognizedArgumentsException)));
            }
        }
    }
}