using System.Text.Json.Serialization;

namespace DragaliaAPI.Shared.Definitions;

public class DataQuest : IDataItem<int>
{
    [JsonPropertyName("_Id")]
    [JsonRequired]
    public required int Id { get; set; }

    [JsonPropertyName("_Scene01")]
    [JsonRequired]
    public required string Scene1 { get; set; }

    [JsonPropertyName("_AreaName01")]
    [JsonRequired]
    public required string AreaName1 { get; set; }

    [JsonPropertyName("_Scene02")]
    [JsonRequired]
    public required string Scene2 { get; set; }

    [JsonPropertyName("_AreaName02")]
    [JsonRequired]
    public required string AreaName2 { get; set; }

    [JsonPropertyName("_Scene03")]
    [JsonRequired]
    public required string Scene3 { get; set; }

    [JsonPropertyName("_AreaName03")]
    [JsonRequired]
    public required string AreaName3 { get; set; }

    [JsonPropertyName("_Scene04")]
    [JsonRequired]
    public required string Scene4 { get; set; }

    [JsonPropertyName("_AreaName04")]
    [JsonRequired]
    public required string AreaName4 { get; set; }

    [JsonPropertyName("_Scene05")]
    [JsonRequired]
    public required string Scene5 { get; set; }

    [JsonPropertyName("_AreaName05")]
    [JsonRequired]
    public required string AreaName5 { get; set; }

    [JsonPropertyName("_Scene06")]
    [JsonRequired]
    public required string Scene6 { get; set; }

    [JsonPropertyName("_AreaName06")]
    [JsonRequired]
    public required string AreaName6 { get; set; }
}
