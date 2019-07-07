using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Telegraf.Channel
{
    public class UdpTelegrafChannel: ITelegrafChannel
    {
        internal const int DefaultPort = 8125;
        internal const string DefaultHost = "127.0.0.1";
        private readonly Socket _socket;
        private readonly int _maxPacketSize;
        private readonly IPEndPoint _ipEndPoint;

        public UdpTelegrafChannel(Uri uri, int maxPacketSize = 512)
        {
            _ipEndPoint = ParseEndpoint(uri, DefaultHost, DefaultPort);

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
            {
                ExclusiveAddressUse = false
            };

            _maxPacketSize = maxPacketSize;
        }

        public bool SupportsBatchedWrites => false;

        public void Write(string metric)
        {
            if (string.IsNullOrWhiteSpace(metric))
                return;

            var buffer = Encoding.UTF8.GetBytes(metric);

            Send(buffer);
        }

        private void Send(byte[] encodedCommand)
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

                    Send(encodedCommandFirst);

                    var remainingCharacters = encodedCommand.Length - i - 1;

                    if (remainingCharacters <= 0)
                        return;

                    var encodedCommandSecond = new byte[remainingCharacters];

                    Array.Copy(encodedCommand, i + 1, encodedCommandSecond, 0, encodedCommandSecond.Length);

                    Send(encodedCommandSecond);

                    return;
                }
            }

            _socket.SendTo(encodedCommand, encodedCommand.Length, SocketFlags.None, _ipEndPoint);
        }

        public void Dispose()
        {
            _socket?.Dispose();
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
    }
}
