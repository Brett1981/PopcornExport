using Microsoft.ApplicationInsights;

namespace PopcornExport.Services.Logging
{
    /// <summary>
    /// The logger using Application Insights
    /// </summary>
    public interface ILoggingService
    {
        TelemetryClient Telemetry { get; }
    }
}
