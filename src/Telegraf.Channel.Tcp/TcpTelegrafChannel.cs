using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;
// ReSharper disable RedundantBoolCompare

namespace Telegraf.Channel
{
    public class TcpTelegrafChannel : ITelegrafChannel
    {
        internal const int DefaultTcpPort = 8094;
        internal const string DefaultTcpHost = "127.0.0.1";
        private Socket _socket;
        private readonly IPEndPoint _ipEndPoint;
        private readonly ILogger _logger = LogManager.GetLogger(typeof(TcpTelegrafChannel).FullName);
        private readonly ConcurrentDictionary<Guid, Task> _tasks = new ConcurrentDictionary<Guid, Task>();
        private bool _disposed;
        private bool _disposing;
        private readonly TimeSpan _disposingWaitTaskTimeout;
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public bool SupportsBatchedWrites => true;

        public TcpTelegrafChannel(Uri uri, TimeSpan disposingWaitTaskTimeout)
        {
            _ipEndPoint = ParseEndpoint(uri, DefaultTcpHost, DefaultTcpPort);

            _disposingWaitTaskTimeout = disposingWaitTaskTimeout;

            Reconnect();
        }

        public void Write(string metric)
        {
            if (_disposed || _disposing)
                return;

            if (string.IsNullOrWhiteSpace(metric))
                return;

            metric = metric.EndsWith("\n") ? metric : $"{metric}\n";
            var buffer = Encoding.UTF8.GetBytes(metric);

            var taskId = Guid.NewGuid();

            var task = Task.Factory.StartNew(
                () => WriteInternal(buffer, taskId),
                _cancellationTokenSource.Token,
                TaskCreationOptions.DenyChildAttach,
                TaskScheduler.Default);

            _tasks.TryAdd(taskId, task);
        }

        internal bool EnsureConnection()
        {
            var isConnected = IsConnected();

            if (isConnected)
                return true;

            isConnected = Reconnect();

            return isConnected;
        }

        internal void WriteInternal(byte[] buffer, Guid taskId)
        {
            try
            {
                var isConnected = IsConnected() || EnsureConnection();

                if (isConnected == false)
                    return;

                var socketEventArgs = new SocketAsyncEventArgs { SocketFlags = SocketFlags.None };

                socketEventArgs.SetBuffer(buffer, 0, buffer.Length);

                _socket.SendAsync(socketEventArgs);
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
            finally
            {
                _tasks.TryRemove(taskId, out _);
            }
        }

        private bool IsConnected()
        {
            _lock.EnterReadLock();

            try
            {
                var isConnected = _socket.Connected;

                return isConnected;
            }
            finally
            {
                _lock.ExitReadLock();
            }

        }

        private bool Reconnect()
        {
            _lock.EnterWriteLock();

            try
            {
                var oldSocket = _socket;

                CloseSocket(oldSocket);

                _socket = CreateSocket();

                _socket.ConnectAsync(new SocketAsyncEventArgs
                {
                    RemoteEndPoint = _ipEndPoint
                });

                _socket.Connect(_ipEndPoint);

                return IsConnected();
            }
            catch (Exception e)
            {
                _logger.Error(e);

                return false;
            }
            finally
            {
                _lock.ExitWriteLock();
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

        internal IEnumerable<Task> GetTasks()
        {
            var tasks = _tasks.Where(t => t.Value != null && t.Value.IsCompleted == false).Select(t => t.Value);

            return tasks;
        }

        public void Dispose()
        {
            if (_disposed || _disposing)
                return;

            _disposing = true;

            try
            {
                _logger.Info($"Waiting for completion of {GetTasks().Count()} tasks, maximum waiting time: {_disposingWaitTaskTimeout:g}");

                var tasks = GetTasks().ToArray();

                var completed = Task.WaitAll(tasks, _disposingWaitTaskTimeout);

                if (completed == false)
                    _logger.Warn($"During '{_disposingWaitTaskTimeout:g}' {tasks.Count(t => t.IsCompleted)} from  {tasks.Length} tasks were completed");
                else
                    _logger.Info("All tasks completed");

                _cancellationTokenSource.Cancel();

                CloseSocket(_socket);
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }

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
    }
}
