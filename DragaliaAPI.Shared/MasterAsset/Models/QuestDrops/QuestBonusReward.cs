using System.Text.Json.Serialization;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models.QuestDrops;

public record QuestBonusReward(int QuestId, IEnumerable<Drop> Bonuses);

// TODO: Extend with random quantity variation
public record Drop(
    [property: JsonConverter(typeof(JsonStringEnumConverter))] EntityTypes EntityType,
    int Id,
    int Quantity
);
