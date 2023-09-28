using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.TimeAttack;

public interface ITimeAttackCacheService
{
    Task<TimeAttackCacheEntry?> Get();
    Task Set(int questId, PartyInfo ingameData);
}
