using System;
using System.IO;

namespace Telegraf.Channel.Tcp.Helpers
{
    internal static class StreamHelper
    {
        public static byte[] ReadFully(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            stream.Seek(0, SeekOrigin.Begin);

            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);

                return ms.ToArray();
            }
        }
    }
}
