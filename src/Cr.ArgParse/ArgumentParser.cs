using System.Collections.Generic;

namespace Cr.ArgParse
{
    public class ArgumentParser : IArgumentParser
    {
        public IParseResult ParseArguments(IEnumerable<string> args)
        {
            return new ParseResult();
        }
    }
}