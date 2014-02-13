using System;

namespace Cr.ArgParse.Tests
{
    public class TestEmptyAndSpaceContainingArguments : ParserTestCase
    {
        public TestEmptyAndSpaceContainingArguments()
        {
            ArgumentSignatures = new[]
            {new Argument("x") {ValueCount = new ValueCount("?")}, new Argument("-y", "--yyy") {Destination = "y"}};
            Failures = new[] {"-y"};
            Successes = new SuccessCollection
            {
                {new[] {""}, new ParseResult {{"x", ""}, {"y", null}}},
                {new[] {"a badger"}, new ParseResult {{"x", "a badger"}, {"y", null}}},
                {new[] {"-a badger"}, new ParseResult {{"x", "-a badger"}, {"y", null}}},
                {new[] {"-y", ""}, new ParseResult {{"x", null}, {"y", ""}}},
                {new[] {"-y", "a badger"}, new ParseResult {{"x", null}, {"y", "a badger"}}},
                {new[] {"-y", "-a badger"}, new ParseResult {{"x", null}, {"y", "-a badger"}}},
                {new[] {"--yyy=a badger"}, new ParseResult {{"x", null}, {"y", "a badger"}}},
                {new[] {"--yyy=-a badger"}, new ParseResult {{"x", null}, {"y", "-a badger"}}}
            };
        }
    }

    public class TestNargsRemainder : ParserTestCase
    {
        public TestNargsRemainder()
        {
            ArgumentSignatures = new[] {new Argument("x"), new Argument("y") {IsRemainder = true}, new Argument("-z")};
            Failures = new[] {"", "-z", "-z Z"};
            Successes = new SuccessCollection
            {
                {"X", new ParseResult {{"x", "X"}, {"y", new object[] {}}, {"z", null}}},
                {"-z Z X", new ParseResult {{"x", "X"}, {"y", new object[] {}}, {"z", "Z"}}},
                {"X A B -z Z", new ParseResult {{"x", "X"}, {"y", new[] {"A", "B", "-z", "Z"}}, {"z", null}}},
                {"X Y --foo", new ParseResult {{"x", "X"}, {"y", new[] {"Y", "--foo"}}, {"z", null}}}
            };
        }
    }

    public class TestNargsZeroOrMore : ParserTestCase
    {
        public TestNargsZeroOrMore()
        {
            ArgumentSignatures = new[]
            {
                new Argument("-x") {ValueCount = new ValueCount("*")},
                new Argument("y") {ValueCount = new ValueCount("*")}
            };

            Successes = new SuccessCollection
            {
                {"", new ParseResult {{"x", null}, {"y", new object[] {}}}},
                {"-x", new ParseResult {{"x", new object[] {}}, {"y", new object[] {}}}},
                {"-x a", new ParseResult {{"x", new[] {"a"}}, {"y", new object[] {}}}},
                {"-x a -- b", new ParseResult {{"x", new[] {"a"}}, {"y", new[] {"b"}}}},
                {"a", new ParseResult {{"x", null}, {"y", new[] {"a"}}}},
                {"a -x", new ParseResult {{"x", new object[] {}}, {"y", new[] {"a"}}}},
                {"a -x b", new ParseResult {{"x", new[] {"b"}}, {"y", new[] {"a"}}}}
            };
        }
    }

    public class TestOptionLike : ParserTestCase
    {
        public TestOptionLike()
        {
            ArgumentSignatures = new[]
            {
                new Argument("-x") {TypeName = "float"}, new Argument("-3") {Destination = "y", TypeName = "float"},
                new Argument("z") {ValueCount = new ValueCount("*")}
            };
            Failures = new[]
            {
                "-x", "-y2.5", "-xa", "-x -a", "-x -3", "-x -3.5", "-3 -3.5", "-x -2.5", "-x -2.5 a", "-3 -.5", "a x -1",
                "-x -1 a", "-3 -1 a"
            };
            Successes = new SuccessCollection
            {
                {"", new ParseResult {{"x", null}, {"y", null}, {"z", new object[] {}}}},
                {"-x 2.5", new ParseResult {{"x", 2.5}, {"y", null}, {"z", new object[] {}}}},
                {"-x 2.5 a", new ParseResult {{"x", 2.5}, {"y", null}, {"z", new[] {"a"}}}},
                {"-3.5", new ParseResult {{"x", null}, {"y", 0.5}, {"z", new object[] {}}}},
                {"-3-.5", new ParseResult {{"x", null}, {"y", -0.5}, {"z", new object[] {}}}},
                {"-3 .5", new ParseResult {{"x", null}, {"y", 0.5}, {"z", new object[] {}}}},
                {"a -3.5", new ParseResult {{"x", null}, {"y", 0.5}, {"z", new[] {"a"}}}},
                {"a", new ParseResult {{"x", null}, {"y", null}, {"z", new[] {"a"}}}},
                {"a -x 1", new ParseResult {{"x", 1.0}, {"y", null}, {"z", new[] {"a"}}}},
                {"-x 1 a", new ParseResult {{"x", 1.0}, {"y", null}, {"z", new[] {"a"}}}},
                {"-3 1 a", new ParseResult {{"x", null}, {"y", 1.0}, {"z", new[] {"a"}}}}
            };
        }
    }

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

    public class TestOptionalsAlternatePrefixChars : ParserTestCase
    {
        public TestOptionalsAlternatePrefixChars()
        {
            ArgumentSignatures = new[]
            {
                new Argument("+f") {ActionName = "store_true"}, new Argument("::bar"),
                new Argument("/baz") {ActionName = "store_const", ConstValue = 42}
            };
            Failures = new[]
            {"--bar", "-fbar", "-b B", "B", "-f", "--bar B", "-baz", "-h", "--help", "+h", "::help", "/help"};
            Successes = new SuccessCollection
            {
                {"", new ParseResult {{"bar", null}, {"baz", null}, {"f", false}}},
                {"+f", new ParseResult {{"bar", null}, {"baz", null}, {"f", true}}},
                {"::ba B", new ParseResult {{"bar", "B"}, {"baz", null}, {"f", false}}},
                {"+f ::bar B", new ParseResult {{"bar", "B"}, {"baz", null}, {"f", true}}},
                {"+f /b", new ParseResult {{"bar", null}, {"baz", 42}, {"f", true}}},
                {"/ba +f", new ParseResult {{"bar", null}, {"baz", 42}, {"f", true}}}
            };
        }
    }

    public class TestOptionalsAlternatePrefixCharsAddedHelp : ParserTestCase
    {
        public TestOptionalsAlternatePrefixCharsAddedHelp()
        {
            ArgumentSignatures = new[]
            {
                new Argument("+f") {ActionName = "store_true"}, new Argument("::bar"),
                new Argument("/baz") {ActionName = "store_const", ConstValue = 42}
            };
            Failures = new[] {"--bar", "-fbar", "-b B", "B", "-f", "--bar B", "-baz"};
            Successes = new SuccessCollection
            {
                {"", new ParseResult {{"bar", null}, {"baz", null}, {"f", false}}},
                {"+f", new ParseResult {{"bar", null}, {"baz", null}, {"f", true}}},
                {"::ba B", new ParseResult {{"bar", "B"}, {"baz", null}, {"f", false}}},
                {"+f ::bar B", new ParseResult {{"bar", "B"}, {"baz", null}, {"f", true}}},
                {"+f /b", new ParseResult {{"bar", null}, {"baz", 42}, {"f", true}}},
                {"/ba +f", new ParseResult {{"bar", null}, {"baz", 42}, {"f", true}}}
            };
        }
    }

    public class TestOptionalsAlternatePrefixCharsMultipleShortArgs : ParserTestCase
    {
        public TestOptionalsAlternatePrefixCharsMultipleShortArgs()
        {
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

    public class TestOptionalsChoices : ParserTestCase
    {
        public TestOptionalsChoices()
        {
            ArgumentSignatures = new[] {new Argument("-f"), new Argument("-g") {TypeName = "int"}};
            Failures = new[] {"a", "-f d", "-fad", "-ga", "-g 6"};
            Successes = new SuccessCollection
            {
                {"", new ParseResult {{"f", null}, {"g", null}}},
                {"-f a", new ParseResult {{"f", "a"}, {"g", null}}},
                {"-f c", new ParseResult {{"f", "c"}, {"g", null}}},
                {"-g 0", new ParseResult {{"f", null}, {"g", 0}}},
                {"-g 03", new ParseResult {{"f", null}, {"g", 3}}},
                {"-fb -g4", new ParseResult {{"f", "b"}, {"g", 4}}}
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

    public class TestOptionalsRequired : ParserTestCase
    {
        public TestOptionalsRequired()
        {
            ArgumentSignatures = new[] {new Argument("-x") {TypeName = "int"}};
            Failures = new[] {"a", ""};
            Successes = new SuccessCollection
            {
                {"-x 1", new ParseResult {{"x", 1}}},
                {"-x42", new ParseResult {{"x", 42}}}
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

    public class TestParserDefaultSuppress : ParserTestCase
    {
        public TestParserDefaultSuppress()
        {
            ArgumentSignatures = new[]
            {
                new Argument("foo") {ValueCount = new ValueCount("?")},
                new Argument("bar") {ValueCount = new ValueCount("*")},
                new Argument("--baz") {ActionName = "store_true"}
            };
            Failures = new[] {"-x"};
            Successes = new SuccessCollection
            {
                {"", new ParseResult {}},
                {"a", new ParseResult {{"foo", "a"}}},
                {"a b", new ParseResult {{"bar", new[] {"b"}}, {"foo", "a"}}},
                {"--baz", new ParseResult {{"baz", true}}},
                {"a --baz", new ParseResult {{"baz", true}, {"foo", "a"}}},
                {"--baz a b", new ParseResult {{"bar", new[] {"b"}}, {"baz", true}, {"foo", "a"}}}
            };
        }
    }

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

    public class TestPositionalsChoicesInt : ParserTestCase
    {
        public TestPositionalsChoicesInt()
        {
            ArgumentSignatures = new[] {new Argument("spam") {TypeName = "int"}};
            Failures = new[] {"", "--foo", "h", "42", "ef"};
            Successes = new SuccessCollection
            {
                {"4", new ParseResult {{"spam", 4}}},
                {"15", new ParseResult {{"spam", 15}}}
            };
        }
    }

    public class TestPositionalsChoicesString : ParserTestCase
    {
        public TestPositionalsChoicesString()
        {
            ArgumentSignatures = new[] {new Argument("spam")};
            Failures = new[] {"", "--foo", "h", "42", "ef"};
            Successes = new SuccessCollection
            {
                {"a", new ParseResult {{"spam", "a"}}},
                {"g", new ParseResult {{"spam", "g"}}}
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