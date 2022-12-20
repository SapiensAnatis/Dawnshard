using System.Text.Json.Serialization;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.Definitions;

public class DataQuest : IDataItem<int>
{
    public IEnumerable<DataQuestAreaInfo> AreaInfo =>
        new List<DataQuestAreaInfo>()
        {
            new(this.Scene1, this.AreaName1),
            new(this.Scene2, this.AreaName2),
            new(this.Scene3, this.AreaName3),
            new(this.Scene4, this.AreaName4),
            new(this.Scene5, this.AreaName5),
            new(this.Scene6, this.AreaName6),
        }.Where(x => !string.IsNullOrEmpty(x.ScenePath) && !string.IsNullOrEmpty(x.AreaName));

    [JsonPropertyName("_Id")]
    [JsonRequired]
    public required int Id { get; set; }

    [JsonPropertyName("_DungeonType")]
    [JsonRequired]
    public required DungeonTypes DungeonType { get; set; }

    [JsonPropertyName("_QuestPlayModeType")]
    [JsonRequired]
    public required QuestPlayModeTypes QuestPlayModeType { get; set; }

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
