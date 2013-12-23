using System;
using System.Collections.Generic;

namespace Cr.ArgParse
{
    public class Parser : ActionContainer, IArgumentParser
    {
        public Parser(string description, IList<string> prefixes, string conflictHandlerName) : base(description, prefixes, conflictHandlerName)
        {
        }

        public ParseResult ParseArguments(IEnumerable<string> args)
        {
            return new ParseResult();
        }

        public Parser()
            : this("", new[] { "-", "--", "/" },"resolve")
        {
        }
    }
}