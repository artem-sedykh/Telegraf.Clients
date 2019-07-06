using NUnit.Framework;
using Telegraf.Formatters;

namespace Telegraf.Infrastructure.Tests.Formatters
{
    [TestFixture]
    public class KeyFormatterTest
    {
        [TestCase("   weather  ", ExpectedResult = "weather")]
        [TestCase("wea ther", ExpectedResult = "wea\\ ther")]
        [TestCase("wea,ther", ExpectedResult = "wea\\,ther")]
        [TestCase("temp=rature", ExpectedResult = "temp\\=rature")]
        [TestCase("weather\n", ExpectedResult = "weather")]
        [TestCase("weather\t", ExpectedResult = "weather")]
        [TestCase("weather\v", ExpectedResult = "weather")]
        [TestCase("  ", ExpectedResult = null)]
        [TestCase(null, ExpectedResult = null)]
        public string Format(string key)
        {
            return KeyFormatter.Format(key);
        }
    }
}
