using System;
using System.Threading.Tasks;
using Telegraf.Infrastructure;

namespace Telegraf.Infux.Client
{
    public interface ITelegrafInfuxClient
    {
        void Send(string measurement, Func<IFieldBuilder, IFieldBuilder> fieldBuilder, Func<ITagBuilder, ITagBuilder> tabBuilder, DateTime? timestamp = null);

        Task SendAsync(string measurement, Func<IFieldBuilder, IFieldBuilder> fieldBuilder, Func<ITagBuilder, ITagBuilder> tabBuilder, DateTime? timestamp = null);
    }
}
