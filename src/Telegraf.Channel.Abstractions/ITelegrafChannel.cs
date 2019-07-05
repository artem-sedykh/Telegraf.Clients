using System;
using System.IO;

namespace Telegraf.Channel
{
    public interface ITelegrafChannel : IDisposable
    {
        bool SupportsBatchedWrites { get; }

        void WriteBuffer(Stream stream);
    }
}
