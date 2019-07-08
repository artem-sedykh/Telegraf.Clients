using System;

namespace Telegraf.Channel
{
    public interface ITelegrafChannel : IDisposable
    {
        bool SupportsBatchedWrites { get; }

        void Write(string metric);
    }
}
