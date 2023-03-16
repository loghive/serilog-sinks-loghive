namespace SimpleSink
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var log = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.LogHiveSink("your-api-key", "yourprojectname", "yourgroupname",
                LogEventLevel.Information, LogEventLevel.Information)
                .CreateLogger();

            var position = new { Latitude = 25, Longitude = 134 };
            var elapsedMs = 34;

            log.Information("Processed {@Position} in {Elapsed:000} ms.", position, elapsedMs);

            Console.ReadKey();
        }
    }
}