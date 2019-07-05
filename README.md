# Telegraf.Statsd.Client

### Influx Statsd

[Telegraf Statsd documentation](https://github.com/influxdata/telegraf/tree/master/plugins/inputs/statsd)

In order to take advantage of InfluxDB's tagging system, we have made a couple
additions to the standard statsd protocol. First, you can specify
tags in a manner similar to the line-protocol, like this:
```
users.current,reg=us-w:32|g
|-----------|--------|-----|
|           |        |     |
|           |        |     |
+-----------+--------+-+---+
|measurement|,tag_set|:data|
+-----------+--------+-+---+
```

### Supported platforms

`Telegraf.Statsd.Client` version 4 is built for these target frameworks:

* `net46`
* `netstandard2.0`

## Usage example
add Telegraf.Statsd.Client
```
dotnet add package Telegraf.Statsd.Client
dotnet add package Telegraf.Channel.Udp 
```

```cs
var uri = new Uri("udp://192.168.56.101:8125");
var channel = new UdpTelegrafChannel(uri);
var client = new TelegrafStatsdClient(channel, new Dictionary<string, string>
{
    {"database_tag", "test_app"}
});

//send time(100ms)
client.Time("services", 100, t => t.Tag("service", "some-service").Tag("method", "some-method"));
```