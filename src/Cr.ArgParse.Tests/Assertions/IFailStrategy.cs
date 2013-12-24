using System;
using System.Reflection;

namespace Cr.ArgParse.Tests.Assertions
{
    public interface IFailStrategy
    {
        void PropertyMismatch(object expectedValue, object actualValue, Type objectType,
            PropertyInfo propertyInfo, string originalMessage = "");

        void UncomparableProperty(Type objectType, PropertyInfo propertyInfo);
        void CollectionSizesMismatch(int expectedSize, int actualSize, Type collectionType);
        void ItemNotinBothDictionaries(object key);
        void ItemWithKeyMismatch(object expectedValue, object actualValue, object key, string originalMessage = "");
        void ItemMismatch(object expectedValue, object actualValue, int position, string originalMessage = "");
        void ReferenceMismatch();
    }
}