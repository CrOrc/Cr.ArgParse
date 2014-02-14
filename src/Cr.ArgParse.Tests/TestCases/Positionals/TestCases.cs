namespace Cr.ArgParse.Tests.TestCases.Positionals
{
    public class TestPositionalsActionAppend : ParserTestCase
    {
        public TestPositionalsActionAppend()
        {
            ArgumentSignatures = new[]
            {
                new Argument("spam") {ActionName = "append"},
                new Argument("spam") {ActionName = "append", ValueCount = new ValueCount(2)}
            };
            Failures = new[] {"", "--foo", "a", "a b", "a b c d"};
            Successes = new SuccessCollection
            {
                {"a b c", new ParseResult {{"spam", new object[] {"a", new[] {"b", "c"}}}}}
            };
        }
    }

    public class TestPositionalsNargs1 : ParserTestCase
    {
        public TestPositionalsNargs1()
        {
            ArgumentSignatures = new[] {new Argument("foo") {ValueCount = new ValueCount(1)}};
            Failures = new[] {"", "-x", "a b"};
            Successes = new SuccessCollection {{"a", new ParseResult {{"foo", new[] {"a"}}}}};
        }
    }

    public class TestPositionalsNargs2 : ParserTestCase
    {
        public TestPositionalsNargs2()
        {
            ArgumentSignatures = new[] {new Argument("foo") {ValueCount = new ValueCount(2)}};
            Failures = new[] {"", "a", "-x", "a b c"};
            Successes = new SuccessCollection {{"a b", new ParseResult {{"foo", new[] {"a", "b"}}}}};
        }
    }

    public class TestPositionalsNargs2None : ParserTestCase
    {
        public TestPositionalsNargs2None()
        {
            ArgumentSignatures = new[] {new Argument("foo") {ValueCount = new ValueCount(2)}, new Argument("bar")};
            Failures = new[] {"", "--foo", "a", "a b", "a b c d"};
            Successes = new SuccessCollection {{"a b c", new ParseResult {{"bar", "c"}, {"foo", new[] {"a", "b"}}}}};
        }
    }

    public class TestPositionalsNargs2OneOrMore : ParserTestCase
    {
        public TestPositionalsNargs2OneOrMore()
        {
            ArgumentSignatures = new[]
            {
                new Argument("foo") {ValueCount = new ValueCount(2)},
                new Argument("bar") {ValueCount = new ValueCount("+")}
            };
            Failures = new[] {"", "--foo", "a", "a b"};
            Successes = new SuccessCollection
            {
                {"a b c", new ParseResult {{"bar", new[] {"c"}}, {"foo", new[] {"a", "b"}}}}
            };
        }
    }

    public class TestPositionalsNargs2Optional : ParserTestCase
    {
        public TestPositionalsNargs2Optional()
        {
            ArgumentSignatures = new[]
            {
                new Argument("foo") {ValueCount = new ValueCount(2)},
                new Argument("bar") {ValueCount = new ValueCount("?")}
            };
            Failures = new[] {"", "--foo", "a", "a b c d"};
            Successes = new SuccessCollection
            {
                {"a b", new ParseResult {{"bar", null}, {"foo", new[] {"a", "b"}}}},
                {"a b c", new ParseResult {{"bar", "c"}, {"foo", new[] {"a", "b"}}}}
            };
        }
    }

    public class TestPositionalsNargs2ZeroOrMore : ParserTestCase
    {
        public TestPositionalsNargs2ZeroOrMore()
        {
            ArgumentSignatures = new[]
            {
                new Argument("foo") {ValueCount = new ValueCount(2)},
                new Argument("bar") {ValueCount = new ValueCount("*")}
            };
            Failures = new[] {"", "--foo", "a"};
            Successes = new SuccessCollection
            {
                {"a b", new ParseResult {{"bar", new object[] {}}, {"foo", new[] {"a", "b"}}}},
                {"a b c", new ParseResult {{"bar", new[] {"c"}}, {"foo", new[] {"a", "b"}}}}
            };
        }
    }

    public class TestPositionalsNargsNone : ParserTestCase
    {
        public TestPositionalsNargsNone()
        {
            ArgumentSignatures = new[] {new Argument("foo")};
            Failures = new[] {"", "-x", "a b"};
            Successes = new SuccessCollection {{"a", new ParseResult {{"foo", "a"}}}};
        }
    }

    public class TestPositionalsNargsNone1 : ParserTestCase
    {
        public TestPositionalsNargsNone1()
        {
            ArgumentSignatures = new[] {new Argument("foo"), new Argument("bar") {ValueCount = new ValueCount(1)}};
            Failures = new[] {"", "--foo", "a", "a b c"};
            Successes = new SuccessCollection {{"a b", new ParseResult {{"bar", new[] {"b"}}, {"foo", "a"}}}};
        }
    }

    public class TestPositionalsNargsNoneNone : ParserTestCase
    {
        public TestPositionalsNargsNoneNone()
        {
            ArgumentSignatures = new[] {new Argument("foo"), new Argument("bar")};
            Failures = new[] {"", "-x", "a", "a b c"};
            Successes = new SuccessCollection {{"a b", new ParseResult {{"bar", "b"}, {"foo", "a"}}}};
        }
    }

    public class TestPositionalsNargsNoneOneOrMore : ParserTestCase
    {
        public TestPositionalsNargsNoneOneOrMore()
        {
            ArgumentSignatures = new[] {new Argument("foo"), new Argument("bar") {ValueCount = new ValueCount("+")}};
            Failures = new[] {"", "--foo", "a"};
            Successes = new SuccessCollection
            {
                {"a b", new ParseResult {{"bar", new[] {"b"}}, {"foo", "a"}}},
                {"a b c", new ParseResult {{"bar", new[] {"b", "c"}}, {"foo", "a"}}}
            };
        }
    }

    public class TestPositionalsNargsNoneOneOrMore1 : ParserTestCase
    {
        public TestPositionalsNargsNoneOneOrMore1()
        {
            ArgumentSignatures = new[]
            {
                new Argument("foo"), new Argument("bar") {ValueCount = new ValueCount("+")},
                new Argument("baz") {ValueCount = new ValueCount(1)}
            };
            Failures = new[] {"", "--foo", "a", "b"};
            Successes = new SuccessCollection
            {
                {"a b c", new ParseResult {{"bar", new[] {"b"}}, {"baz", new[] {"c"}}, {"foo", "a"}}},
                {"a b c d", new ParseResult {{"bar", new[] {"b", "c"}}, {"baz", new[] {"d"}}, {"foo", "a"}}}
            };
        }
    }

    public class TestPositionalsNargsNoneOptional : ParserTestCase
    {
        public TestPositionalsNargsNoneOptional()
        {
            ArgumentSignatures = new[] {new Argument("foo"), new Argument("bar") {ValueCount = new ValueCount("?")}};
            Failures = new[] {"", "--foo", "a b c"};
            Successes = new SuccessCollection
            {
                {"a", new ParseResult {{"bar", null}, {"foo", "a"}}},
                {"a b", new ParseResult {{"bar", "b"}, {"foo", "a"}}}
            };
        }
    }

    public class TestPositionalsNargsNoneOptional1 : ParserTestCase
    {
        public TestPositionalsNargsNoneOptional1()
        {
            ArgumentSignatures = new[]
            {
                new Argument("foo"), new Argument("bar") {DefaultValue = 0.625, ValueCount = new ValueCount("?")},
                new Argument("baz") {ValueCount = new ValueCount(1)}
            };
            Failures = new[] {"", "--foo", "a"};
            Successes = new SuccessCollection
            {
                {"a b", new ParseResult {{"bar", 0.625}, {"baz", new[] {"b"}}, {"foo", "a"}}},
                {"a b c", new ParseResult {{"bar", "b"}, {"baz", new[] {"c"}}, {"foo", "a"}}}
            };
        }
    }

    public class TestPositionalsNargsNoneZeroOrMore : ParserTestCase
    {
        public TestPositionalsNargsNoneZeroOrMore()
        {
            ArgumentSignatures = new[] {new Argument("foo"), new Argument("bar") {ValueCount = new ValueCount("*")}};
            Failures = new[] {"", "--foo"};
            Successes = new SuccessCollection
            {
                {"a", new ParseResult {{"bar", new object[] {}}, {"foo", "a"}}},
                {"a b", new ParseResult {{"bar", new[] {"b"}}, {"foo", "a"}}},
                {"a b c", new ParseResult {{"bar", new[] {"b", "c"}}, {"foo", "a"}}}
            };
        }
    }

    public class TestPositionalsNargsNoneZeroOrMore1 : ParserTestCase
    {
        public TestPositionalsNargsNoneZeroOrMore1()
        {
            ArgumentSignatures = new[]
            {
                new Argument("foo"), new Argument("bar") {ValueCount = new ValueCount("*")},
                new Argument("baz") {ValueCount = new ValueCount(1)}
            };
            Failures = new[] {"", "--foo", "a"};
            Successes = new SuccessCollection
            {
                {"a b", new ParseResult {{"bar", new object[] {}}, {"baz", new[] {"b"}}, {"foo", "a"}}},
                {"a b c", new ParseResult {{"bar", new[] {"b"}}, {"baz", new[] {"c"}}, {"foo", "a"}}}
            };
        }
    }

    public class TestPositionalsNargsOneOrMore : ParserTestCase
    {
        public TestPositionalsNargsOneOrMore()
        {
            ArgumentSignatures = new[] {new Argument("foo") {ValueCount = new ValueCount("+")}};
            Failures = new[] {"", "-x"};
            Successes = new SuccessCollection
            {
                {"a", new ParseResult {{"foo", new[] {"a"}}}},
                {"a b", new ParseResult {{"foo", new[] {"a", "b"}}}}
            };
        }
    }

    public class TestPositionalsNargsOneOrMore1 : ParserTestCase
    {
        public TestPositionalsNargsOneOrMore1()
        {
            ArgumentSignatures = new[]
            {
                new Argument("foo") {ValueCount = new ValueCount("+")},
                new Argument("bar") {ValueCount = new ValueCount(1)}
            };
            Failures = new[] {"", "--foo", "a"};
            Successes = new SuccessCollection
            {
                {"a b", new ParseResult {{"bar", new[] {"b"}}, {"foo", new[] {"a"}}}},
                {"a b c", new ParseResult {{"bar", new[] {"c"}}, {"foo", new[] {"a", "b"}}}}
            };
        }
    }

    public class TestPositionalsNargsOneOrMoreNone : ParserTestCase
    {
        public TestPositionalsNargsOneOrMoreNone()
        {
            ArgumentSignatures = new[] {new Argument("foo") {ValueCount = new ValueCount("+")}, new Argument("bar")};
            Failures = new[] {"", "--foo", "a"};
            Successes = new SuccessCollection
            {
                {"a b", new ParseResult {{"bar", "b"}, {"foo", new[] {"a"}}}},
                {"a b c", new ParseResult {{"bar", "c"}, {"foo", new[] {"a", "b"}}}}
            };
        }
    }

    public class TestPositionalsNargsOptional : ParserTestCase
    {
        public TestPositionalsNargsOptional()
        {
            ArgumentSignatures = new[] {new Argument("foo") {ValueCount = new ValueCount("?")}};
            Failures = new[] {"-x", "a b"};
            Successes = new SuccessCollection
            {
                {"", new ParseResult {{"foo", null}}},
                {"a", new ParseResult {{"foo", "a"}}}
            };
        }
    }

    public class TestPositionalsNargsOptional1 : ParserTestCase
    {
        public TestPositionalsNargsOptional1()
        {
            ArgumentSignatures = new[]
            {
                new Argument("foo") {ValueCount = new ValueCount("?")},
                new Argument("bar") {ValueCount = new ValueCount(1)}
            };
            Failures = new[] {"", "--foo", "a b c"};
            Successes = new SuccessCollection
            {
                {"a", new ParseResult {{"bar", new[] {"a"}}, {"foo", null}}},
                {"a b", new ParseResult {{"bar", new[] {"b"}}, {"foo", "a"}}}
            };
        }
    }

    public class TestPositionalsNargsOptionalConvertedDefault : ParserTestCase
    {
        public TestPositionalsNargsOptionalConvertedDefault()
        {
            ArgumentSignatures = new[]
            {new Argument("foo") {DefaultValue = "42", ValueCount = new ValueCount("?"), TypeName = "int"}};
            Failures = new[] {"-x", "a b", "1 2"};
            Successes = new SuccessCollection
            {
                {"", new ParseResult {{"foo", 42}}},
                {"1", new ParseResult {{"foo", 1}}}
            };
        }
    }

    public class TestPositionalsNargsOptionalDefault : ParserTestCase
    {
        public TestPositionalsNargsOptionalDefault()
        {
            ArgumentSignatures = new[] {new Argument("foo") {DefaultValue = 42, ValueCount = new ValueCount("?")}};
            Failures = new[] {"-x", "a b"};
            Successes = new SuccessCollection
            {
                {"", new ParseResult {{"foo", 42}}},
                {"a", new ParseResult {{"foo", "a"}}}
            };
        }
    }

    public class TestPositionalsNargsOptionalNone : ParserTestCase
    {
        public TestPositionalsNargsOptionalNone()
        {
            ArgumentSignatures = new[]
            {new Argument("foo") {DefaultValue = 42, ValueCount = new ValueCount("?")}, new Argument("bar")};
            Failures = new[] {"", "--foo", "a b c"};
            Successes = new SuccessCollection
            {
                {"a", new ParseResult {{"bar", "a"}, {"foo", 42}}},
                {"a b", new ParseResult {{"bar", "b"}, {"foo", "a"}}}
            };
        }
    }

    public class TestPositionalsNargsOptionalOneOrMore : ParserTestCase
    {
        public TestPositionalsNargsOptionalOneOrMore()
        {
            ArgumentSignatures = new[]
            {
                new Argument("foo") {ValueCount = new ValueCount("?")},
                new Argument("bar") {ValueCount = new ValueCount("+")}
            };
            Failures = new[] {"", "--foo"};
            Successes = new SuccessCollection
            {
                {"a", new ParseResult {{"bar", new[] {"a"}}, {"foo", null}}},
                {"a b", new ParseResult {{"bar", new[] {"b"}}, {"foo", "a"}}},
                {"a b c", new ParseResult {{"bar", new[] {"b", "c"}}, {"foo", "a"}}}
            };
        }
    }

    public class TestPositionalsNargsOptionalOptional : ParserTestCase
    {
        public TestPositionalsNargsOptionalOptional()
        {
            ArgumentSignatures = new[]
            {
                new Argument("foo") {ValueCount = new ValueCount("?")},
                new Argument("bar") {DefaultValue = 42, ValueCount = new ValueCount("?")}
            };
            Failures = new[] {"--foo", "a b c"};
            Successes = new SuccessCollection
            {
                {"", new ParseResult {{"bar", 42}, {"foo", null}}},
                {"a", new ParseResult {{"bar", 42}, {"foo", "a"}}},
                {"a b", new ParseResult {{"bar", "b"}, {"foo", "a"}}}
            };
        }
    }

    public class TestPositionalsNargsOptionalZeroOrMore : ParserTestCase
    {
        public TestPositionalsNargsOptionalZeroOrMore()
        {
            ArgumentSignatures = new[]
            {
                new Argument("foo") {ValueCount = new ValueCount("?")},
                new Argument("bar") {ValueCount = new ValueCount("*")}
            };
            Failures = new[] {"--foo"};
            Successes = new SuccessCollection
            {
                {"", new ParseResult {{"bar", new object[] {}}, {"foo", null}}},
                {"a", new ParseResult {{"bar", new object[] {}}, {"foo", "a"}}},
                {"a b", new ParseResult {{"bar", new[] {"b"}}, {"foo", "a"}}},
                {"a b c", new ParseResult {{"bar", new[] {"b", "c"}}, {"foo", "a"}}}
            };
        }
    }

    public class TestPositionalsNargsZeroOrMore : ParserTestCase
    {
        public TestPositionalsNargsZeroOrMore()
        {
            ArgumentSignatures = new[] {new Argument("foo") {ValueCount = new ValueCount("*")}};
            Failures = new[] {"-x"};
            Successes = new SuccessCollection
            {
                {"", new ParseResult {{"foo", new object[] {}}}},
                {"a", new ParseResult {{"foo", new[] {"a"}}}},
                {"a b", new ParseResult {{"foo", new[] {"a", "b"}}}}
            };
        }
    }

    public class TestPositionalsNargsZeroOrMore1 : ParserTestCase
    {
        public TestPositionalsNargsZeroOrMore1()
        {
            ArgumentSignatures = new[]
            {
                new Argument("foo") {ValueCount = new ValueCount("*")},
                new Argument("bar") {ValueCount = new ValueCount(1)}
            };
            Failures = new[] {"", "--foo"};
            Successes = new SuccessCollection
            {
                {"a", new ParseResult {{"bar", new[] {"a"}}, {"foo", new object[] {}}}},
                {"a b", new ParseResult {{"bar", new[] {"b"}}, {"foo", new[] {"a"}}}},
                {"a b c", new ParseResult {{"bar", new[] {"c"}}, {"foo", new[] {"a", "b"}}}}
            };
        }
    }

    public class TestPositionalsNargsZeroOrMoreDefault : ParserTestCase
    {
        public TestPositionalsNargsZeroOrMoreDefault()
        {
            ArgumentSignatures = new[] {new Argument("foo") {DefaultValue = "bar", ValueCount = new ValueCount("*")}};
            Failures = new[] {"-x"};
            Successes = new SuccessCollection
            {
                {"", new ParseResult {{"foo", "bar"}}},
                {"a", new ParseResult {{"foo", new[] {"a"}}}},
                {"a b", new ParseResult {{"foo", new[] {"a", "b"}}}}
            };
        }
    }

    public class TestPositionalsNargsZeroOrMoreNone : ParserTestCase
    {
        public TestPositionalsNargsZeroOrMoreNone()
        {
            ArgumentSignatures = new[] {new Argument("foo") {ValueCount = new ValueCount("*")}, new Argument("bar")};
            Failures = new[] {"", "--foo"};
            Successes = new SuccessCollection
            {
                {"a", new ParseResult {{"bar", "a"}, {"foo", new object[] {}}}},
                {"a b", new ParseResult {{"bar", "b"}, {"foo", new[] {"a"}}}},
                {"a b c", new ParseResult {{"bar", "c"}, {"foo", new[] {"a", "b"}}}}
            };
        }
    }
}