using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Cr.ArgParse.Tests
{
    public class ParserTestCase : IEnumerable<TestCaseData>
    {
        public IList<Argument> ArgumentSignatures { get; set; }
        public IList<string> Failures { get; set; }
        public IList<Tuple<string, ParseResult>> Successes { get; set; }
        public Tuple<string, IList<string>, string> ParserSignature { get; set; }

        public Type DefaultExceptionType { get; set; }

        public ParserTestCase()
        {
            DefaultExceptionType = typeof (ParserException);
        }

        private class PartialTestCase
        {
            private string ParserTestCaseName { get; set; }

            private ParserTestCase ParserTestCase { get; set; }
            private Action<Parser> AddArguments { get; set; }

            public PartialTestCase(ParserTestCase parserTestCase, Action<Parser> addArguments)
            {
                ParserTestCase = parserTestCase;
                ParserTestCaseName = ParserTestCase.GetType().Name;
                AddArguments = addArguments;
                AddArgumentsName = addArguments.Method.Name;
            }

            private string FormatTestCaseName(string argsStr, string format = "{0}")
            {
                return string.Format("{0}_{1}_{2}", ParserTestCaseName, AddArgumentsName,
                    string.Format(format, string.Format("args: {0}", argsStr)));
            }

            private string AddArgumentsName { get; set; }

            private IList<Tuple<string, ParseResult>> Successes
            {
                get { return ParserTestCase.Successes; }
            }

            private IList<string> Failures
            {
                get { return ParserTestCase.Failures; }
            }

            private Type DefaultExceptionType
            {
                get { return ParserTestCase.DefaultExceptionType; }
            }

            private Tuple<string, IList<string>, string> ParserSignature
            {
                get { return ParserTestCase.ParserSignature; }
            }

            private Parser CreateParser()
            {
                var parser = ParserSignature != null
                    ? new Parser(ParserSignature.Item1, ParserSignature.Item2, ParserSignature.Item3)
                    : new Parser();
                AddArguments(parser);
                return parser;
            }

            public IEnumerable<TestCaseData> TestCases
            {
                get
                {
                    var cachedParsers = new Parser[1];
                    Func<Parser> parserFactory =
                        () => cachedParsers[0] ?? (cachedParsers[0] = CreateParser());

                    return (Successes ?? new Tuple<string, ParseResult>[] {}).Select(
                        success =>
                            new TestCaseData(parserFactory,
                                success.Item1.Split(new char[] {}, StringSplitOptions.RemoveEmptyEntries), success.Item2,
                                null)
                                .SetName(FormatTestCaseName(success.Item1, "Success({0})")))
                        .Concat(
                            (Failures ?? new string[] {}).Select(
                                argsStr =>
                                    new TestCaseData(parserFactory,
                                        argsStr.Split(new char[] {}, StringSplitOptions.RemoveEmptyEntries), null,
                                        DefaultExceptionType)
                                        .SetName(FormatTestCaseName(argsStr, "Fail({0})"))));
                }
            }
        }

        private void NoGroups(Parser parser)
        {
            if (ArgumentSignatures == null) return;
            foreach (var argumentSignature in ArgumentSignatures)
                parser.AddArgument(argumentSignature);
        }

        private void OneGroup(Parser parser)
        {
            if (ArgumentSignatures == null) return;
            var group = parser.AddArgumentGroup("foo");
            foreach (var argumentSignature in ArgumentSignatures)
                group.AddArgument(argumentSignature);
        }

        private void ManyGroups(Parser parser)
        {
            if (ArgumentSignatures == null) return;
            foreach (var it in ArgumentSignatures.Select((it, i) => new {argumentSignature = it, i}))
            {
                var group = parser.AddArgumentGroup("foo" + it.i);
                group.AddArgument(it.argumentSignature);
            }
        }

        private IEnumerable<TestCaseData> TestCases
        {
            get
            {
                return new Action<Parser>[] {NoGroups, OneGroup, ManyGroups}.SelectMany(
                    addAction => new PartialTestCase(this, addAction).TestCases);
            }
        }

        public IEnumerator<TestCaseData> GetEnumerator()
        {
            return TestCases.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}