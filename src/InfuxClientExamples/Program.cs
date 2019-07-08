using System;
using System.Collections.Generic;
using System.Text;
using Telegraf.Channel;
using Telegraf.Infux.Client.Impl;

namespace InfuxClientExamples
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = "192.168.56.101";

            SendMetricsWithUdpChannel(host);

            SendMetricsWithTcpChannel(host);

            SendMetricsWithHttpChannel(host);

            SendMetricsWithHttpChannelAndAuth(host);
        }

        private static void SendMetricsWithUdpChannel(string host)
        {
            var uri = new Uri($"udp://{host}:8094");

            using (var channel = new UdpTelegrafChannel(uri))
            {
                //[[outputs.influxdb]]
                //## The value of this tag will be used to determine the database.  If this
                //## tag is not set the 'database' option is used as the default.
                //# database_tag = ""
                // see more https://github.com/influxdata/telegraf/tree/master/plugins/outputs/influxdb
                var defaultTags = new Dictionary<string, string>
                {
                    { "database_tag", "influx_client_udp_cannel" },
                    { "host", Environment.MachineName }
                };

                var client = new TelegrafInfuxClient(channel, defaultTags);

                client.Send("weather", f => f.Field("temperature", 82), t => t.Tag("location", "us-midwest"), DateTime.Now);
            }
        }

        private static void SendMetricsWithTcpChannel(string host)
        {
            var uri = new Uri($"tcp://{host}:8095");

            using (var channel = new TcpTelegrafChannel(uri, TimeSpan.FromSeconds(60)))
            {
                //[[outputs.influxdb]]
                //## The value of this tag will be used to determine the database.  If this
                //## tag is not set the 'database' option is used as the default.
                //# database_tag = ""
                // see more https://github.com/influxdata/telegraf/tree/master/plugins/outputs/influxdb
                var defaultTags = new Dictionary<string, string>
                {
                    {"database_tag", "influx_client_tcp_cannel"},
                    {"host", Environment.MachineName}
                };

                var client = new TelegrafInfuxClient(channel, defaultTags);

                client.Send("weather", f => f.Field("temperature", 82), t => t.Tag("location", "us-midwest"), DateTime.Now);
            }
        }

        private static void SendMetricsWithHttpChannel(string host)
        {
            var uri = new Uri($"http://{host}:8080/write");

            using (var channel = new HttpTelegrafChannel(uri, TimeSpan.FromSeconds(30)))
            {
                //[[outputs.influxdb]]
                //## The value of this tag will be used to determine the database. If this
                //## tag is not set the 'database' option is used as the default.
                //# database_tag = ""
                // see more https://github.com/influxdata/telegraf/tree/master/plugins/outputs/influxdb
                var defaultTags = new Dictionary<string, string>
                {
                    {"database_tag", "influx_client_http_cannel"},
                    {"host", Environment.MachineName}
                };

                var client = new TelegrafInfuxClient(channel, defaultTags);

                client.Send("weather", f => f.Field("temperature", 82), t => t.Tag("location", "us-midwest"), DateTime.Now);
            }
        }

        private static void SendMetricsWithHttpChannelAndAuth(string host)
        {
            var username = "foobar";
            var password = "barfoo";
            var encoded = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));

            var authorizationHeaderValue = $"Basic {encoded}";

            var uri = new Uri($"http://{host}:8081/write");

            using (var channel = new HttpTelegrafChannel(
                uri, 
                TimeSpan.FromSeconds(30),
                authorizationHeaderValue: authorizationHeaderValue))
            {
                //[[outputs.influxdb]]
                //## The value of this tag will be used to determine the database. If this
                //## tag is not set the 'database' option is used as the default.
                //# database_tag = ""
                // see more https://github.com/influxdata/telegraf/tree/master/plugins/outputs/influxdb
                var defaultTags = new Dictionary<string, string>
                {
                    {"database_tag", "influx_client_http_cannel_and_auth"},
                    {"host", Environment.MachineName}
                };

                var client = new TelegrafInfuxClient(channel, defaultTags);

                client.Send("weather", f => f.Field("temperature", 82), t => t.Tag("location", "us-midwest"), DateTime.Now);
            }
        }
    }
}
