using LogHive.SDK.CSharp;
using Newtonsoft.Json;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Debugging;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Serilog.Sinks.LogHive
{
    public class LogHiveSink : ILogEventSink
    {
        private readonly LogHiveSinkOptions _options;
        private readonly LogHiveApi _logger;
        private const string logHiveUrl = "https://api.loghive.app/v1";

        public LogHiveSink(LogHiveSinkOptions options)
        {
            _options = options;

            if (_options.Url != null)
            {
                if (_options.Url == "")
                {
                    _options.Url = logHiveUrl;
                }
            }
            if (_options.ProjectName == null)
            {
                if (_options.ProjectName == "")
                {
                    SelfLog.WriteLine("No LogHive Project found in Appsettings.");
                }
            }
            if (_options.GroupName == null)
            {
                if (_options.GroupName == "")
                {
                    SelfLog.WriteLine("No LogHive Group found in Appsettings.");
                }
            }
            if (_options.ApiKey == null)
            {
                if (_options.ApiKey == "")
                {
                    SelfLog.WriteLine("No LogHive ApiKey found in Appsettings.");
                }
            }

            _logger = new LogHiveApi(_options.ApiKey ?? "", _options.Url ?? logHiveUrl);
        }

        private const int maxStackTraceDepth = 5;

        public void Emit(LogEvent logEvent)
        {
            if (logEvent.Level >= _options.RestrictedToMinimumLevel)
            {
                try
                {
                    _ = Task.Run(() =>
                    {
                        var eventname = string.Empty;
                        var properties = new Dictionary<string, object>();

                        _options.ParseMessageTemplateForEventName ??= false;

                        //var exceptionString = new StringBuilder();

                        if (logEvent.Exception != null)
                        {
                            var counter = 1;
                            var nestedException = logEvent.Exception;
                            properties.Add($"EventName", logEvent.RenderMessage());
                            properties.Add($"MessageTemplate", logEvent.MessageTemplate.Text);
                            properties.Add($"TemplateProperties", logEvent.Properties);
                            properties.Add($"Timstamp", logEvent.Timestamp);
                            do
                            {
                                properties.Add($"message{counter}", logEvent.Exception.Message);
                                if (nestedException.StackTrace != null)
                                {
                                    properties.Add($"stacktrace{counter}", nestedException.StackTrace);
                                }
                                nestedException = nestedException.InnerException;

                                counter++;
                            }
                            while (nestedException != null && counter < maxStackTraceDepth);

                            var response = _logger.AddEventAsync(_options.ProjectName ?? "", _options.GroupName ?? "", "Exception", JsonConvert.SerializeObject(properties), logEvent.Level >= _options.MinimumPushNotificationLevel);
                        }
                        else
                        {
                            if (!(bool)_options.ParseMessageTemplateForEventName)
                            {
                                eventname = logEvent.MessageTemplate.Text;
                            }
                            else
                            {
                                eventname = logEvent.RenderMessage();
                            }
                            properties.Add($"EventName", eventname);
                            properties.Add($"MessageTemplate", logEvent.MessageTemplate.Text);
                            properties.Add($"TemplateProperties", logEvent.Properties);
                            properties.Add($"Timstamp", logEvent.Timestamp);

                            var response = _logger.AddEventAsync(_options.ProjectName ?? "", _options.GroupName ?? "", eventname, properties.Count > 0 ? JsonConvert.SerializeObject(properties) : "", logEvent.Level >= _options.MinimumPushNotificationLevel);
                        }
                    });
                }
                catch (Exception exc)
                {
                    SelfLog.WriteLine("Pushing Event to LogHive failed. {0}", exc);
                }
            }
        }
    }

    public static class LogHiveSinkExtensions
    {
        public static LoggerConfiguration LogHiveSink(
                  this LoggerSinkConfiguration loggerConfiguration,
                  string apiKey,
                  string projectName,
                  string groupName,
                  bool parseMessageTemplateForEventName = false,
                  LogEventLevel restrictedToMinimumLevel = LogEventLevel.Error,
                  LogEventLevel minimumPushNotificationLevel = LogEventLevel.Fatal,
                  string url = "")
        {
            var options = new LogHiveSinkOptions()
            {
                ApiKey = apiKey,
                ProjectName = projectName,
                GroupName = groupName,
                Url = url,
                ParseMessageTemplateForEventName = parseMessageTemplateForEventName,
                RestrictedToMinimumLevel = restrictedToMinimumLevel,
                MinimumPushNotificationLevel = minimumPushNotificationLevel,
            };
            return loggerConfiguration.Sink(new LogHiveSink(options));
        }
    }
}