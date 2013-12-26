using System;
using System.Collections.Generic;

namespace Cr.ArgParse
{
    public class Parser : ActionContainer, IArgumentParser
    {
        public Parser(string description, IList<string> prefixes, string conflictHandlerName) : base(description, prefixes, conflictHandlerName)
        {
        }

        public ParseResult ParseArguments(IEnumerable<string> args, ParseResult parseResult = null)
        {
            if(parseResult == null)
                parseResult = new ParseResult();
            //add any action defaults that aren't present
            foreach (var action in Actions)
            {
                //if action.dest is not SUPPRESS:
                if (action.Destination != null)
                {
                    var res = parseResult[action.Destination];
                    if(ReferenceEquals(res,null))
                        //if action.default is not SUPPRESS:
                        if (!ReferenceEquals(action.Argument.DefaultValue, null))
                            parseResult[action.Destination] = action.Argument.DefaultValue;
                }
            }
            return parseResult;
        }

        public Parser()
            : this("", new[] { "-", "--", "/" },"resolve")
        {
        }
    }
}