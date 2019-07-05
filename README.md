# Telegraf.Statsd.Client
## Usage

```cs
var uri = new Uri("udp://192.168.56.101:8125");
var channel = new UdpTelegrafChannel(uri);
var client = new TelegrafStatsdClient(channel, new Dictionary<string, string>
{
    {"database_tag", "test_app"}
});

//send time
client.Time("services", 100, t => t.Tag("service", "some-service").Tag("method", "some-method"));
```