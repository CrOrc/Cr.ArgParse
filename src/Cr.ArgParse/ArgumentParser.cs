using System;
using System.Collections.Generic;

namespace Cr.ArgParse
{
    public class ArgumentParser : ArgumentActionContainer, IArgumentParser
    {
        public IParseResult ParseArguments(IEnumerable<string> args)
        {
            return new ParseResult();
        }

        public ArgumentParser()
        {
            Prefixes = new[] {"-", "--", "/"};
            RegisterArgumentActions(new Dictionary<string, Func<Argument, IArgumentAction>>
            {
                {"store", arg => new StoreAction(arg)},
                {"count", arg => new CountAction(arg)}
            });
            DefaultAction = "store";
        }
    }
}