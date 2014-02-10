using System.Collections.Generic;
using System.Linq;

namespace Cr.ArgParse.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> list)
        {
            return ReferenceEquals(list, null) || !list.Any();
        }

        public static bool IsTrue(this IEnumerable<bool> list)
        {
            return !ReferenceEquals(list, null) && list.Any(it => it);
        }
    }
}