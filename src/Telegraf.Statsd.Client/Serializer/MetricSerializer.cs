using System;
using System.Globalization;
using Telegraf.Formatters;
using Telegraf.Statsd.Models;

namespace Telegraf.Statsd.Serializer
{
    internal class MetricSerializer
    {
        private const string MetricDatagramFormat = "{0}|{1}";
        private const string SampledMetricDatagramFormat = MetricDatagramFormat + "|@{2:G}";
        private readonly CultureInfo cultureInfo = CultureInfo.InvariantCulture;
        private const string StatsdFormat = "{0}:{1}";
        private const string StatsdTagsFormat = "{0},{2}:{1}";

        public string SerializeMetric(Metric metric)
        {
            var value = FormattedMetricValue(metric.Type, metric.Value, metric.Sample);

            var measurement = MeasurementBuilder.BuildMeasurement(metric.Name);

            var tags = metric.Tags;

            var tagsValue = tags == null ? null : string.Join(",", TagsFormatter.Format(tags));

            var format = string.IsNullOrWhiteSpace(tagsValue) ? StatsdFormat : StatsdTagsFormat;

            var datagram = string.Format(format, measurement, value, tagsValue);

            return datagram;
        }

        internal string FormattedMetricValue(MetricType type, MetricValue metricValue, double sample)
        {
            if (sample < 0 || sample > 1)
                throw new ArgumentOutOfRangeException(nameof(sample));

            var metricTypeSpecifier = GetMetricTypeSpecifier(type);

            var value = metricValue.ToString();

            if (string.IsNullOrWhiteSpace(metricTypeSpecifier))
                throw new ArgumentException(nameof(type));

            if (sample < 0 || sample > 1)
                throw new ArgumentOutOfRangeException(nameof(sample));

            var format = sample < 1 ? SampledMetricDatagramFormat : MetricDatagramFormat;

            return string.Format(cultureInfo, format, value, metricTypeSpecifier, sample);
        }

        internal static string GetMetricTypeSpecifier(MetricType metricType)
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
    }
}
