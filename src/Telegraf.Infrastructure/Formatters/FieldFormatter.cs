using System.Collections.Generic;
using System.Linq;

namespace Telegraf.Formatters
{
    public static class FieldFormatter
    {
        public static string[] Format(IDictionary<string, object> fields)
        {
            if (fields == null)
                return null;

            if (fields.Count == 0)
                return new string[0];

            var fieldSet = fields
                .Select(f => new { Key = KeyFormatter.Format(f.Key), Value = FieldValueFormatter.Format(f.Value) })
                .Where(f => string.IsNullOrWhiteSpace(f.Key) == false && string.IsNullOrWhiteSpace(f.Value) == false)
                .Select(f => $"{f.Key}={f.Value}")
                .ToArray();

            return fieldSet;
        }
    }
}
