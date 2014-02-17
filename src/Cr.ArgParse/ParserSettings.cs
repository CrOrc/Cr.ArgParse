using System.Collections.Generic;

namespace Cr.ArgParse
{
    public class ParserSettings
    {
        public ParserSettings()
        {
            Description = "";
            Prefixes = new[] {"-", "--", "/"};
            ConflictHandlerType = ConflictHandlerType.Error;
        }

        public ConflictHandlerType ConflictHandlerType { get; set; }

        public string Description { get; set; }
        public IList<string> Prefixes { get; set; }
    }
}