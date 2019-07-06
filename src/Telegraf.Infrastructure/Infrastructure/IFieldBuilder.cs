using System.Collections.Generic;

namespace Telegraf.Infrastructure
{
    public interface IFieldBuilder : IEnumerable<KeyValuePair<string, object>>
    {
        IFieldBuilder Field(string key, object value);
    }
}
