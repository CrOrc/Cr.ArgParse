using System;
using System.Collections.Generic;

namespace Cr.ArgParse
{
    public class ArgumentParser : ArgumentActionContainer, IArgumentParser
    {
        public ArgumentParser(string description, IList<string> prefixes, string conflictHandlerName) : base(description, prefixes, conflictHandlerName)
        {
        }

        public ParseResult ParseArguments(IEnumerable<string> args)
        {
            return new ParseResult();
        }

        public ArgumentParser()
            : this("", new[] { "-", "--", "/" },"resolve")
        {
            RegisterArgumentActions(new Dictionary<string, Func<Argument, ArgumentAction>>
            {
                {"store", arg => new StoreAction(arg)},
                {"count", arg => new CountAction(arg)}
            });
            DefaultAction = "store";
        }
    }
}