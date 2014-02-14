using System;

namespace Cr.ArgParse.Tests.TestCases
{
    public class TestTypeUserDefined : ParserTestCase
    {
        public class MyType : IEquatable<MyType>
        {
            public override int GetHashCode()
            {
                return (Value != null ? Value.GetHashCode() : 0);
            }

            public static bool operator ==(MyType left, MyType right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(MyType left, MyType right)
            {
                return !Equals(left, right);
            }

            public MyType(string value)
            {
                Value = value;
            }

            public string Value { get; private set; }

            public bool Equals(MyType other)
            {
                return other != null && Value == other.Value;
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as MyType);
            }
        }

        public TestTypeUserDefined()
        {
            ArgumentSignatures = new[]
            {
                new Argument("-x") {TypeFactory = arg => new MyType(arg)},
                new Argument("spam") {TypeFactory = arg => new MyType(arg)}
            };

            Successes = new SuccessCollection
            {
                {"a -x b", new ParseResult {{"x", new MyType("b")}, {"spam", new MyType("a")}}}
            };
        }
    }
}