using NUnit.Framework;

namespace Cr.ArgParse.Tests
{
    [TestFixture] public class ValueCountTest
    {
        private static void AssertAreEqual(ValueCount v1, ValueCount v2)
        {
            AssertAreEqualShort(v1, v2);
            Assert.AreEqual(v1.OriginalString, v2.OriginalString);
        }

        private static void AssertAreEqualSimple(ValueCount v1, ValueCount v2)
        {
            AssertAreEqualShort(v1, v2);
            AssertAreEqualRepr(v1, v2);
        }

        private static void AssertAreEqualShort(ValueCount v1, ValueCount v2)
        {
            Assert.AreEqual(v1.Min, v2.Min);
            Assert.AreEqual(v1.Max, v2.Max);
        }

        private static void AssertAreEqualRepr(ValueCount v1, ValueCount v2)
        {
            Assert.AreEqual(v1.ToString(), v2.ToString());
        }

        [Test] public void DefaultConstructor()
        {
            var valueCount = new ValueCount();
            AssertAreEqual(new ValueCount(0, 1), valueCount);
            AssertAreEqual(new ValueCount((uint?) null), valueCount);
            AssertAreEqual(new ValueCount("?"), valueCount);
        }

        [Test] public void OneArgConstructorIntNegative()
        {
            var value = -10;
            var valueCount = new ValueCount(value);
            Assert.AreEqual(0, valueCount.Min);
            Assert.AreEqual(1, valueCount.Max);
            Assert.AreEqual("?", valueCount.OriginalString);
        }

        [Test] public void OneArgConstructorIntNonNegative()
        {
            var value = 10;
            var valueCount = new ValueCount(value);
            Assert.AreEqual(value, valueCount.Min);
            Assert.AreEqual(value, valueCount.Max);
            Assert.AreEqual("{10}", valueCount.OriginalString);
        }

        [Test] public void OneArgConstructorStringAllMissing()
        {
            var valueCount = new ValueCount(",");
            AssertAreEqualSimple(new ValueCount(0, 1), valueCount);
        }

        [Test] public void OneArgConstructorStringOneInt()
        {
            var valueCount = new ValueCount("10");
            AssertAreEqualSimple(new ValueCount(10), valueCount);
        }

        [Test] public void OneArgConstructorStringOneOrMore()
        {
            var valueCount = new ValueCount("+");
            AssertAreEqual(new ValueCount(1, null), valueCount);
        }

        [Test] public void OneArgConstructorStringOptional()
        {
            var valueCount = new ValueCount("?");
            AssertAreEqual(new ValueCount(0, 1), valueCount);
        }

        [Test] public void OneArgConstructorStringTwoInt()
        {
            var valueCount = new ValueCount("10,30");
            AssertAreEqualSimple(new ValueCount(10, 30), valueCount);
        }

        [Test] public void OneArgConstructorStringTwoIntFirstMissing()
        {
            var valueCount = new ValueCount(",30");
            AssertAreEqualSimple(new ValueCount(0, 30), valueCount);
        }

        [Test] public void OneArgConstructorStringTwoIntSecondMissing()
        {
            var valueCount = new ValueCount("10,");
            AssertAreEqualSimple(new ValueCount(10, null), valueCount);
        }

        [Test] public void OneArgConstructorStringZeroOrMore()
        {
            var valueCount = new ValueCount("*");
            AssertAreEqual(new ValueCount(0, null), valueCount);
        }

        [Test] public void OneArgConstructorUint()
        {
            var value = (uint?) 10;
            var valueCount = new ValueCount(value);
            Assert.AreEqual(value, valueCount.Min);
            Assert.AreEqual(value, valueCount.Max);
            Assert.AreEqual("{10}", valueCount.OriginalString);
        }

        [Test] public void Roundrip()
        {
            var valueCounts = new[]
            {
                new ValueCount(), new ValueCount(0, 1), new ValueCount(null, 1), new ValueCount(1, null),
                new ValueCount(null, 10), new ValueCount(10, null), new ValueCount(10, 30)
            };
            foreach (var valueCount in valueCounts)
                AssertAreEqualSimple(valueCount, new ValueCount(valueCount.ToString()));
        }

        [Test] public void ToRegexString()
        {
            Assert.AreEqual("?", new ValueCount(0, 1).ToString());
            Assert.AreEqual("*", new ValueCount(0, null).ToString());
            Assert.AreEqual("+", new ValueCount(1, null).ToString());
            Assert.AreEqual("{1,10}", new ValueCount(1, 10).ToString());
            Assert.AreEqual("{0,10}", new ValueCount(null, 10).ToString());
            Assert.AreEqual("{10,}", new ValueCount(10, null).ToString());
        }

        [Test] public void TwoArgConstructorIntFirstNegative()
        {
            var value1 = -10;
            var value2 = 30;
            var valueCount = new ValueCount(value1, value2);
            Assert.AreEqual(0, valueCount.Min);
            Assert.AreEqual(value2, valueCount.Max);
            Assert.AreEqual("{0,30}", valueCount.OriginalString);
        }

        [Test] public void TwoArgConstructorIntSecondNegative()
        {
            var value1 = 10;
            var value2 = -10;
            var valueCount = new ValueCount(value1, value2);
            Assert.AreEqual(value1, valueCount.Min);
            Assert.AreEqual(null, valueCount.Max);
            Assert.AreEqual("{10,}", valueCount.OriginalString);
        }

        [Test] public void TwoArgConstructorUint()
        {
            var value1 = (uint?) 10;
            var value2 = (uint?) 30;
            var valueCount = new ValueCount(value1, value2);
            Assert.AreEqual(value1, valueCount.Min);
            Assert.AreEqual(value2, valueCount.Max);
            Assert.AreEqual("{10,30}", valueCount.OriginalString);
        }

        [Test] public void TwoArgConstructorUintFirstNull()
        {
            var value2 = (uint?) 30;
            var valueCount = new ValueCount(null, value2);
            Assert.AreEqual(0, valueCount.Min);
            Assert.AreEqual(value2, valueCount.Max);
            Assert.AreEqual("{0,30}", valueCount.OriginalString);
        }

        [Test] public void TwoArgConstructorUintSecondNull()
        {
            var value1 = (uint?) 10;
            var valueCount = new ValueCount(value1, null);
            Assert.AreEqual(value1, valueCount.Min);
            Assert.AreEqual(null, valueCount.Max);
            Assert.AreEqual("{10,}", valueCount.OriginalString);
        }
    }
}