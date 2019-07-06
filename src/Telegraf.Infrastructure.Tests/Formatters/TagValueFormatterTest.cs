using NUnit.Framework;
using Telegraf.Formatters;

namespace Telegraf.Infrastructure.Tests.Formatters
{
    [TestFixture]
    public class TagValueFormatterTest
    {
        [TestCase("  ", ExpectedResult = null)]
        [TestCase(null, ExpectedResult = null)]
        [TestCase("weather\n", ExpectedResult = "weather")]
        [TestCase("weather\t", ExpectedResult = "weather")]
        [TestCase("weather\v", ExpectedResult = "weather")]
        [TestCase("   weather  ", ExpectedResult = "weather")]
        [TestCase("us,midwest", ExpectedResult = "us\\,midwest")]
        [TestCase("us midwest", ExpectedResult = "us\\ midwest")]
        [TestCase("us=midwest", ExpectedResult = "us\\=midwest")]
        public string Format(string value)
        {
            return TagValueFormatter.Format(value);
        }
    }
}
