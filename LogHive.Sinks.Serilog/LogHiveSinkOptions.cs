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
        public LogEventLevel? RestrictedToMinimumLevel = LogEventLevel.Error;
        public LogEventLevel? MinimumPushNotificationLevel = LogEventLevel.Error;
    }
}