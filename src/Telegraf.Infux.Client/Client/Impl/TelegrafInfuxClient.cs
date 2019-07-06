﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Telegraf.Channel;
using Telegraf.Infrastructure;
using Telegraf.Infux.Models;

namespace Telegraf.Infux.Client.Impl
{
    public class TelegrafInfuxClient: ITelegrafInfuxClient
    {
        private readonly ITelegrafChannel _channel;
        private readonly IDictionary<string, string> _tags;

        public TelegrafInfuxClient(ITelegrafChannel channel, IDictionary<string, string> tags)
        {
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));
            _tags = tags ?? new Dictionary<string, string>();
        }

        public void Send(string measurement, Func<IFieldBuilder, IFieldBuilder> fieldBuilder, Func<ITagBuilder, ITagBuilder> tabBuilder, DateTime? timestamp = null)
        {
            if (fieldBuilder == null)
                throw new ArgumentNullException(nameof(fieldBuilder));

            var tags = BuildTags(tabBuilder);
            var fields = fieldBuilder(new FieldBuilder()).ToDictionary(f => f.Key, f => f.Value);

            var point = new InfluxPoint(measurement, fields, tags, timestamp);

            Publish(point);
        }

        public Task SendAsync(string measurement, Func<IFieldBuilder, IFieldBuilder> fieldBuilder, Func<ITagBuilder, ITagBuilder> tabBuilder, DateTime? timestamp = null)
        {
            return Task.Factory.StartNew(() => Send(measurement, fieldBuilder, tabBuilder, timestamp));
        }

        internal void Publish(InfluxPoint point)
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            {
                point.Format(writer);

                writer.Flush();

                _channel.WriteBuffer(stream);
            }
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
    }
}
