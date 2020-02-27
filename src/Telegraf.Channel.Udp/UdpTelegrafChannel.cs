using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace Telegraf.Channel
{
    public class UdpTelegrafChannel: ITelegrafChannel
    {
        internal const int DefaultPort = 8125;
        internal const string DefaultHost = "127.0.0.1";
        private readonly Socket _socket;
        private readonly int _maxPacketSize;
        private readonly IPEndPoint _ipEndPoint;
        private bool _isDisposed;

        public UdpTelegrafChannel(Uri uri, int maxPacketSize = 512)
        {
            _ipEndPoint = ParseEndpoint(uri, DefaultHost, DefaultPort);

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);

            _maxPacketSize = maxPacketSize;
        }

        public bool SupportsBatchedWrites => false;

        public void Write(string metric)
        {
            try
            {
                var task = WriteAsync(metric);

                task.Wait();
            }
            catch (AggregateException e)
            {
                var di = ExceptionDispatchInfo.Capture(e.GetBaseException());

                di.Throw();

                throw;
            }
        }

        public async Task WriteAsync(string metric)
        {
            if (string.IsNullOrWhiteSpace(metric))
                return;

            var buffer = Encoding.UTF8.GetBytes(metric);

            await SendAsync(buffer);
        }

        private async Task SendAsync(byte[] encodedCommand)
        {
            if (_maxPacketSize > 0 && encodedCommand.Length > _maxPacketSize)
            {
                var newline = Encoding.UTF8.GetBytes("\n")[0];

                for (var i = _maxPacketSize; i > 0; i--)
                {
                    if (encodedCommand[i] != newline)
                        continue;

                    var encodedCommandFirst = new byte[i];

                    Array.Copy(encodedCommand, encodedCommandFirst, encodedCommandFirst.Length);

                    await SendAsync(encodedCommandFirst);

                    var remainingCharacters = encodedCommand.Length - i - 1;

                    if (remainingCharacters <= 0)
                        return;

                    var encodedCommandSecond = new byte[remainingCharacters];

                    Array.Copy(encodedCommand, i + 1, encodedCommandSecond, 0, encodedCommandSecond.Length);

                    await SendAsync(encodedCommandSecond);

                    return;
                }
            }

            await SendToAsyncInternal(new ArraySegment<byte>(encodedCommand));
        }

        internal Task<int> SendToAsyncInternal(ArraySegment<byte> buffer)
        {
            var tcs = new TaskCompletionSource<int>(this);

            _socket.BeginSendTo(buffer.Array ?? throw new InvalidOperationException(), buffer.Offset, buffer.Count, SocketFlags.None, _ipEndPoint, iar =>
            {
                var innerTcs = (TaskCompletionSource<int>)iar.AsyncState;
                try { innerTcs.TrySetResult(((Socket)innerTcs.Task.AsyncState).EndSendTo(iar)); }
                catch (Exception e) { innerTcs.TrySetException(e); }
            }, tcs);

            return tcs.Task;
        }

        internal static IPEndPoint ParseEndpoint(Uri uri, string defaultHost, int defaultPort)
        {
            var host = uri.Host;
            var port = uri.Port;

            var ipAddress =
                string.IsNullOrWhiteSpace(host) ?
                    GetHostAddress(defaultHost) :
                    GetHostAddress(host);

            if (port == -1)
                port = defaultPort;

            var endpoint = new IPEndPoint(ipAddress, port);
            return endpoint;
        }

        private static IPAddress GetHostAddress(string host)
        {
            var hostAddresses = Dns.GetHostAddresses(host);
            var hostAddress = hostAddresses[0];

            return hostAddress;
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        // The bulk of the clean-up code is implemented in Dispose(bool)
        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            if (disposing)
            {
                // free managed resources
                _socket?.Dispose();
            }

            _isDisposed = true;
        }

        ~UdpTelegrafChannel()
        {
            Dispose(false);
        }
    }
}
