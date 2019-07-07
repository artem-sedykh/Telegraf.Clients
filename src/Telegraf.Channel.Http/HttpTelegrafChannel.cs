using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace Telegraf.Channel
{
    public class HttpTelegrafChannel: ITelegrafChannel
    {
        private readonly Uri _uri;
        private readonly int _httpTimeout;
        private readonly ILogger _logger = LogManager.GetLogger(typeof(HttpTelegrafChannel).FullName);
        private readonly IList<Task> _tasks = new List<Task>();
        private readonly object _lockObj = new object();
        private bool _disposed;
        private bool _disposing;
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
            var task = WriteAsync(metric);

            task.ContinueWith(_ =>
            {
                lock (_lockObj)
                {
                    _tasks.Remove(task);
                }
            });

            lock (_lockObj)
            {
                _tasks.Add(task);
            }
        }

        internal async Task WriteAsync(string metric)
        {
            if (_disposed || _disposing)
                return;

            if (string.IsNullOrWhiteSpace(metric))
                return;

            var httpWebRequest = CreateWebRequest();

            using (var requestStream = await Task.Factory.FromAsync(httpWebRequest.BeginGetRequestStream, httpWebRequest.EndGetRequestStream, httpWebRequest))
            {
                using (var writer = new StreamWriter(requestStream, Encoding.UTF8))
                {
                    await writer.WriteLineAsync(metric);
                }
            }

            try
            {
                using (var webResponse = await Task.Factory.FromAsync(httpWebRequest.BeginGetResponse, httpWebRequest.EndGetResponse, httpWebRequest))
                {
                    using (webResponse.GetResponseStream())
                    {
                        _logger.Trace($"metric: {metric} sent");
                    }
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

        public void Dispose()
        {
            if (_disposed || _disposing)
                return;

            _disposing = true;

            try
            {
                var tasks = _tasks.Where(t => t != null && t.IsCompleted == false).ToArray();

                _logger.Info($"Waiting for completion of {tasks.Length} tasks, maximum waiting time: {_disposingWaitTaskTimeout:g}");

                var completed = Task.WaitAll(_tasks.ToArray(), _disposingWaitTaskTimeout);

                if (completed == false)
                    _logger.Warn($"During '{_disposingWaitTaskTimeout:g}' {tasks.Count(t => t.IsCompleted)} from  {tasks.Length} tasks were completed");
                else
                    _logger.Info("All tasks completed");
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }

            _disposed = true;
        }
    }
}
