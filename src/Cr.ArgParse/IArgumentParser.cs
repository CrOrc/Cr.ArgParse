using System.Collections.Generic;

namespace Cr.ArgParse
{
    public interface IArgumentParser
    {
        IParseResult ParseArguments(IEnumerable<string> args);
        void AddArgument(Argument argument);
    }
}