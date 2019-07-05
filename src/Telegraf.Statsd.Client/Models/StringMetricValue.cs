namespace Telegraf.Statsd.Models
{
    internal class StringMetricValue : MetricValue
    {
        public string Value { get; }

        public StringMetricValue(string value)
        {
            Value = value;
        }
    }
}
