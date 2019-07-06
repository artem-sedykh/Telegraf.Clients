using System.Collections;
using System.Collections.Generic;
using Telegraf.Infrastructure;

namespace Telegraf.Infux.Models
{
    internal class FieldBuilder: IFieldBuilder
    {
        private readonly IDictionary<string, object> _fields;

        public FieldBuilder()
        {
            _fields = new Dictionary<string, object>();
        }

        public FieldBuilder(IDictionary<string, object> fields)
        {
            _fields = fields ?? new Dictionary<string, object>();
        }

        public IFieldBuilder Field(string key, object value)
        {
            _fields[key] = value;

            return this;
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _fields.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
