using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Telegraf.Channel.Udp.Tests
{
    [TestFixture]
    public class UdpTelegrafChannelTest:IDisposable
    {
        private readonly ITelegrafChannel _channel;
        private readonly UdpClient _receivingUdpClient;
        private IPEndPoint _ipEndPoint;

        private readonly int _port;

        public UdpTelegrafChannelTest()
        {
            _port = 8125;

            _channel = new UdpTelegrafChannel(new Uri($"udp://127.0.0.1:{_port}"));
            _receivingUdpClient = new UdpClient(_port);
            _ipEndPoint = new IPEndPoint(IPAddress.Any, _port);
        }

        [TestCase("test_metric_sended_async")]
        public async Task WriteAsync(string metric)
        {
            await _channel.WriteAsync(metric);

            var receiveBytes = _receivingUdpClient.Receive(ref _ipEndPoint);

            var returnData = Encoding.ASCII.GetString(receiveBytes);

            Assert.AreEqual(metric,returnData);
        }

        [TestCase("test_metric_sended_sync")]
        public void Write(string metric)
        {
            _channel.Write(metric);

            var receiveBytes = _receivingUdpClient.Receive(ref _ipEndPoint);

            var returnData = Encoding.ASCII.GetString(receiveBytes);

            Assert.AreEqual(metric, returnData);
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _receivingUdpClient?.Dispose();
        }
    }
}