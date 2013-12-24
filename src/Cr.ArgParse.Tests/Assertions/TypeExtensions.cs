using System;

namespace Cr.ArgParse.Tests.Assertions
{
    public static class TypeExtensions
    {
        public static bool IsComparable(this Type value)
        {
            return typeof(IComparable).IsAssignableFrom(value) || value.IsPrimitive || value.IsValueType;
        }
    }
}