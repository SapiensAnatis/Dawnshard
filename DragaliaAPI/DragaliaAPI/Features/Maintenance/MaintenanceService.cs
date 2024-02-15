using DragaliaAPI.Models.Options;
using Microsoft.Extensions.Options;

namespace DragaliaAPI.Features.Maintenance;

public class MaintenanceService(IOptionsMonitor<MaintenanceOptions> maintenanceOptions)
{
    public bool CheckIsMaintenance() => maintenanceOptions.CurrentValue.Enabled;

    public string GetMaintenanceText()
    {
        string date = string.Empty;
        string schedule = string.Empty;

        if (maintenanceOptions.CurrentValue.End is not null)
        {
            DateTime japanStandardTimeDate =
                maintenanceOptions.CurrentValue.End.Value.UtcDateTime.AddHours(9);
            date = japanStandardTimeDate.ToString("s");
            schedule = "Check back at:";
        }

        string xml = $"""
            <title>{maintenanceOptions.CurrentValue.Title}</title>
            <body>{maintenanceOptions.CurrentValue.Body}</body>
            <schedule>{schedule}</schedule>
            <date>{date}</date>
            """;

        return xml;
    }
}
