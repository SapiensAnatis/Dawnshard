namespace DragaliaAPI.Models.Nintendo;

public record AnalyticsConfigResponse
{
    public string accessToken { get; } = "";
    public string applicationId { get; } = "";
    public string city { get; } = "";
    public string country { get; } = "";
    public long expirationTime { get; } = long.MaxValue;
    public bool immediateReporting { get; } = false;
    public string mode { get; } = "";
    public string region { get; } = "";
    public int reportingPeriod { get; } = int.MaxValue;
    public string topic { get; } = "";
}