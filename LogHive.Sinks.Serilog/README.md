# Serilog.Sinks.LogHive
This C# library is a sink for the Serilog Logging Framework. With this extension, all log events that occur are automatically transmitted to the event and log service LogHive.
LogHive is a log and events service specially designed for software applications such as apps, websites or services. 
With LogHive, push notifications can be sent to the web browser or to a mobile device, or the log messages can be analyzed in a separate dashboard.

Checkout the NuGet Package: [![NuGet](https://img.shields.io/nuget/v/Serilog.Sinks.LogHive.svg)](https://www.nuget.org/packages/Serilog.Sinks.LogHive)

Licence: MIT

## Compatibility
The library is written in:
[![.NET Version](https://img.shields.io/badge/.NET6.0-blue)](https://shields.io/)
[![.NET Version](https://img.shields.io/badge/.NETStandard2.1-blue)](https://shields.io/)

# Contents
1. [General](#general)
2. [Requirements](#requirements)
3. [Documentation](#documentation)

## General
To ensure that the Serilog extension is found, you should include the namespace Serilog.Sinks.LogHive in your Usings section.
```json
"Serilog": {
    "Using": [
      "Serilog",
      "Serilog.Sinks.Console",
      "Serilog.Sinks.LogHive"
    ],
    "MinimumLevel": {
      "Default": "Warning"
    },
    "WriteTo": [
      {
        "Name": "Console"
      },
      {
        "Name": "LogHiveSink",
        "Args": {
          "ApiKey": "your-api-key",
          "ProjectName": "yourprojectname",
          "GroupName": "yourgroupname",
          "ParseMessageTemplateForEventName": false,
          "RestrictedToMinimumLevel": "Error",
          "MinimumPushNotificationLevel": "Error"
        }
      }    
    ]
  }
```
ApiKey: your personal api key
ProjectName: your LogHive project name
GroupName: a group name for the events (e.g. bugs)
ParseMessageTemplateForEventName: parse message template to full string without property placeholders (default: false)
RestrictedToMinimumLevel: minimum Log Level to push the event to LogHive
MinimumPushNotificationLevel: minimum Log Level to push a notification

You can also instantiate logging directly through the code:
```c#
var log = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.LogHiveSink("your-api-key", "yourprojectname", "yourgroupname",
                LogEventLevel.Information, LogEventLevel.Information)
                .CreateLogger();

var position = new { Latitude = 25, Longitude = 134 };
var elapsedMs = 34;

log.Information("Processed {@Position} in {Elapsed:000} ms.", position, elapsedMs);
```

## Requirements
To use this API you need an API key.  You can register for a free API key at [https://app.loghive.app](https://app.loghive.app).

## Documentation
A full documentation is available under [https://docs.loghive.app/](https://docs.loghive.app/).