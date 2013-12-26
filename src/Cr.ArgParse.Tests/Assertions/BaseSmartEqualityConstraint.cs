using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Cr.ArgParse.Tests.Assertions
{
    public class BaseSmartEqualityConstraint : Constraint
    {
        private readonly object expectedStored;

        private IDictionary<object, object> ComparedObjectMap { get; set; }

        private class CompareResult
        {
            public bool HasPassed { get; set; }
            public string FailureDescription { get; set; }
        }

        private CompareResult MyCompareResult { get; set; }

        private class ReferenceEqualityComparer : EqualityComparer<object>
        {
            public override bool Equals(object x, object y)
            {
                return ReferenceEquals(x, y);
            }

            public override int GetHashCode(object obj)
            {
                return ReferenceEquals(obj, null) ? 0 : obj.GetHashCode();
            }
        }

        public BaseSmartEqualityConstraint(object expected) : base(expected)
        {
            expectedStored = expected;
            FailStrategy = new FailStrategy();
            ComparedObjectMap = new Dictionary<object, object>(new ReferenceEqualityComparer());
        }

        protected IFailStrategy FailStrategy { get; set; }

        public void AreEqual(object expected, object actual)
        {
            if (ReferenceEquals(expected, null))
            {
                Assert.AreEqual(expected, actual);
                return;
            }
            if (ReferenceEquals(expected, actual)) return;
            object oldActual;
            if (ComparedObjectMap.TryGetValue(expected, out oldActual))
            {
                if (ReferenceEquals(actual, oldActual))
                    return;
                return;
            }
            ComparedObjectMap[expected] = actual;
            try
            {
                var objectType = expected.GetType();
                if (objectType == typeof (object))
                {
                    Assert.AreEqual(expected, actual);
                }
                else
                {
                    var specificMethod = GetType()
                        .GetMethod("AreEqual", BindingFlags.Instance | BindingFlags.Public, null,
                            new[] {objectType, objectType}, null);
                    var hasSpecificMethod = false;
                    if (!ReferenceEquals(specificMethod, null))
                    {
                        var args = specificMethod.GetParameters();
                        hasSpecificMethod = args.Length == 2 && args[0].ParameterType == objectType;
                    }
                    if (hasSpecificMethod)
                    {
                        try
                        {
                            specificMethod.Invoke(this, new[] {expected, actual});
                        }
                        catch (TargetInvocationException err)
                        {
                            throw err.InnerException;
                        }
                    }
                    else if (objectType.IsComparable() || !objectType.IsClass)
                    {
                        Assert.AreEqual(expected, actual);
                    }
                    else
                    {
                        var genericIDictionaryType =
                            objectType.GetInterfaces().FirstOrDefault(it =>
                                it.IsGenericType && it.GetGenericTypeDefinition() == typeof (IDictionary<,>));
                        if (!ReferenceEquals(genericIDictionaryType, null))
                        {
                            var genericArgs = genericIDictionaryType.GetGenericArguments();
                            var genericIDictionaryMethod =
                                typeof (BaseSmartEqualityConstraint)
                                    .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                                    .Where(it => it.Name == "AreEqual" && it.IsGenericMethodDefinition)
                                    .Where(
                                        it =>
                                        {
                                            var pis =
                                                it.GetParameters();
                                            return it.GetGenericArguments().Length == 2 &&
                                                   pis.Length == 2
                                                   && pis[0].ParameterType.IsGenericType
                                                   &&
                                                   pis[0].ParameterType.GetGenericTypeDefinition() ==
                                                   typeof (IDictionary<,>)
                                                   && pis[1].ParameterType.IsGenericType
                                                   &&
                                                   pis[1].ParameterType.GetGenericTypeDefinition() ==
                                                   typeof (IDictionary<,>);
                                        })
                                    .FirstOrDefault();
                            if (!ReferenceEquals(genericIDictionaryMethod, null))
                            {
                                var method = genericIDictionaryMethod.MakeGenericMethod(genericArgs);
                                try
                                {
                                    method.Invoke(this, new[] {expected, actual});
                                }
                                catch (TargetInvocationException err)
                                {
                                    throw err.InnerException;
                                }
                            }
                        }
                        else
                        {
                            if (typeof (IDictionary).IsAssignableFrom(objectType))
                            {
                                AreEqual(expected as IDictionary, actual as IDictionary);
                            }
                            else if (typeof (IEnumerable).IsAssignableFrom(objectType))
                            {
                                AreEqual(expected as IEnumerable, actual as IEnumerable);
                            }
                            else if (objectType.IsClass)
                            {
                                AreEqualByProperties(expected, actual, objectType);
                            }
                        }
                    }
                }
            }
            catch
            {
                ComparedObjectMap.Remove(expected);
                throw;
            }
            finally
            {
                ComparedObjectMap.Remove(expected);
            }
        }

        private void AreInGraphOrEqual(object expected, object actual)
        {
            if (ReferenceEquals(expected, null))
                AreEqual(expected, actual);
            else
            {
                object oldActual;
                if (ComparedObjectMap.TryGetValue(expected, out oldActual))
                {
                    if (!ReferenceEquals(actual, oldActual))
                        FailStrategy.ReferenceMismatch();
                }
                else
                    AreEqual(expected, actual);
            }
        }

        protected void AreEqual<TKey, TValue>(IDictionary<TKey, TValue> dictionary1,
            IDictionary<TKey, TValue> dictionary2)
        {
            if (dictionary1 == null || dictionary2 == null)
            {
                Assert.AreEqual(dictionary1, dictionary2);
                return;
            }

            if (dictionary1.Count != dictionary2.Count)
                FailStrategy.CollectionSizesMismatch(dictionary1.Count, dictionary2.Count, dictionary1.GetType());
            foreach (var keyValue in dictionary1)
            {
                if (!dictionary2.ContainsKey(keyValue.Key))
                    FailStrategy.ItemNotinBothDictionaries(keyValue.Key);
                try
                {
                    AreInGraphOrEqual(keyValue.Value, dictionary2[keyValue.Key]);
                }
                catch (Exception ex)
                {
                    FailStrategy.ItemWithKeyMismatch(keyValue.Value, dictionary2[keyValue.Key], keyValue.Key, ex.Message);
                }
            }
        }

        protected virtual void AreEqual(IDictionary dictionary1, IDictionary dictionary2)
        {
            if (dictionary1 == null || dictionary2 == null)
            {
                Assert.AreEqual(dictionary1, dictionary2);
                return;
            }

            if (dictionary1.Count != dictionary2.Count)
                FailStrategy.CollectionSizesMismatch(dictionary1.Count, dictionary2.Count, dictionary1.GetType());
            foreach (var key in dictionary1.Keys)
            {
                if (!dictionary2.Contains(key))
                    FailStrategy.ItemNotinBothDictionaries(key);
                try
                {
                    AreInGraphOrEqual(dictionary1[key], dictionary2[key]);
                }
                catch (Exception ex)
                {
                    FailStrategy.ItemWithKeyMismatch(dictionary1[key], dictionary2[key], key, ex.Message);
                }
            }
        }


        protected void AreEqual(IEnumerable enumerable1, IEnumerable enumerable2)
        {
            if (enumerable1 == null || enumerable2 == null)
            {
                Assert.AreEqual(enumerable1, enumerable2);
                return;
            }
            var collectionItems1 = enumerable1.Cast<object>().ToList();
            var collectionItems2 = enumerable2.Cast<object>().ToList();
            var size1 = collectionItems1.Count;
            var size2 = collectionItems2.Count;
            if (size1 != size2)
                FailStrategy.CollectionSizesMismatch(size1, size2, enumerable1.GetType());

            for (var i = 0; i < size1; ++i)
            {
                var collectionItem1 = collectionItems1[i];
                var collectionItem2 = collectionItems2[i];
                try
                {
                    AreInGraphOrEqual(collectionItem1, collectionItem2);
                }
                catch (Exception ex)
                {
                    FailStrategy.ItemMismatch(collectionItem1, collectionItem2, i, ex.Message);
                }
            }
        }

        public void AreEqualByProperties(object expected, object actual, Type objectType = null,
            params string[] propertiesToIgnore)
        {
            if (expected == null || actual == null)
            {
                Assert.AreEqual(expected, actual);
                return;
            }
            if (objectType == null)
                objectType = expected.GetType();
            foreach (
                var propertyInfo in objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p =>
                    p.CanRead && p.GetIndexParameters().Length == 0 &&
                    !propertiesToIgnore.Contains(p.Name)))
            {
                var valueA = propertyInfo.GetValue(expected, null);
                var valueB = propertyInfo.GetValue(actual, null);

                try
                {
                    AreInGraphOrEqual(valueA, valueB);
                }
                catch (Exception ex)
                {
                    FailStrategy.PropertyMismatch(valueA, valueB, objectType, propertyInfo,
                        valueA != null && valueB != null ? ex.Message : "");
                }
            }
        }

        public override bool Matches(object actual)
        {
            this.actual = actual;
            try
            {
                AreEqual(expectedStored, actual);
                MyCompareResult = new CompareResult {HasPassed = true, FailureDescription = ""};
            }
            catch (Exception err)
            {
                MyCompareResult = new CompareResult {HasPassed = false, FailureDescription = err.Message};
            }
            return MyCompareResult.HasPassed;
        }

        public override void WriteDescriptionTo(MessageWriter writer)
        {
            writer.WriteExpectedValue(expectedStored);
        }

        public override void WriteActualValueTo(MessageWriter writer)
        {
            base.WriteActualValueTo(writer);
            writer.WriteLine();
            writer.Write(MyCompareResult.FailureDescription);
        }
    }
}