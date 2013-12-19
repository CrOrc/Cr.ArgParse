using System.Collections.Generic;

namespace Cr.ArgParse
{
    /// <summary>
    /// Single argument description
    /// </summary>
    public class Argument
    {
        /// <summary>
        /// strings to identify argument. Empty for positional arguments
        /// </summary>
        public IList<string> OptionStrings { get; set; }
        /// <summary>
        /// Name for storing parsing results
        /// </summary>
        public string Destination { get; set; }
        /// <summary>
        /// Description of argument
        /// </summary>
        public string HelpText { get; set; }
    }
}