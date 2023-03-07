using LogHive.SDK.CSharp;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Debugging;
using Serilog.Events;
using System;
using System.Text;
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

        private const string exceptionDelimiter = "----- Message -----";
        private const string exceptionStackTrace = "----- StackTrace -----";
        private const int maxStackTraceDepth = 5;

        public void Emit(LogEvent logEvent)
        {
            if (logEvent.Level >= _options.RestrictedToMinimumLevel)
            {
                try
                {
                    _ = Task.Run(() =>
                    {
                        var message = logEvent.RenderMessage();
                        var exceptionString = new StringBuilder();

                        if (logEvent.Exception != null)
                        {
                            var counter = 0;
                            var nestedException = logEvent.Exception;
                            do
                            {
                                exceptionString.AppendLine(exceptionDelimiter);
                                exceptionString.AppendLine(logEvent.Exception.Message);
                                if (nestedException.StackTrace != null)
                                {
                                    exceptionString.AppendLine(exceptionStackTrace).AppendLine(nestedException.StackTrace);
                                }
                                nestedException = nestedException.InnerException;
                                counter++;
                            }
                            while (nestedException != null && counter < maxStackTraceDepth);

                            var response = _logger.AddEventAsync(_options.ProjectName ?? "", _options.GroupName ?? "", message, exceptionString.ToString(), logEvent.Level >= _options.MinimumPushNotificationLevel);
                        }
                        else
                        {
                            var response = _logger.AddEventAsync(_options.ProjectName ?? "", _options.GroupName ?? "", message, "", logEvent.Level >= _options.MinimumPushNotificationLevel);
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
                  LogEventLevel restrictedToMinimumLevel = LogEventLevel.Error,
                  LogEventLevel minimumPushNotificationLevel = LogEventLevel.Error,
                  string url = "")
        {
            var options = new LogHiveSinkOptions()
            {
                ApiKey = apiKey,
                ProjectName = projectName,
                GroupName = groupName,
                Url = url,
                RestrictedToMinimumLevel = restrictedToMinimumLevel,
                MinimumPushNotificationLevel = minimumPushNotificationLevel,
            };
            return loggerConfiguration.Sink(new LogHiveSink(options));
        }
    }
}