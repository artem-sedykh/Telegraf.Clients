# Telegraf.Statsd.Client [![NuGet](https://img.shields.io/nuget/v/Telegraf.Statsd.Client.svg)](https://www.nuget.org/packages/Telegraf.Statsd.Client/) [![Downloads](https://img.shields.io/nuget/dt/Telegraf.Statsd.Client.svg)](https://www.nuget.org/packages/Telegraf.Statsd.Client/) [![Build status](https://ci.appveyor.com/api/projects/status/uvdicwji4o0i9paf?svg=true)](https://ci.appveyor.com/project/artem-sedykh/telegraf-clients/branch/master)
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

## Usage example
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
var tcpChannel = new TcpTelegrafChannel(uri, TimeSpan.FromSeconds(60));
var client = new TelegrafStatsdClient(tcpChannel, new Dictionary<string, string>
{
    {"database_tag", "test_app"}
});

//send time(100ms)
client.Time("services", 100, t => t.Tag("service", "some-service").Tag("method", "some-method"));
```

# Telegraf.Infux.Client [![NuGet](https://img.shields.io/nuget/v/Telegraf.Infux.Client.svg)](https://www.nuget.org/packages/Telegraf.Infux.Client/) [![Downloads](https://img.shields.io/nuget/dt/Telegraf.Infux.Client.svg)](https://www.nuget.org/packages/Telegraf.Infux.Client/) [![Build status](https://ci.appveyor.com/api/projects/status/uvdicwji4o0i9paf?svg=true)](https://ci.appveyor.com/project/artem-sedykh/telegraf-clients/branch/master)

### Influx statistic client

[Telegraf socket_listener documentation](https://github.com/influxdata/telegraf/tree/master/plugins/inputs/socket_listener)

The Socket Listener is a service input plugin that listens for messages from streaming (tcp, unix) or datagram (udp, unixgram) protocols.

A single line of text in line protocol format represents one data point in InfluxDB. It informs InfluxDB of the point’s measurement, tag set, field set, and timestamp. The following code block shows a sample of line protocol and breaks it into its individual components:
```
weather,location=us-midwest temperature=82 1465839830100400200
  |    -------------------- --------------  |
  |             |             |             |
  |             |             |             |
+-----------+--------+-+---------+-+---------+
|measurement|,tag_set| |field_set| |timestamp|
+-----------+--------+-+---------+-+---------+
```
[InfluxDB line protocol tutorial](https://docs.influxdata.com/influxdb/v1.7/write_protocols/line_protocol_tutorial/)
### Supported platforms

`Telegraf.Infux.Client` is built for these target frameworks:

* `net46`
* `netstandard2.0`

## Usage example
add Telegraf.Infux.Client
```
dotnet add package Telegraf.Infux.Client
dotnet add package Telegraf.Channel.Udp 
dotnet add package Telegraf.Channel.Tcp
dotnet add package Telegraf.Channel.Http 

```

send metrics use udp channel
```cs
var uri = new Uri("udp://192.168.56.101:8094");
var udpChannel = new UdpTelegrafChannel(uri);
var client = new TelegrafInfuxClient(udpChannel, new Dictionary<string, string>
{
    {"database_tag", "test_app"}
});

client.Send("weather", f => f.Field("temperature", 82), t => t.Tag("location", "us-midwest"), DateTime.Now);
```

send metrics use tcp channel
```cs
var uri = new Uri("tcp://192.168.56.101:8095");
var tcpChannel = new TcpTelegrafChannel(uri, TimeSpan.FromSeconds(60));
var client = new TelegrafInfuxClient(tcpChannel, new Dictionary<string, string>
{
    {"database_tag", "test_app"}
});

client.Send("weather", f => f.Field("temperature", 82), t => t.Tag("location", "us-midwest"), DateTime.Now);
```

send metrics use http channel
```cs
var uri = new Uri($"http://192.168.56.101:8080/write");
var disposingWaitTaskTimeout = TimeSpan.FromSeconds(30);
var httpTimeout = TimeSpan.FromSeconds(15);

var httpChannel = new HttpTelegrafChanne(uri, disposingWaitTaskTimeou, httpTimeout);

var client = new TelegrafInfuxClient(httpChannel, new Dictionary<string, string>
{
    {"database_tag", "test_app"}
});

client.Send("weather", f => f.Field("temperature", 82), t => t.Tag("location", "us-midwest"), DateTime.Now);
```

send metrics use http channel and auth header
```cs
var uri = new Uri($"http://192.168.56.101:8081/write");
var disposingWaitTaskTimeout = TimeSpan.FromSeconds(30);
var httpTimeout = TimeSpan.FromSeconds(15);

var username = "foobar";
var password = "barfoo";
var encoded = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));

 var authorizationHeaderValue = $"Basic {encoded}";

var httpChannel = new HttpTelegrafChanne(uri, disposingWaitTaskTimeou, httpTimeout, authorizationHeaderValue);

var client = new TelegrafInfuxClient(httpChannel, new Dictionary<string, string>
{
    {"database_tag", "test_app"}
});

client.Send("weather", f => f.Field("temperature", 82), t => t.Tag("location", "us-midwest"), DateTime.Now);
```