using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models.QuestDrops;

public record QuestBonusReward(int QuestId, IEnumerable<QuestBonusDrop> Bonuses);

// TODO: Extend with random quantity variation

public record QuestBonusDrop(EntityTypes EntityType, int Id, int Quantity);
