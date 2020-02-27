using System;
using System.Threading.Tasks;

namespace Telegraf.Channel
{
    public interface ITelegrafChannel : IDisposable
    {
        bool SupportsBatchedWrites { get; }

        void Write(string metric);


        Task WriteAsync(string metric);
    }
}
