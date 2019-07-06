using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Telegraf.Formatters
{
    public static class KeyFormatter
    {
        private static readonly Regex CleanRegex;

        static KeyFormatter()
        {
            var characters = new HashSet<char> { ':', '|', '@', '\n', '\r', '\t', ':', '|' };
            var ingnoreCharacters = new[] { '\\' };

            foreach (var invalidChar in Path.GetInvalidFileNameChars().Union(Path.GetInvalidPathChars()).Where(c => ingnoreCharacters.Contains(c) == false))
                characters.Add(invalidChar);

            var pattern = $"[{string.Join("", characters.Select(c => Regex.Escape(c.ToString(CultureInfo.InvariantCulture))))}]";

            CleanRegex = new Regex(pattern, RegexOptions.Compiled);
        }

        public static string Format(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            value = value.Trim().ToLowerInvariant();

            //wea\ ther,location=us-midwest temperature=82 1465839830100400200
            value = value.Replace(" ", "\\ ");

            //wea\,ther,location=us-midwest temperature=82 1465839830100400200
            value = value.Replace(",", "\\,");

            //weather,location=us-midwest temp\=rature=82 1465839830100400200
            value = value.Replace("=", "\\=");

            value = CleanRegex.Replace(value, string.Empty);

            return value;
        }
    }
}
