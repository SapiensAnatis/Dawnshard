using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;

namespace DragaliaAPI.Features.TimeAttack;

public interface ITimeAttackRepository
{
    IQueryable<DbReceivedRankingTierReward> ReceivedRewards { get; }

    void AddRewards(IEnumerable<DbReceivedRankingTierReward> rewards);
    Task CreateOrUpdateClear(DbTimeAttackClear clear);
}
