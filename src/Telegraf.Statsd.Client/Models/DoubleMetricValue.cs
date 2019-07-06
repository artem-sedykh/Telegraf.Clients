using System;
using System.Globalization;

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

        public override string ToString()
        {
            var metricValueFormat = ExplicitlySigned ? "{0:+#.####;-#.####;#}" : "{0}";

            var formattedValue = Math.Abs(Value) < 0.00000001 ? ExplicitlySigned ? "+0" : "0" : string.Format(CultureInfo.InvariantCulture, metricValueFormat, (float)Value);

            return formattedValue;
        }
    }
}
