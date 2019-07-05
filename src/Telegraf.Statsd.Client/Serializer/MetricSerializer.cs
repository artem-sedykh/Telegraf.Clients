using System;
using System.Collections.Generic;
using System.Globalization;
using Telegraf.Formatters;
using Telegraf.Statsd.Models;

namespace Telegraf.Statsd.Serializer
{
    internal class MetricSerializer
    {
        private const string MetricDatagramFormat = "{0}|{1}";
        private const string SampledMetricDatagramFormat = MetricDatagramFormat + "|@{2:N3}";
        private readonly CultureInfo cultureInfo = CultureInfo.InvariantCulture;

        public string SerializeMetric(Metric metric)
        {
            var type = GetMetricTypeSpecifier(metric.Type);

            switch (metric.Value)
            {
                case DoubleMetricValue doubleMetricValue:
                    {
                        var datagram = SerializeMetric(metric.Name, type, doubleMetricValue.Value, metric.Sample, metric.Tags, doubleMetricValue.ExplicitlySigned);

                        return datagram;
                    }
                case StringMetricValue stringMetricValue:
                    {
                        var datagram = SerializeMetric(metric.Name, type, stringMetricValue.Value, metric.Sample, metric.Tags);

                        return datagram;
                    }
                default:
                    throw new InvalidOperationException();
            }
        }

        public string SerializeMetric(string metric, string type, double value, double sample, IDictionary<string, string> tags, bool explicitlySigned = false)
        {
            if (string.IsNullOrWhiteSpace(metric))
                throw new ArgumentException("metric");

            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException("type");

            if (sample < 0 || sample > 1)
                throw new ArgumentOutOfRangeException(nameof(sample));

            var metricValueFormat = explicitlySigned ? "{0:+#.####;-#.####;#}" : "{0}";

            var metricValue = Math.Abs(value) < 0.00000001 ? explicitlySigned ? "+0" : "0" : string.Format(cultureInfo, metricValueFormat, (float)value);

            var datagram = SerializeMetric(metric, type, metricValue, sample, tags);

            return datagram;
        }

        public string SerializeMetric(string measurement, string type, string value, double sample, IDictionary<string, string> tags)
        {
            if (string.IsNullOrWhiteSpace(measurement))
                throw new ArgumentException(nameof(measurement));

            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException(nameof(type));

            if (sample < 0 || sample > 1)
                throw new ArgumentOutOfRangeException(nameof(sample));

            measurement = MeasurementBuilder.BuildMeasurement(measurement);

            var format = sample < 1 ? SampledMetricDatagramFormat : MetricDatagramFormat;

            var metric = string.Format(cultureInfo, format, value, type, sample);

            var datagram = InfluxStatsDMetric(measurement, tags, metric);

            return datagram;
        }

        private static string GetMetricTypeSpecifier(MetricType metricType)
        {
            switch (metricType)
            {
                case MetricType.Counter:
                    return "c";
                case MetricType.Gauge:
                    return "g";
                case MetricType.Time:
                    return "ms";
                case MetricType.Set:
                    return "s";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static string InfluxStatsDMetric(string measurement, IDictionary<string, string> tags, string metric)
        {
            var allTags = tags == null ? null : string.Join(",", TagsFormatter.Format(tags));

            measurement = KeyFormatter.Format(measurement);

            var datagram = $"{measurement}";

            if (!string.IsNullOrEmpty(allTags))
                datagram += $",{allTags}";

            datagram += $":{metric}";

            return datagram;
        }
    }
}
