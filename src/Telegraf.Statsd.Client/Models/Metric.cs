using System.Collections.Generic;

namespace Telegraf.Statsd.Models
{
    internal class Metric
    {
        public string Name { get; }

        public MetricType Type { get; }

        public MetricValue Value { get; }

        public double Sample { get; }

        public IDictionary<string, string> Tags { get; }

        public Metric(string name, MetricType type, MetricValue value, double sample, IDictionary<string, string> tags)
        {
            Name = name;
            Value = value;
            Sample = sample;
            Type = type;
            Tags = tags ?? new Dictionary<string, string>();
        }

        public static Metric Counter(string name, double value, double sample = 1, IDictionary<string, string> tags = null)
        {
            return new Metric(name, MetricType.Counter, new DoubleMetricValue(value), sample, tags);
        }

        public static Metric Gauge(string name, double value, double sample = 1, IDictionary<string, string> tags = null)
        {
            return new Metric(name, MetricType.Gauge, new DoubleMetricValue(value), sample, tags);
        }

        public static Metric Time(string name, double value, double sample = 1, IDictionary<string, string> tags = null)
        {
            return new Metric(name, MetricType.Time, new DoubleMetricValue(value), sample, tags);
        }

        public static Metric Set(string name, string value, double sample = 1, IDictionary<string, string> tags = null)
        {
            return new Metric(name, MetricType.Set, new StringMetricValue(value), sample, tags);
        }
    }
}