using System;
using System.Collections.Generic;
using Cr.ArgParse.Extensions;

namespace Cr.ArgParse
{
    public class ArgumentParser : IArgumentParser
    {
        public IParseResult ParseArguments(IEnumerable<string> args)
        {
            return new ParseResult();
        }

        public void AddArgument(Argument argument)
        {
            var localPrefixes = Prefixes;
            Argument preparedArgument;
            if (argument.OptionStrings.IsNullOrEmpty() ||
                argument.OptionStrings.Count == 1 && !argument.OptionStrings[0].StartsWith(localPrefixes))
                preparedArgument = PreparePositionalArgument(argument);
            else
                preparedArgument = PrepareOptionalArgument(argument);
            var argumentAction = GetArgumentAction(preparedArgument);
            if(argumentAction == null)
                throw new Exception("Unregistered action exception");
        }

        private IDictionary<string, Func<Argument, IArgumentAction>> actions = new Dictionary<string, Func<Argument, IArgumentAction>>(StringComparer.InvariantCultureIgnoreCase);

        private IArgumentAction GetArgumentAction(Argument argument)
        {
            return new ArgumentAction(argument);
        }

        private Argument PreparePositionalArgument(Argument argument)
        {
            return argument;
        }

        private Argument PrepareOptionalArgument(Argument argument)
        {
            return argument;
        }

        public ArgumentParser()
        {
            Prefixes = new[] {"-", "--", "/"};
        }

        public IList<string> Prefixes { get; set; }
    }
}