using System;
using System.Threading.Tasks;
using Telegraf.Infrastructure;

namespace Telegraf.Statsd.Client.Impl
{
    public class NullTelegrafStatsdClient: ITelegrafStatsdClient
    {
        public void Counter(string measurement, double value, double sample = 1, Func<ITagBuilder, ITagBuilder> builder = null)
        {

        }

        public Task CounterAsync(string measurement, double value, double sample = 1, Func<ITagBuilder, ITagBuilder> builder = null)
        {
            return Task.CompletedTask;
        }

        public void Counter(string measurement, double value, Func<ITagBuilder, ITagBuilder> builder)
        {

        }

        public Task CounterAsync(string measurement, double value, Func<ITagBuilder, ITagBuilder> builder)
        {
            return Task.CompletedTask;
        }

        public void Gauge(string measurement, double value, double sample = 1, Func<ITagBuilder, ITagBuilder> builder = null)
        {

        }

        public Task GaugeAsync(string measurement, double value, double sample = 1, Func<ITagBuilder, ITagBuilder> builder = null)
        {
            return Task.CompletedTask;
        }

        public void Gauge(string measurement, double value, Func<ITagBuilder, ITagBuilder> builder)
        {

        }

        public Task GaugeAsync(string measurement, double value, Func<ITagBuilder, ITagBuilder> builder)
        {
            return Task.CompletedTask;
        }

        public void Time(string measurement, double value, double sample = 1, Func<ITagBuilder, ITagBuilder> builder = null)
        {

        }

        public Task TimeAsync(string measurement, double value, double sample = 1, Func<ITagBuilder, ITagBuilder> builder = null)
        {
            return Task.CompletedTask;
        }

        public void Time(string measurement, double value, Func<ITagBuilder, ITagBuilder> builder)
        {

        }

        public Task TimeAsync(string measurement, double value, Func<ITagBuilder, ITagBuilder> builder)
        {
            return Task.CompletedTask;
        }

        public void Set(string measurement, string value, double sample = 1, Func<ITagBuilder, ITagBuilder> builder = null)
        {

        }

        public Task SetAsync(string measurement, string value, double sample = 1, Func<ITagBuilder, ITagBuilder> builder = null)
        {
            return Task.CompletedTask;
        }

        public void Set(string measurement, string value, Func<ITagBuilder, ITagBuilder> builder)
        {

        }

        public Task SetAsync(string measurement, string value, Func<ITagBuilder, ITagBuilder> builder)
        {
            return Task.CompletedTask;
        }
    }
}
