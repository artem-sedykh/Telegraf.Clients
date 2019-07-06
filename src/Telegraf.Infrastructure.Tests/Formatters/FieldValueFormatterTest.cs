using System;
using System.Collections;
using NUnit.Framework;
using Telegraf.Formatters;

namespace Telegraf.Infrastructure.Tests.Formatters
{
    [TestFixture]
    public class FieldValueFormatterTest
    {
        [TestCaseSource(nameof(Source))]
        public string Format(object value)
        {
            var result = FieldValueFormatter.Format(value);

            return result;
        }

        public static IEnumerable Source
        {
            get
            {
                var ts = new TimeSpan(0, 12, 01, 15);

                yield return new TestCaseData(82).Returns("82i");
                yield return new TestCaseData(82D).Returns("82");
                yield return new TestCaseData(82.12D).Returns("82.12");
                yield return new TestCaseData(82.123m).Returns("82.123");
                yield return new TestCaseData(true).Returns("t");
                yield return new TestCaseData(false).Returns("f");
                yield return new TestCaseData(ts).Returns($"{ts.TotalMilliseconds}");

                yield return new TestCaseData("too warm").Returns("\"too warm\"");

                yield return new TestCaseData("too 'warm'").Returns("\"too 'warm'\"");
            }
        }
    }
}
