using System;
using System.Collections;
using NUnit.Framework;
using Telegraf.Statsd.Models;
using Telegraf.Statsd.Serializer;

namespace Telegraf.Statsd.Client.Tests.Serializers
{
    [TestFixture]
    public class MetricSerializerTest
    {
        [TestCase(MetricType.Gauge, ExpectedResult = "g")]
        [TestCase(MetricType.Counter, ExpectedResult = "c")]
        [TestCase(MetricType.Set, ExpectedResult = "s")]
        [TestCase(MetricType.Time, ExpectedResult = "ms")]
        public string GetMetricTypeSpecifier(int metricType)
        {
            return MetricSerializer.GetMetricTypeSpecifier((MetricType)metricType);
        }

        [TestCase(MetricType.Undefined)]
        public void GetMetricTypeSpecifierArgumentOutOfRangeException(int metricType)
        {
            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                MetricSerializer.GetMetricTypeSpecifier((MetricType) metricType);
            });
        }

        [TestCaseSource(nameof(MetricSource))]
        public string SerializeMetric(object metric)
        {
            var serializer = new MetricSerializer();

            var serializedMetric = serializer.SerializeMetric((Metric)metric);

            return serializedMetric;
        }

        public static IEnumerable MetricSource
        {
            get
            {
                #region Counters

                yield return new TestCaseData(Metric.Counter("deploys.test.myservice", 1, 0.1)).Returns("deploys.test.myservice:1|c|@0.1");

                yield return new TestCaseData(Metric.Counter("deploys.test.myservice", 1)).Returns("deploys.test.myservice:1|c");

                yield return new TestCaseData(Metric.Counter("deploys.test.myservice", 101)).Returns("deploys.test.myservice:101|c");

                yield return new TestCaseData(Metric.Counter("deploys.test.myservice", -1)).Returns("deploys.test.myservice:-1|c");

                yield return new TestCaseData(Metric.Counter("deploys.test.myservice", 0.1)).Returns("deploys.test.myservice:0.1|c");

                #endregion

                #region Gauges

                yield return new TestCaseData(Metric.Gauge("users.current.den001.myapp", 32)).Returns("users.current.den001.myapp:32|g");

                yield return new TestCaseData(Metric.Gauge("users.current.den001.myapp", -10)).Returns("users.current.den001.myapp:-10|g");

                yield return new TestCaseData(Metric.Gauge("users.current.den001.myapp", 1,0.1)).Returns("users.current.den001.myapp:1|g|@0.1");

                #endregion

                #region Sets

                yield return new TestCaseData(Metric.Set("users.unique", "101")).Returns("users.unique:101|s");

                yield return new TestCaseData(Metric.Set("users.unique", "test")).Returns("users.unique:test|s");

                #endregion

                #region Timings

                yield return new TestCaseData(Metric.Time("load.time", 320)).Returns("load.time:320|ms");

                yield return new TestCaseData(Metric.Time("load.time", 200,0.1)).Returns("load.time:200|ms|@0.1");

                #endregion
            }
        }
    }
}
