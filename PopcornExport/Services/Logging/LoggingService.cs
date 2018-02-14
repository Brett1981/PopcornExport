using Microsoft.ApplicationInsights;
using PopcornExport.Helpers;

namespace PopcornExport.Services.Logging
{
    /// <summary>
    /// The logger using Application Insights
    /// </summary>
    public class LoggingService : ILoggingService
    {
        public TelemetryClient Telemetry { get; }

        public LoggingService()
        {
            Telemetry = new TelemetryClient
            {
                InstrumentationKey = Constants.ApplicationInsightsKey
            };
        }
    }
}