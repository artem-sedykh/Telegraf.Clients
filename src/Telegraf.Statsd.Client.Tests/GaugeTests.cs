using System.Collections;
using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using Telegraf.Statsd.Models;

namespace Telegraf.Statsd.Client.Tests
{
    [TestFixture]
    public class GaugeTests
    {
        [TestCaseSource(nameof(Source))]
        public void BuidGaugeMetric(string measurement, double value, double sample, IDictionary<string,string> tags)
        {
            var metric = Metric.Gauge(measurement, value, sample, tags);

            Assert.IsInstanceOf<DoubleMetricValue>(metric.Value);

            var doubleMetricValue = (DoubleMetricValue)metric.Value;

            Assert.AreEqual(metric.Name, measurement);
            Assert.AreEqual(metric.Type, MetricType.Gauge);
            Assert.AreEqual(doubleMetricValue.Value, value);
            Assert.AreEqual(metric.Sample, sample);

            if (tags == null)
            {
                Assert.IsNotNull(metric.Tags);
                Assert.IsEmpty(metric.Tags);
            }
            else
            {
                metric.Tags.Should().BeEquivalentTo(tags);
            }
        }

        public static IEnumerable Source
        {
            get
            {
                var fixture = new Fixture();

                object tags = fixture.Create<Dictionary<string, string>>();

                yield return new TestCaseData("measurement",3, 15, tags);

                yield return new TestCaseData("measurement", 10, 1, null);

                yield return new TestCaseData("_test_measurement", 1.23,1, null);
            }
        }
    }
}
