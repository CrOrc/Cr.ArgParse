using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Cr.ArgParse.Tests
{
    public class ParserTestCase
    {
        private readonly Lazy<string> typeNameLazy;
        public IList<Argument> ArgumentSignatures { get; set; }
        public IList<string> Failures { get; set; }
        public IList<Tuple<string, ParseResult>> Successes { get; set; }

        public Type DefaultExceptionType { get; set; }

        public ParserTestCase()
        {
            typeNameLazy = new Lazy<string>(() => GetType().Name);
            DefaultExceptionType = typeof (ParserException);
        }

        private Parser CreateParser()
        {
            var parser = new Parser();
            if (ArgumentSignatures == null) return parser;
            foreach (var argumentSignature in ArgumentSignatures)
                parser.AddArgument(argumentSignature);
            return parser;
        }

        private string TypeName
        {
            get { return typeNameLazy.Value; }
        }

        private string FormatTestCaseName(string argsStr, string format = "{0}")
        {
            return string.Format("{0}_{1}", TypeName, string.Format(format, string.Format("args: {0}", argsStr)));
        }

        public IEnumerable<TestCaseData> TestCases
        {
            get
            {
                var parser = CreateParser();

                return (Successes ?? new Tuple<string, ParseResult>[] {}).Select(
                    success =>
                        new TestCaseData(parser,
                            success.Item1.Split(new char[] {}, StringSplitOptions.RemoveEmptyEntries), success.Item2,
                            null)
                            .SetName(FormatTestCaseName(success.Item1, "Success({0})")))
                    .Concat(
                        (Failures ?? new string[] {}).Select(
                            argsStr =>
                                new TestCaseData(parser,
                                    argsStr.Split(new char[] {}, StringSplitOptions.RemoveEmptyEntries), null,
                                    DefaultExceptionType)
                                    .SetName(FormatTestCaseName(argsStr, "Fail({0})"))));
            }
        }
    }
}