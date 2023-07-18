using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Extensions;

public static class EntityExtensions
{
    public static AtgenFirstClearSet ToFirstClearSet(this Entity entity)
    {
        return new AtgenFirstClearSet(entity.Id, entity.Type, entity.Quantity);
    }

    public static AtgenMissionsClearSet ToMissionClearSet(this Entity entity, int missionNo)
    {
        return new AtgenMissionsClearSet(entity.Id, entity.Type, entity.Quantity, missionNo);
    }

    public static AtgenDropAll ToDropAll(this Entity entity)
    {
        return new AtgenDropAll(entity.Id, entity.Type, entity.Quantity, 0, 0);
    }
}
