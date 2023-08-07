using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;

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
            entity_id = this.Id,
            entity_type = this.Type,
            entity_quantity = this.Quantity
        };
    }

    public AtgenDuplicateEntityList ToDuplicateEntityList()
    {
        return new() { entity_id = this.Id, entity_type = this.Type };
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
};
