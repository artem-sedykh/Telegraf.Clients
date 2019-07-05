using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Telegraf.Statsd.Serializer
{
    internal class MeasurementBuilder
    {
        private static readonly Regex ReservedCharactersRegex;

        static MeasurementBuilder()
        {
            var reservedCharacters = new HashSet<char> { ':', '|', '@', ' ', '\n', '\r', '\t', '\\', };

            foreach (var invalidChar in Path.GetInvalidFileNameChars().Union(Path.GetInvalidPathChars()))
                reservedCharacters.Add(invalidChar);

            var pattern = $"[{string.Join("", reservedCharacters.Select(c => Regex.Escape(c.ToString(CultureInfo.InvariantCulture))))}]";

            ReservedCharactersRegex = new Regex(pattern, RegexOptions.Compiled);
        }

        public static string BuildMeasurement(params string[] names)
        {
            var metricName = string.Join(".", names.Where(n => string.IsNullOrWhiteSpace(n) == false).Select(s => SanitizeMeasurement(s.Trim('.').Trim(' '))));

            return metricName;
        }

        public static string SanitizeMeasurement(string metric)
        {
            metric = ReservedCharactersRegex.Replace(metric, "_");
            return metric;
        }
    }
}
