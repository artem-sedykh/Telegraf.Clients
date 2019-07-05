namespace Telegraf.Statsd.Models
{
    internal class DoubleMetricValue : MetricValue
    {
        public double Value { get; }

        public bool ExplicitlySigned { get; }

        public DoubleMetricValue(double value, bool explicitlySigned = false)
        {
            Value = value;
            ExplicitlySigned = explicitlySigned;
        }
    }
}
