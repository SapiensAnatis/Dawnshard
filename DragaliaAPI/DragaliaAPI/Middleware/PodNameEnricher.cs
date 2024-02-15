using Serilog.Core;
using Serilog.Events;

namespace DragaliaAPI.Middleware;

/// <summary>
/// Provides the pod name in the logs. Useful to isolate logs per pod if scaling in K8s
/// </summary>
public class PodNameEnricher : ILogEventEnricher
{
    private const string EnvPodName = "HOSTNAME";
    private const string PropertyName = "PodName";

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        string? podName = Environment.GetEnvironmentVariable(EnvPodName);

        if (!string.IsNullOrEmpty(podName))
        {
            LogEventProperty property = propertyFactory.CreateProperty(PropertyName, podName);
            logEvent.AddPropertyIfAbsent(property);
        }
    }
}
