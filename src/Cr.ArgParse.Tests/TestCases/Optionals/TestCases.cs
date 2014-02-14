using System;

namespace Cr.ArgParse.Tests.TestCases.Optionals
{
    public class TestOptionalsActionAppend : ParserTestCase
    {
        public TestOptionalsActionAppend()
        {
            ArgumentSignatures = new[] {new Argument("--baz") {ActionName = "append"}};
            Failures = new[] {"a", "--baz", "a --baz", "--baz a b"};
            Successes = new SuccessCollection
            {
                {"", new ParseResult {{"baz", null}}},
                {"--baz a", new ParseResult {{"baz", new[] {"a"}}}},
                {"--baz a --baz b", new ParseResult {{"baz", new[] {"a", "b"}}}}
            };
        }
    }

    public class TestOptionalsActionAppendConst : ParserTestCase
    {
        public TestOptionalsActionAppendConst()
        {
            ArgumentSignatures = new[]
            {
                new Argument("-b") {ActionName = "append_const", ConstValue = typeof (Exception)},
                new Argument("-c") {ActionName = "append", Destination = "b"}
            };
            Failures = new[] {"a", "-c", "a -c", "-bx", "-b x"};
            Successes = new SuccessCollection
            {
                {"", new ParseResult {{"b", null}}},
                {"-b", new ParseResult {{"b", new[] {typeof (Exception)}}}},
                {
                    "-b -cx -b -cyz",
                    new ParseResult {{"b", new object[] {typeof (Exception), "x", typeof (Exception), "yz"}}}
                }
            };
        }
    }

    public class TestOptionalsActionAppendConstWithDefault : ParserTestCase
    {
        public TestOptionalsActionAppendConstWithDefault()
        {
            ArgumentSignatures = new[]
            {
                new Argument("-b")
                {
                    ActionName = "append_const",
                    ConstValue = typeof (Exception),
                    DefaultValue = new[] {"X"}
                },
                new Argument("-c") {ActionName = "append", Destination = "b"}
            };
            Failures = new[] {"a", "-c", "a -c", "-bx", "-b x"};
            Successes = new SuccessCollection
            {
                {"", new ParseResult {{"b", new[] {"X"}}}},
                {"-b", new ParseResult {{"b", new object[] {"X", typeof (Exception)}}}},
                {
                    "-b -cx -b -cyz",
                    new ParseResult {{"b", new object[] {"X", typeof (Exception), "x", typeof (Exception), "yz"}}}
                }
            };
        }
    }

    public class TestOptionalsActionAppendWithDefault : ParserTestCase
    {
        public TestOptionalsActionAppendWithDefault()
        {
            ArgumentSignatures = new[] {new Argument("--baz") {ActionName = "append", DefaultValue = new[] {"X"}}};
            Failures = new[] {"a", "--baz", "a --baz", "--baz a b"};
            Successes = new SuccessCollection
            {
                {"", new ParseResult {{"baz", new[] {"X"}}}},
                {"--baz a", new ParseResult {{"baz", new[] {"X", "a"}}}},
                {"--baz a --baz b", new ParseResult {{"baz", new[] {"X", "a", "b"}}}}
            };
        }
    }

    public class TestOptionalsActionCount : ParserTestCase
    {
        public TestOptionalsActionCount()
        {
            ArgumentSignatures = new[] {new Argument("-x") {ActionName = "count"}};
            Failures = new[] {"a", "-x a", "-x b", "-x a -x b"};
            Successes = new SuccessCollection
            {
                {"", new ParseResult {{"x", null}}},
                {"-x", new ParseResult {{"x", 1}}}
            };
        }
    }

    public class TestOptionalsActionStore : ParserTestCase
    {
        public TestOptionalsActionStore()
        {
            ArgumentSignatures = new[] {new Argument("-x") {ActionName = "store"}};
            Failures = new[] {"a", "a -x"};
            Successes = new SuccessCollection
            {
                {"", new ParseResult {{"x", null}}},
                {"-xfoo", new ParseResult {{"x", "foo"}}}
            };
        }
    }

    public class TestOptionalsActionStoreConst : ParserTestCase
    {
        public TestOptionalsActionStoreConst()
        {
            ArgumentSignatures = new[] {new Argument("-y") {ActionName = "store_const", ConstValue = typeof (object)}};
            Failures = new[] {"a"};
            Successes = new SuccessCollection
            {
                {"", new ParseResult {{"y", null}}},
                {"-y", new ParseResult {{"y", typeof (object)}}}
            };
        }
    }

    public class TestOptionalsActionStoreFalse : ParserTestCase
    {
        public TestOptionalsActionStoreFalse()
        {
            ArgumentSignatures = new[] {new Argument("-z") {ActionName = "store_false"}};
            Failures = new[] {"a", "-za", "-z a"};
            Successes = new SuccessCollection
            {
                {"", new ParseResult {{"z", true}}},
                {"-z", new ParseResult {{"z", false}}}
            };
        }
    }

    public class TestOptionalsActionStoreTrue : ParserTestCase
    {
        public TestOptionalsActionStoreTrue()
        {
            ArgumentSignatures = new[] {new Argument("--apple") {ActionName = "store_true"}};
            Failures = new[] {"a", "--apple=b", "--apple b"};
            Successes = new SuccessCollection
            {
                {"", new ParseResult {{"apple", false}}},
                {"--apple", new ParseResult {{"apple", true}}}
            };
        }
    }

    public class TestOptionalsAlmostNumericAndPositionals : ParserTestCase
    {
        public TestOptionalsAlmostNumericAndPositionals()
        {
            ArgumentSignatures = new[]
            {
                new Argument("x") {ValueCount = new ValueCount("?")},
                new Argument("-k4") {ActionName = "store_true", Destination = "y"}
            };
            Failures = new[] {"-k3"};
            Successes = new SuccessCollection
            {
                {"", new ParseResult {{"x", null}, {"y", false}}},
                {"-2", new ParseResult {{"x", "-2"}, {"y", false}}},
                {"a", new ParseResult {{"x", "a"}, {"y", false}}},
                {"-k4", new ParseResult {{"x", null}, {"y", true}}},
                {"-k4 a", new ParseResult {{"x", "a"}, {"y", true}}}
            };
        }
    }

    public class TestOptionalsDefault : ParserTestCase
    {
        public TestOptionalsDefault()
        {
            ArgumentSignatures = new[] {new Argument("-x"), new Argument("-y") {DefaultValue = 42}};
            Failures = new[] {"a"};
            Successes = new SuccessCollection
            {
                {"", new ParseResult {{"x", null}, {"y", 42}}},
                {"-xx", new ParseResult {{"x", "x"}, {"y", 42}}},
                {"-yy", new ParseResult {{"x", null}, {"y", "y"}}}
            };
        }
    }

    public class TestOptionalsDest : ParserTestCase
    {
        public TestOptionalsDest()
        {
            ArgumentSignatures = new[] {new Argument("--foo-bar"), new Argument("--baz") {Destination = "zabbaz"}};
            Failures = new[] {"a"};
            Successes = new SuccessCollection
            {
                {"--foo-bar f", new ParseResult {{"foo_bar", "f"}, {"zabbaz", null}}},
                {"--baz g", new ParseResult {{"foo_bar", null}, {"zabbaz", "g"}}},
                {"--foo-bar h --baz i", new ParseResult {{"foo_bar", "h"}, {"zabbaz", "i"}}},
                {"--baz j --foo-bar k", new ParseResult {{"foo_bar", "k"}, {"zabbaz", "j"}}}
            };
        }
    }

    public class TestOptionalsNargs1 : ParserTestCase
    {
        public TestOptionalsNargs1()
        {
            ArgumentSignatures = new[] {new Argument("-x") {ValueCount = new ValueCount(1)}};
            Failures = new[] {"a", "-x"};
            Successes = new SuccessCollection
            {
                {"", new ParseResult {{"x", null}}},
                {"-x a", new ParseResult {{"x", new[] {"a"}}}}
            };
        }
    }

    public class TestOptionalsNargs3 : ParserTestCase
    {
        public TestOptionalsNargs3()
        {
            ArgumentSignatures = new[] {new Argument("-x") {ValueCount = new ValueCount(3)}};
            Failures = new[] {"a", "-x", "-x a", "-x a b", "a -x", "a -x b"};
            Successes = new SuccessCollection
            {
                {"", new ParseResult {{"x", null}}},
                {"-x a b c", new ParseResult {{"x", new[] {"a", "b", "c"}}}}
            };
        }
    }

    public class TestOptionalsNargsDefault : ParserTestCase
    {
        public TestOptionalsNargsDefault()
        {
            ArgumentSignatures = new[] {new Argument("-x")};
            Failures = new[] {"a", "-x"};
            Successes = new SuccessCollection
            {
                {"", new ParseResult {{"x", null}}},
                {"-x a", new ParseResult {{"x", "a"}}}
            };
        }
    }

    public class TestOptionalsNargsOneOrMore : ParserTestCase
    {
        public TestOptionalsNargsOneOrMore()
        {
            ArgumentSignatures = new[]
            {
                new Argument("-x") {ValueCount = new ValueCount("+")},
                new Argument("-y") {DefaultValue = "spam", ValueCount = new ValueCount("+")}
            };
            Failures = new[] {"a", "-x", "-y", "a -x", "a -y b"};
            Successes = new SuccessCollection
            {
                {"", new ParseResult {{"x", null}, {"y", "spam"}}},
                {"-x a", new ParseResult {{"x", new[] {"a"}}, {"y", "spam"}}},
                {"-x a b", new ParseResult {{"x", new[] {"a", "b"}}, {"y", "spam"}}},
                {"-y a", new ParseResult {{"x", null}, {"y", new[] {"a"}}}},
                {"-y a b", new ParseResult {{"x", null}, {"y", new[] {"a", "b"}}}}
            };
        }
    }

    public class TestOptionalsNargsOptional : ParserTestCase
    {
        public TestOptionalsNargsOptional()
        {
            ArgumentSignatures = new[]
            {
                new Argument("-w") {ValueCount = new ValueCount("?")},
                new Argument("-x") {ConstValue = 42, ValueCount = new ValueCount("?")},
                new Argument("-y") {DefaultValue = "spam", ValueCount = new ValueCount("?")},
                new Argument("-z")
                {
                    ConstValue = "42",
                    DefaultValue = "84",
                    ValueCount = new ValueCount("?"),
                    TypeName = "int"
                }
            };
            Failures = new[] {"2"};
            Successes = new SuccessCollection
            {
                {"", new ParseResult {{"w", null}, {"x", null}, {"y", "spam"}, {"z", 84}}},
                {"-w", new ParseResult {{"w", null}, {"x", null}, {"y", "spam"}, {"z", 84}}},
                {"-w 2", new ParseResult {{"w", "2"}, {"x", null}, {"y", "spam"}, {"z", 84}}},
                {"-x", new ParseResult {{"w", null}, {"x", 42}, {"y", "spam"}, {"z", 84}}},
                {"-x 2", new ParseResult {{"w", null}, {"x", "2"}, {"y", "spam"}, {"z", 84}}},
                {"-y", new ParseResult {{"w", null}, {"x", null}, {"y", null}, {"z", 84}}},
                {"-y 2", new ParseResult {{"w", null}, {"x", null}, {"y", "2"}, {"z", 84}}},
                {"-z", new ParseResult {{"w", null}, {"x", null}, {"y", "spam"}, {"z", 42}}},
                {"-z 2", new ParseResult {{"w", null}, {"x", null}, {"y", "spam"}, {"z", 2}}}
            };
        }
    }

    public class TestOptionalsNargsZeroOrMore : ParserTestCase
    {
        public TestOptionalsNargsZeroOrMore()
        {
            ArgumentSignatures = new[]
            {
                new Argument("-x") {ValueCount = new ValueCount("*")},
                new Argument("-y") {DefaultValue = "spam", ValueCount = new ValueCount("*")}
            };
            Failures = new[] {"a"};
            Successes = new SuccessCollection
            {
                {"", new ParseResult {{"x", null}, {"y", "spam"}}},
                {"-x", new ParseResult {{"x", new object[] {}}, {"y", "spam"}}},
                {"-x a", new ParseResult {{"x", new[] {"a"}}, {"y", "spam"}}},
                {"-x a b", new ParseResult {{"x", new[] {"a", "b"}}, {"y", "spam"}}},
                {"-y", new ParseResult {{"x", null}, {"y", new object[] {}}}},
                {"-y a", new ParseResult {{"x", null}, {"y", new[] {"a"}}}},
                {"-y a b", new ParseResult {{"x", null}, {"y", new[] {"a", "b"}}}}
            };
        }
    }

    public class TestOptionalsNumericAndPositionals : ParserTestCase
    {
        public TestOptionalsNumericAndPositionals()
        {
            ArgumentSignatures = new[]
            {
                new Argument("x") {ValueCount = new ValueCount("?")},
                new Argument("-4") {ActionName = "store_true", Destination = "y"}
            };
            Failures = new[] {"-2", "-315"};
            Successes = new SuccessCollection
            {
                {"", new ParseResult {{"x", null}, {"y", false}}},
                {"a", new ParseResult {{"x", "a"}, {"y", false}}},
                {"-4", new ParseResult {{"x", null}, {"y", true}}},
                {"-4 a", new ParseResult {{"x", "a"}, {"y", true}}}
            };
        }
    }

    public class TestOptionalsShortLong : ParserTestCase
    {
        public TestOptionalsShortLong()
        {
            ArgumentSignatures = new[] {new Argument("-v", "--verbose", "-n", "--noisy") {ActionName = "store_true"}};
            Failures = new[] {"--x --verbose", "-N", "a", "-v x"};
            Successes = new SuccessCollection
            {
                {"", new ParseResult {{"verbose", false}}},
                {"-v", new ParseResult {{"verbose", true}}},
                {"--verbose", new ParseResult {{"verbose", true}}},
                {"-n", new ParseResult {{"verbose", true}}},
                {"--noisy", new ParseResult {{"verbose", true}}}
            };
        }
    }
}