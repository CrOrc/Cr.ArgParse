using System.Collections.Generic;

namespace Cr.ArgParse.Extensions
{
    public static class ListExtensions
    {
        public static bool IsNullOrEmpty<T>(this IList<T> list)
        {
            return ReferenceEquals(list, null) || list.Count < 1;
        }
    }
}