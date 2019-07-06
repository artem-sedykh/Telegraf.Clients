using Telegraf.Formatters;
using Telegraf.Infux.Models;

namespace Telegraf.Infux.Serializers
{
    internal static class InfluxPointSerializer
    {
        public static string Serialize(InfluxPoint point)
        {
            var tags = point.Tags;
            var fields = point.Fields;

            var allTags = string.Join(",", TagsFormatter.Format(tags));
            var allFields = string.Join(",", FieldFormatter.Format(fields));

            var tagsPart = allTags.Length > 0 ? $",{allTags}" : allTags;

            var measurement = KeyFormatter.Format(point.Measurement);

            return $"{measurement}{tagsPart} {allFields} {FieldValueFormatter.FormatTimestamp(point.UtcTimestamp)}".Trim();
        }
    }
}
