using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace Cr.ArgParse.Tests
{
    public class SimpleParserTestCases : IEnumerable<TestCaseData>
    {
        private IEnumerable<TestCaseData> TestCases
        {
            get
            {
                return
                    Assembly.GetAssembly(typeof (ParserTestCase))
                        .GetTypes()
                        .Where(it => typeof (ParserTestCase).IsAssignableFrom(it) && it != typeof (ParserTestCase) && !it.GetCustomAttributes(typeof(IgnoreCaseAttribute),true).Any())
                        .Select(it => Activator.CreateInstance(it) as IEnumerable<TestCaseData>)
                        .Where(it => it != null)
                        .SelectMany(it => it);
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