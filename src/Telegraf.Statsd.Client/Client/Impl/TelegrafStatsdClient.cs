using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegraf.Channel;
using Telegraf.Infrastructure;
using Telegraf.Statsd.Models;
using Telegraf.Statsd.Serializer;

namespace Telegraf.Statsd.Client.Impl
{
    public class TelegrafStatsdClient:ITelegrafStatsdClient
    {
        private readonly ITelegrafChannel _channel;
        private readonly IDictionary<string, string> _tags;
        private readonly MetricSerializer _metricSerializer = new MetricSerializer();
        private static readonly Random Sampler = new Random();

        public TelegrafStatsdClient(ITelegrafChannel channel, IDictionary<string,string> tags)
        {
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));
            _tags = tags ?? new Dictionary<string, string>();
        }

        public void Counter(string measurement, double value, double sample = 1, Func<ITagBuilder, ITagBuilder> builder = null)
        {
            var tags = BuildTags(builder);

            Publish(Metric.Counter(measurement, value, sample, tags));
        }

        public async Task CounterAsync(string measurement, double value, double sample = 1, Func<ITagBuilder, ITagBuilder> builder = null)
        {
            var tags = BuildTags(builder);

            await PublishAsync(Metric.Counter(measurement, value, sample, tags));
        }

        public void Counter(string measurement, double value, Func<ITagBuilder, ITagBuilder> builder)
        {
            Counter(measurement, value, 1, builder);
        }

        public async Task CounterAsync(string measurement, double value, Func<ITagBuilder, ITagBuilder> builder)
        {
            await CounterAsync(measurement, value, 1, builder);
        }

        public void Gauge(string measurement, double value, double sample = 1, Func<ITagBuilder, ITagBuilder> builder = null)
        {
            var tags = BuildTags(builder);

            Publish(Metric.Gauge(measurement, value, sample, tags));
        }

        public async Task GaugeAsync(string measurement, double value, double sample = 1, Func<ITagBuilder, ITagBuilder> builder = null)
        {
            var tags = BuildTags(builder);

            await PublishAsync(Metric.Gauge(measurement, value, sample, tags));
        }

        public void Gauge(string measurement, double value, Func<ITagBuilder, ITagBuilder> builder)
        {
            Gauge(measurement, value, 1D, builder);
        }

        public async Task GaugeAsync(string measurement, double value, Func<ITagBuilder, ITagBuilder> builder)
        {
            await GaugeAsync(measurement, value, 1D, builder);
        }

        public void Time(string measurement, double value, double sample = 1, Func<ITagBuilder, ITagBuilder> builder = null)
        {
            var tags = BuildTags(builder);

            Publish(Metric.Time(measurement, value, sample, tags));
        }

        public async Task TimeAsync(string measurement, double value, double sample = 1, Func<ITagBuilder, ITagBuilder> builder = null)
        {
            var tags = BuildTags(builder);

            await PublishAsync(Metric.Time(measurement, value, sample, tags));
        }

        public void Time(string measurement, double value, Func<ITagBuilder, ITagBuilder> builder)
        {
            Time(measurement, value, 1D, builder);
        }

        public async Task TimeAsync(string measurement, double value, Func<ITagBuilder, ITagBuilder> builder)
        {
            await TimeAsync(measurement, value, 1D, builder);
        }

        public void Set(string measurement, string value, double sample = 1, Func<ITagBuilder, ITagBuilder> builder = null)
        {
            var tags = BuildTags(builder);

            Publish(Metric.Set(measurement, value, sample, tags));
        }

        public async Task SetAsync(string measurement, string value, double sample = 1, Func<ITagBuilder, ITagBuilder> builder = null)
        {
            var tags = BuildTags(builder);

            await PublishAsync(Metric.Set(measurement, value, sample, tags));
        }

        public void Set(string measurement, string value, Func<ITagBuilder, ITagBuilder> builder)
        {
            Set(measurement, value, 1D, builder);
        }

        public async Task SetAsync(string measurement, string value, Func<ITagBuilder, ITagBuilder> builder)
        {
            await SetAsync(measurement, value, 1D, builder);
        }

        private IDictionary<string, string> BuildTags(Func<ITagBuilder, ITagBuilder> builder)
        {
            if (builder == null)
                return _tags.ToDictionary(t => t.Key, t => t.Value);

            var tagBuilder = builder(new TagBuilder());

            var result = tagBuilder.ToDictionary(t => t.Key, t => t.Value);

            foreach (var key in _tags.Keys)
            {
                result[key] = _tags[key];
            }

            return result;
        }

        internal void Publish(Metric metric)
        {
            if (metric.Sample < 1 && metric.Sample < Sampler.NextDouble())
                return;

            var payload = _metricSerializer.SerializeMetric(metric);

            _channel.Write(payload);
        }

        internal async Task PublishAsync(Metric metric)
        {
            if (metric.Sample < 1 && metric.Sample < Sampler.NextDouble())
                return;

            var payload = _metricSerializer.SerializeMetric(metric);

            await _channel.WriteAsync(payload);
        }
    }
}
