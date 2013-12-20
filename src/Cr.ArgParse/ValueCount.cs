using System;
using System.Collections.Generic;
using System.Linq;

namespace Cr.ArgParse
{
    /// <summary>
    /// Describe count arguments
    /// </summary>
    public class ValueCount
    {
        public ValueCount() : this(0, 1)
        {
        }

        public ValueCount(uint? n) : this(n, n)
        {
        }

        public ValueCount(uint? min, uint? max)
        {
            Min = min;
            Max = max;
            if (!Min.HasValue && !Max.HasValue)
            {
                Min = 0;
                Max = 1;
            }
            OriginalString = GetRegexString();
        }

        public ValueCount(string countString)
        {
            OriginalString = countString;
            switch (countString)
            {
                case "?":
                    Min = 0;
                    Max = 1;
                    break;
                case "*":
                    Min = 0;
                    break;
                case "+":
                    Min = 1;
                    break;
            }
            IList<uint?> values;
            try
            {
                values =
                    (countString ?? "").TrimStart('{').TrimEnd('}').Split(new[] {","}, StringSplitOptions.None)
                        .Select(it => it.Trim())
                        .Select(it => string.IsNullOrEmpty(it) ? (uint?) null : uint.Parse(it))
                        .ToList();
            }
            catch
            {
                values = new List<uint?>();
            }
            if (values.Count == 1)
                Min = Max = values[0];
            else if (values.Count > 1)
            {
                Min = values[0];
                Max = values[1];
            }
            if (!Min.HasValue && !Max.HasValue)
            {
                Min = 0;
                Max = 1;
            }
        }

        public string OriginalString { get; private set; }

        private string GetRegexString()
        {
            if (Min == 0 && Max == 1)
                return "?";
            if (Min == 0 && !Max.HasValue)
                return "*";
            if (Min == 1 && !Max.HasValue)
                return "+";
            if (!Min.HasValue && !Max.HasValue)
                return "?";
            return Min == Max ? string.Format("{{{0}}}", Min) : string.Format("{{{0},{1}}}", Min, Max);
        }

        public override string ToString()
        {
            return GetRegexString();
        }

        public uint? Max { get; private set; }

        public uint? Min { get; private set; }
    }
}