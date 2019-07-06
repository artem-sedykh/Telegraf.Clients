using System;
using System.Collections.Generic;
using System.Globalization;

namespace Telegraf.Formatters
{
    public class FieldValueFormatter
    {
        private static readonly DateTime UtcTimestamp = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private static readonly Dictionary<Type, Func<object, string>> Formatters = new Dictionary<Type, Func<object, string>>
        {
            { typeof(sbyte), FormatInteger },
            { typeof(byte), FormatInteger },
            { typeof(short), FormatInteger },
            { typeof(ushort), FormatInteger },
            { typeof(int), FormatInteger },
            { typeof(uint), FormatInteger },
            { typeof(long), FormatInteger },
            { typeof(ulong), FormatInteger },
            { typeof(float), FormatFloat },
            { typeof(double), FormatFloat },
            { typeof(decimal), FormatFloat },
            { typeof(bool), FormatBoolean },
            { typeof(TimeSpan), FormatTimespan }
        };

        public static string Format(object value)
        {
            var v = value ?? string.Empty;

            if (Formatters.TryGetValue(v.GetType(), out var format))
                return format(v);

            return FormatString(v.ToString().Trim());
        }

        private static string FormatInteger(object i)
        {
            return ((IFormattable)i).ToString(null, CultureInfo.InvariantCulture) + "i";
        }

        private static string FormatFloat(object f)
        {
            return ((IFormattable)f).ToString(null, CultureInfo.InvariantCulture);
        }

        private static string FormatTimespan(object ts)
        {
            return ((TimeSpan)ts).TotalMilliseconds.ToString(CultureInfo.InvariantCulture);
        }

        private static string FormatBoolean(object b)
        {
            return (bool)b ? "t" : "f";
        }

        private static string FormatString(string stringValue)
        {
            stringValue = stringValue.Replace("'", "\'");

            stringValue = stringValue.Replace("\"", "\\\"");

            return $"\"{stringValue}\"";
        }

        public static string FormatTimestamp(DateTime? timestamp)
        {
            if (timestamp == null)
                return null;

            var t = timestamp.Value - UtcTimestamp;

            return (t.Ticks * 100L).ToString(CultureInfo.InvariantCulture);
        }
    }
}
