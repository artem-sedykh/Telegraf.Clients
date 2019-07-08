using System;
using System.Collections.Generic;
using Telegraf.Channel;
using Telegraf.Statsd.Client.Impl;

namespace StatsdExamples
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = "192.168.56.101";

            //configure https://github.com/influxdata/telegraf/tree/master/plugins/inputs/statsd
            //# Statsd Server
            //[[inputs.statsd]]
            //## Protocol, must be "tcp", "udp4", "udp6" or "udp" (default=udp)
            //protocol = "udp"
            //## Address and port to host UDP listener on
            //service_address = ":8125"

            SendMetricsWithUdpChannel(host);

            //configure https://github.com/influxdata/telegraf/tree/master/plugins/inputs/statsd
            //# Statsd Server
            //[[inputs.statsd]]
            //## Protocol, must be "tcp", "udp4", "udp6" or "udp" (default=udp)
            //protocol = "tcp"
            //## Address and port to host UDP listener on
            //service_address = ":8126"

            SendMetricsWithTcpChannel(host);
        }

        private static void SendMetricsWithUdpChannel(string host)
        {
            var uri = new Uri($"udp://{host}:8125");

            using (var channel = new UdpTelegrafChannel(uri))
            {
                //[[outputs.influxdb]]
                //## The value of this tag will be used to determine the database.  If this
                //## tag is not set the 'database' option is used as the default.
                //# database_tag = ""
                // see more https://github.com/influxdata/telegraf/tree/master/plugins/outputs/influxdb
                var defaultTags = new Dictionary<string, string>
                {
                    { "database_tag", "statsd_client_udp_cannel" },
                    { "host", Environment.MachineName }
                };

                var client = new TelegrafStatsdClient(channel, defaultTags);

                client.Time("deploys", 200, t => t.Tag("stage", "test").Tag("application", "my-app"));
            }
        }

        private static void SendMetricsWithTcpChannel(string host)
        {
            var uri = new Uri($"tcp://{host}:8126");

            using (var channel = new TcpTelegrafChannel(uri, TimeSpan.FromSeconds(60)))
            {
                //[[outputs.influxdb]]
                //## The value of this tag will be used to determine the database.  If this
                //## tag is not set the 'database' option is used as the default.
                //# database_tag = ""
                // see more https://github.com/influxdata/telegraf/tree/master/plugins/outputs/influxdb
                var defaultTags = new Dictionary<string, string>
                {
                    {"database_tag", "statsd_client_tcp_cannel"},
                    {"host", Environment.MachineName}
                };

                var client = new TelegrafStatsdClient(channel, defaultTags);

                client.Time("deploys", 200, t => t.Tag("stage", "test").Tag("application", "my-app"));
            }
        }
    }
}
