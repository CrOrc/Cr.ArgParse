using System;
using System.IO;
using System.Reflection;
using System.Text;
using NUnit.Framework;

namespace Cr.ArgParse.Tests.Assertions
{
    public class FailStrategy : IFailStrategy
    {
        private static string Indent(string message, string indentation)
        {
            using (var stringReader = new StringReader(message))
            {
                var stringBuilder = new StringBuilder();
                string line;
                while ((line = stringReader.ReadLine()) != null)
                {
                    stringBuilder.AppendLine(indentation + line);
                }
                return stringBuilder.ToString();
            }
        }

        private static string AppendOriginalMessage(string message, string originalMessage)
        {
            return string.IsNullOrWhiteSpace(originalMessage) ? message : string.Format("{0}{1}{2}", message, Environment.NewLine, Indent(originalMessage, "  "));
        }

        public void PropertyMismatch(object expectedValue, object actualValue, Type objectType,
            PropertyInfo propertyInfo, string originalMessage)
        {
            var message = string.Format(FailMessage.PropertyMismatch,
                expectedValue ?? "null",
                actualValue ?? "null",
                objectType.FullName,
                propertyInfo.PropertyType,
                propertyInfo.Name);
            Assert.Fail(AppendOriginalMessage(message, originalMessage));
        }

        public void UncomparableProperty(Type objectType, PropertyInfo propertyInfo)
        {
            Assert.Fail(FailMessage.UncomparableProperty, objectType.FullName, propertyInfo.Name);
        }

        public void CollectionSizesMismatch(int expectedSize, int actualSize, Type collectionType)
        {
            Assert.Fail(FailMessage.CollectionSizesMismatch,
                expectedSize,
                actualSize,
                collectionType.FullName);
        }

        public void ItemNotinBothDictionaries(object key)
        {
            Assert.Fail(FailMessage.ItemNotInBothDictionaries, key);
        }

        public void ItemWithKeyMismatch(object expectedValue, object actualValue, object key, string originalMessage)
        {
            var message = string.Format(FailMessage.ItemWithKeyMismatch, expectedValue, actualValue, key);
            Assert.Fail(AppendOriginalMessage(message, originalMessage));
        }

        public void ItemMismatch(object expectedValue, object actualValue, int position, string originalMessage)
        {
            var message = string.Format(FailMessage.ItemMismatch, expectedValue, actualValue, position);
            Assert.Fail(AppendOriginalMessage(message, originalMessage));
        }

        public void ReferenceMismatch()
        {
            Assert.Fail(FailMessage.ReferenceMismatch);
        }
    }
}