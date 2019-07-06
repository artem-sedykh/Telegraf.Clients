using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NLog;
using Telegraf.Channel.Tcp.Helpers;
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
                var args = new SocketAsyncEventArgs {RemoteEndPoint = _ipEndPoint};

                var oldSocket = _socket;

                CloseSocket(oldSocket);

                _socket = CreateSocket();

                _socket.ConnectAsync(args);
            }
            catch (Exception e)
            {
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

        public void WriteBuffer(Stream stream)
        {
            if(_disposed == true)
                return;

            var encodedCommand = StreamHelper.ReadFully(stream);

            try
            {
                var socketEventArgs = new SocketAsyncEventArgs { SocketFlags = SocketFlags.None };

                socketEventArgs.SetBuffer(encodedCommand, 0, encodedCommand.Length);

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
