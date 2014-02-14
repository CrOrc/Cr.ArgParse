using System.Collections.Generic;

namespace Cr.ArgParse
{
    /// <summary>
    /// Allows to parse list of command line arguments
    /// </summary>
    public interface IArgumentParser
    {
        ParseResult ParseArguments(IEnumerable<string> args, ParseResult parseResult = null);
        ParseResult ParseKnownArguments(IEnumerable<string> args, ParseResult parseResult = null);
    }
}