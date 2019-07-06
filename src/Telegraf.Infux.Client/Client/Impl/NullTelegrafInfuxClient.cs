using System;
using System.Threading.Tasks;
using Telegraf.Infrastructure;

namespace Telegraf.Infux.Client.Impl
{
    public class NullTelegrafInfuxClient : ITelegrafInfuxClient
    {
        public void Send(string measurement, Func<IFieldBuilder, IFieldBuilder> fieldBuilder, Func<ITagBuilder, ITagBuilder> tabBuilder, DateTime? timestamp)
        {

        }

        public Task SendAsync(string measurement, Func<IFieldBuilder, IFieldBuilder> fieldBuilder, Func<ITagBuilder, ITagBuilder> tabBuilder, DateTime? timestamp)
        {
            return Task.CompletedTask;
        }
    }
}
