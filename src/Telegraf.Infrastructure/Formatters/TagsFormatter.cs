using System.Collections.Generic;
using System.Linq;

namespace Telegraf.Formatters
{
    public static class TagsFormatter
    {
        public static string[] Format(IDictionary<string, string> tags)
        {
            if (tags == null)
                return null;

            if (tags.Count == 0)
                return new string[0];

            var tagSet = tags
                .Select(t => new { Key = KeyFormatter.Format(t.Key), Value = TagValueFormatter.Format(t.Value) })
                .Where(t => string.IsNullOrWhiteSpace(t.Key) == false && string.IsNullOrWhiteSpace(t.Value) == false)
                .Select(t => $"{t.Key}={t.Value}")
                .ToArray();

            return tagSet;
        }
    }
}
