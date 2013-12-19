using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Cr.ArgParse.Extensions
{
    public static class StringExtensions
    {
        public static bool StartsWith(this string str, IEnumerable<string> compareStrings)
        {
            return compareStrings.Any(it => str.StartsWith(it, true, CultureInfo.InvariantCulture));
        }
    }
}