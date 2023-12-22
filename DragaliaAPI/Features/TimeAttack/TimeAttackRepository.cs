using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.PlayerDetails;

namespace DragaliaAPI.Features.TimeAttack;

public class TimeAttackRepository(
    ApiContext apiContext,
    IPlayerIdentityService playerIdentityService
) : ITimeAttackRepository
{
    public IQueryable<DbReceivedRankingTierReward> ReceivedRewards =>
        apiContext.ReceivedRankingTierRewards.Where(
            x => x.ViewerId == playerIdentityService.ViewerId
        );

    public async Task CreateOrUpdateClear(DbTimeAttackClear clear)
    {
        if (await apiContext.TimeAttackClears.FindAsync(clear.GameId) is { } existingClear)
        {
            existingClear.Players.AddRange(clear.Players);
        }
        else
        {
            apiContext.TimeAttackClears.Add(clear);
        }
    }

    public void AddRewards(IEnumerable<DbReceivedRankingTierReward> rewards) =>
        apiContext.ReceivedRankingTierRewards.AddRange(rewards);
}
