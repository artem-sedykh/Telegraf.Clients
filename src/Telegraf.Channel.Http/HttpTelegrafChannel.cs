using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace Telegraf.Channel
{
    public class HttpTelegrafChannel: ITelegrafChannel
    {
        private readonly Uri _uri;
        private readonly int _httpTimeout;
        private readonly ILogger _logger = LogManager.GetLogger(typeof(HttpTelegrafChannel).FullName);
        private readonly ConcurrentDictionary<Guid, Task> _tasks = new ConcurrentDictionary<Guid, Task>();
        private bool _disposed;
        private bool _disposing;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly TimeSpan _disposingWaitTaskTimeout;
        private readonly string _authorizationHeaderValue;

        public bool SupportsBatchedWrites => true;


        /// <summary>
        /// Http telegraf channel (<a href="https://github.com/Mirantis/telegraf/tree/master/plugins/inputs/http_listener">http_listener</a>)
        /// </summary>
        /// <param name="uri">Url, example http://telegraf:8080/write</param>
        /// <param name="disposingWaitTaskTimeout">Maximum waiting time for scheduled data upload tasks</param>
        /// <param name="httpTimeout">http timeout</param>
        /// <param name="authorizationHeaderValue">Authorization header, example: <example>Basic Zm9vYmFyOmJhcmZvbw==</example></param>
        public HttpTelegrafChannel(
            Uri uri,
            TimeSpan disposingWaitTaskTimeout,
            TimeSpan? httpTimeout = null,
            string authorizationHeaderValue = null)
        {
            _uri = uri ?? throw new ArgumentNullException(nameof(uri));
            httpTimeout = httpTimeout ?? TimeSpan.FromSeconds(15);

            _httpTimeout = (int)httpTimeout.Value.TotalMilliseconds;
            _disposingWaitTaskTimeout = disposingWaitTaskTimeout;
            _authorizationHeaderValue = authorizationHeaderValue;
        }

        public void Write(string metric)
        {
            if(string.IsNullOrWhiteSpace(metric))
                return;

            var taskId = Guid.NewGuid();

            var task = Task.Factory.StartNew(
                () => WriteInternal(metric, taskId),
                _cancellationTokenSource.Token,
                TaskCreationOptions.DenyChildAttach,
                TaskScheduler.Default);

            _tasks.TryAdd(taskId, task);
        }

        internal void WriteInternal(string metric, Guid taskId)
        {
            try
            {
                if (_cancellationTokenSource.IsCancellationRequested)
                    return;

                var httpWebRequest = CreateWebRequest();

                using (var requestStream = httpWebRequest.GetRequestStream())
                {
                    using (var writer = new StreamWriter(requestStream, Encoding.UTF8))
                    {
                        writer.WriteLine(metric);
                    }
                }

                using (var responce = httpWebRequest.GetResponse())
                {
                    using (responce.GetResponseStream()){ }
                }
            }
            catch (WebException e)
            {
                _logger.Error(e, $"status:{e.Status}, metric:{metric}");
            }
            catch (Exception e)
            {
                _logger.Error(e, $"metric:{metric}");
            }
            finally
            {
                _tasks.TryRemove(taskId, out _);
            }
        }

        private WebRequest CreateWebRequest()
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(_uri);
            httpWebRequest.Method = "POST";
            httpWebRequest.MediaType = "application/octet-stream";
            httpWebRequest.Timeout = _httpTimeout;

            if (string.IsNullOrWhiteSpace(_authorizationHeaderValue) == false)
                httpWebRequest.Headers.Add("Authorization", _authorizationHeaderValue);

            return httpWebRequest;
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
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }

            _disposed = true;
        }
    }
}
