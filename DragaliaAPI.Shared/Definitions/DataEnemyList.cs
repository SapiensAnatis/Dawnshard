using System.Text.Json.Serialization;

namespace DragaliaAPI.Shared.Definitions;

public class DataEnemyList : IDataItem<string>
{
    [JsonRequired]
    public required string Id { get; set; }

    [JsonRequired]
    public required List<int> Enemies { get; set; }
}
