using Cr.ArgParse.Tests.Assertions;
using NUnit.Framework;

namespace Cr.ArgParse.Tests
{
    public class Asserter
    {
        public void AreEqual(object expected, object actual)
        {
            Assert.That(actual, new SmartConstraint(expected));
        }

        public void AreEqual(ArgumentAction action1, ArgumentAction action2)
        {
            Assert.That(action2, new ArgumentActionConstraint(action1));
        }

        private class ArgumentActionConstraint : BaseSmartEqualityConstraint
        {
            public ArgumentActionConstraint(ArgumentAction expected) : base(expected)
            {
            }
        }

        private class SmartConstraint : BaseSmartEqualityConstraint
        {
            public SmartConstraint(object expected)
                : base(expected)
            {
            }

            public void AreEqual(ParseResult expected, ParseResult actual)
            {
                AreEqual(expected.ToDictionary(), expected.ToDictionary());
                CollectionAssert.AreEqual(expected.UnrecognizedArguments, actual.UnrecognizedArguments,
                    "UnrecognizedArguments should be the same");
            }
        }
    }
}