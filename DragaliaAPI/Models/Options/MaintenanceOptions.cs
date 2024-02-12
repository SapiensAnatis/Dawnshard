namespace DragaliaAPI.Models.Options;

public class MaintenanceOptions
{
    public bool Enabled { get; set; }

    public string? Title { get; set; }

    public string? Body { get; set; }

    public DateTimeOffset? End { get; set; }
}
