using System;
using System.Collections.Generic;
using System.IO;
using Telegraf.Infux.Serializers;

namespace Telegraf.Infux.Models
{
    internal class InfluxPoint
    {
        public InfluxPoint(
            string measurement,
            IDictionary<string, object> fields,
            IDictionary<string, string> tags = null,
            DateTime? utcTimestamp = null)
        {
            Measurement = measurement ?? throw new ArgumentNullException(nameof(measurement));

            Fields = fields ?? throw new ArgumentNullException(nameof(fields));

            Tags = tags ?? new Dictionary<string, string>();

            UtcTimestamp = utcTimestamp;
        }

        public string Measurement { get; }

        public IDictionary<string, object> Fields { get; set; }

        public IDictionary<string, string> Tags { get; set; }

        public DateTime? UtcTimestamp { get; }

        public void Format(TextWriter textWriter)
        {
            var value = ToString();

            textWriter.WriteLine($"{value}\n");
        }

        public override string ToString()
        {
            return InfluxPointSerializer.Serialize(this);
        }
    }
}
