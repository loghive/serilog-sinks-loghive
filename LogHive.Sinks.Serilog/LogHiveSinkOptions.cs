using Serilog.Events;
using System;

namespace Serilog.Sinks.LogHive
{
    public class LogHiveSinkOptions
    {
        public string? ApiKey = String.Empty;
        public string? ProjectName = String.Empty;
        public string? GroupName = String.Empty;
        public string? Url = String.Empty;
        public bool? ParseMessageTemplateForEventName = true;
        public LogEventLevel? RestrictedToMinimumLevel = LogEventLevel.Error;
        public LogEventLevel? MinimumPushNotificationLevel = LogEventLevel.Fatal;
    }
}