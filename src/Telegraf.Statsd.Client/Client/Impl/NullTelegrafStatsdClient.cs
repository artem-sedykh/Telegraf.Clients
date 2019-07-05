using System;
using Telegraf.Infrastructure;

namespace Telegraf.Statsd.Client.Impl
{
    public class NullTelegrafStatsdClient: ITelegrafStatsdClient
    {
        public void Counter(string measurement, double value, double sample = 1, Func<ITagBuilder, ITagBuilder> builder = null)
        {

        }

        public void Counter(string measurement, double value, Func<ITagBuilder, ITagBuilder> builder)
        {

        }

        public void Gauge(string measurement, double value, double sample = 1, Func<ITagBuilder, ITagBuilder> builder = null)
        {

        }

        public void Gauge(string measurement, double value, Func<ITagBuilder, ITagBuilder> builder)
        {

        }

        public void Time(string measurement, double value, double sample = 1, Func<ITagBuilder, ITagBuilder> builder = null)
        {

        }

        public void Time(string measurement, double value, Func<ITagBuilder, ITagBuilder> builder)
        {

        }

        public void Set(string measurement, string value, double sample = 1, Func<ITagBuilder, ITagBuilder> builder = null)
        {

        }

        public void Set(string measurement, string value, Func<ITagBuilder, ITagBuilder> builder)
        {

        }
    }
}
