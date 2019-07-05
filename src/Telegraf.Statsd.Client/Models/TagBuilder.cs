using System.Collections;
using System.Collections.Generic;
using Telegraf.Infrastructure;

namespace Telegraf.Statsd.Models
{
    internal class TagBuilder : ITagBuilder
    {
        private readonly IDictionary<string, string> _tags;

        public TagBuilder()
        {
            _tags = new Dictionary<string, string>();
        }

        public TagBuilder(IDictionary<string, string> tags)
        {
            _tags = tags ?? new Dictionary<string, string>();
        }

        public ITagBuilder Tag(string key, string value)
        {
            _tags[key] = value;

            return this;
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _tags.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
