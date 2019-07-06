using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Telegraf.Channel.Helpers;

namespace Telegraf.Channel
{
    public class UdpTelegrafChannel: ITelegrafChannel
    {
        internal const int DefaultUdpPort = 8125;
        internal const string DefaultUdpHost = "127.0.0.1";
        private readonly Socket _UDPSocket;
        private readonly int _maxUDPPacketSize;
        private readonly IPEndPoint _ipEndPoint;

        public UdpTelegrafChannel(Uri uri, int maxUDPPacketSize = 512)
        {
            _ipEndPoint = ParseEndpoint(uri, DefaultUdpHost, DefaultUdpPort);

            _UDPSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            _maxUDPPacketSize = maxUDPPacketSize;
        }

        public bool SupportsBatchedWrites => false;

        public void WriteBuffer(Stream stream)
        {
            var buffer = StreamHelper.ReadFully(stream);

            Send(buffer);
        }

        private void Send(byte[] encodedCommand)
        {
            if (_maxUDPPacketSize > 0 && encodedCommand.Length > _maxUDPPacketSize)
            {
                var newline = Encoding.UTF8.GetBytes("\n")[0];

                for (var i = _maxUDPPacketSize; i > 0; i--)
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

            _UDPSocket.SendTo(encodedCommand, encodedCommand.Length, SocketFlags.None, _ipEndPoint);
        }

        public void Dispose()
        {
            _UDPSocket?.Dispose();
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
