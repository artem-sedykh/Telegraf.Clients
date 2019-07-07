using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using NLog;
// ReSharper disable RedundantBoolCompare

namespace Telegraf.Channel
{
    public class TcpTelegrafChannel:ITelegrafChannel
    {
        internal const int DefaultTcpPort = 8094;
        internal const string DefaultTcpHost = "127.0.0.1";
        private Socket _socket;
        private readonly IPEndPoint _ipEndPoint;
        private readonly object _syncObj = new object();
        private readonly Timer _timer;
        private bool _disposed;
        private readonly ILogger _logger = LogManager.GetLogger(typeof(TcpTelegrafChannel).FullName);

        public bool SupportsBatchedWrites => true;

        public TcpTelegrafChannel(Uri uri)
        {
            _ipEndPoint = ParseEndpoint(uri, DefaultTcpHost, DefaultTcpPort);

            Reconnect();

            _timer = new Timer(obj => CheckConnection(), null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
        }

        public void Write(string metric)
        {
            if (_disposed == true)
                return;

            if (string.IsNullOrWhiteSpace(metric))
                return;

            metric = metric.EndsWith("\n") ? metric : $"{metric}\n";
            var buffer = Encoding.UTF8.GetBytes(metric);

            try
            {
                var socketEventArgs = new SocketAsyncEventArgs { SocketFlags = SocketFlags.None };

                socketEventArgs.SetBuffer(buffer, 0, buffer.Length);

                lock (_syncObj)
                {
                    if (IsConnected() == false)
                    {
                        _logger.Warn("Connection is broken, data lost");

                        return;
                    }

                    _socket.SendAsync(socketEventArgs);
                }
            }
            catch (Exception e)
            {
                // ReSharper disable once InconsistentlySynchronizedField
                _logger.Error(e);
            }
        }

        public void Dispose()
        {
            if (_disposed == true)
                return;

            _timer.Dispose();

            CloseSocket(_socket);

            _disposed = true;
        }

        private Socket CreateSocket()
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            {
                ExclusiveAddressUse = false
            };

            return socket;
        }

        private void CheckConnection()
        {
            lock (_syncObj)
            {
                if(IsConnected())
                    return;

                if (_socket != null)
                    _logger.Warn("Connection is broken, reconnect...");

                Reconnect();
            }
        }

        private bool IsConnected()
        {
            var isConnected = _socket.Connected;

            return isConnected;
        }

        private void Reconnect()
        {
            try
            {
                var oldSocket = _socket;

                CloseSocket(oldSocket);

                _socket = CreateSocket();

                _socket.Connect(_ipEndPoint);
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
        }

        private static void CloseSocket(Socket socket)
        {
            socket?.Dispose();
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
