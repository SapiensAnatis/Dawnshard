using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset.Models.QuestDrops;

namespace DragaliaAPI.Features.Reward;

public record Entity(
    EntityTypes Type,
    int Id = 0,
    int Quantity = 1,
    int? LimitBreakCount = null,
    int? BuildupCount = null,
    int? EquipableCount = null
// TODO: int? Level = null
)
{
    public AtgenBuildEventRewardEntityList ToBuildEventRewardEntityList()
    {
        return new()
        {
            EntityId = this.Id,
            EntityType = this.Type,
            EntityQuantity = this.Quantity
        };
    }

    public AtgenDuplicateEntityList ToDuplicateEntityList()
    {
        return new() { EntityId = this.Id, EntityType = this.Type };
    }

    public AtgenFirstClearSet ToFirstClearSet()
    {
        return new AtgenFirstClearSet(this.Id, this.Type, this.Quantity);
    }

    public AtgenMissionsClearSet ToMissionClearSet(int missionNo)
    {
        return new AtgenMissionsClearSet(this.Id, this.Type, this.Quantity, missionNo);
    }

    public AtgenDropAll ToDropAll()
    {
        return new AtgenDropAll(this.Id, this.Type, this.Quantity, 0, 0);
    }

    public static Entity FromQuestBonusDrop(QuestBonusDrop drop)
    {
        return new Entity(Type: drop.EntityType, Id: drop.Id, Quantity: drop.Quantity);
    }
};
