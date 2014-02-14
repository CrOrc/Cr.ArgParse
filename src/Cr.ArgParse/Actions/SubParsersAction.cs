using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cr.ArgParse.Exceptions;
using Cr.ArgParse.Extensions;

namespace Cr.ArgParse.Actions
{
    public class SubParsersAction : ArgumentAction
    {
        private readonly IDictionary<string, IArgumentParser> subParsers;

        public SubParsersAction(Argument argument) : base(argument)
        {
            subParsers = new SortedDictionary<string, IArgumentParser>(StringComparer.InvariantCulture);
        }

        public override void Call(ParseResult parseResult, object values, string optionString)
        {
            var valueStrings = (values as IEnumerable ?? new string[] {}).OfType<string>().ToList();
            var parserName = valueStrings.FirstOrDefault();
            var argStrings = valueStrings.Skip(1).ToList();
            if (HasValidDestination)
                parseResult[Destination] = parserName;

            var parser = subParsers.SafeGetValue(parserName);
            if (parser == null)
                throw new UnknownParserException(this, parserName);
            
            // parse all the remaining options into the namespace
            // store any unrecognized options on the object, so that the top
            // level parser can decide what to do with them
            parser.ParseKnownArguments(argStrings, parseResult);
        }
    }
}