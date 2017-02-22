using Microsoft.ApplicationInsights;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
