using System.Diagnostics;
using OpenTelemetry;

namespace DragaliaAPI.Infrastructure;

internal sealed class FilteringProcessor : BaseProcessor<Activity>
{
    public override void OnEnd(Activity data)
    {
        Activity root = data;

        while (root.Parent is not null)
        {
            root = root.Parent;
        }

        if (root.OperationName != "Microsoft.AspNetCore.Hosting.HttpRequestIn")
        {
            return;
        }

        foreach ((string key, string? value) in root.Tags)
        {
            if (key == "url.path" && value is "/metrics" or "/health" or "/ping")
            {
                root.ActivityTraceFlags &= ~ActivityTraceFlags.Recorded;
            }
        }
    }
}