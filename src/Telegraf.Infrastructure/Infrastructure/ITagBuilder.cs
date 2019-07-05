using System.Collections.Generic;

namespace Telegraf.Infrastructure
{
    public interface ITagBuilder : IEnumerable<KeyValuePair<string, string>>
    {
        ITagBuilder Tag(string key, string value);
    }
}
