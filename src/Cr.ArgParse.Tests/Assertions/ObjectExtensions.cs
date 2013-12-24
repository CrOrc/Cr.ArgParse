namespace Cr.ArgParse.Tests.Assertions
{
    public static class ObjectExtensions
    {
        public static bool IsComparable(this object instance) { return TypeExtensions.IsComparable(instance.GetType()) || !instance.GetType().IsClass; }
    }
}