# Telegraf.Statsd.Client [![NuGet](https://img.shields.io/nuget/v/Telegraf.Statsd.Client.svg)](https://www.nuget.org/packages/Telegraf.Statsd.Client/) [![Downloads](https://img.shields.io/nuget/dt/Telegraf.Statsd.Client.svg)](https://www.nuget.org/packages/Telegraf.Statsd.Client/) 

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

`Telegraf.Statsd.Client` is built for these target frameworks:

* `net46`
* `netstandard2.0`

## Telegraf.Statsd.Client
add Telegraf.Statsd.Client
```
dotnet add package Telegraf.Statsd.Client
dotnet add package Telegraf.Channel.Udp 
dotnet add package Telegraf.Channel.Tcp 
```

send metrics use udp channel
```cs
var uri = new Uri("udp://192.168.56.101:8125");
var udpChannel = new UdpTelegrafChannel(uri);
var client = new TelegrafStatsdClient(udpChannel, new Dictionary<string, string>
{
    {"database_tag", "test_app"}
});

//send time(100ms)
client.Time("services", 100, t => t.Tag("service", "some-service").Tag("method", "some-method"));
```

send metrics use tcp channel
```cs
var uri = new Uri("tcp://192.168.56.101:8126");
var tcpChannel = new TcpTelegrafChannel(uri);
var client = new TelegrafStatsdClient(tcpChannel, new Dictionary<string, string>
{
    {"database_tag", "test_app"}
});

//send time(100ms)
client.Time("services", 100, t => t.Tag("service", "some-service").Tag("method", "some-method"));
```

## Telegraf.Infux.Client
`Telegraf.Infux.Client` is built for these target frameworks:

* `net46`
* `netstandard2.0`