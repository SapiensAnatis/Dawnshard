using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Reward.V2.Handlers;

internal sealed class DragonHandler(
    ApiContext apiContext,
    IPlayerIdentityService playerIdentityService,
    TimeProvider timeProvider
) : IRewardHandler<DragonReward>
{
    public async Task<GrantResult> GrantAsync(DragonReward reward)
    {
        int storageSpace = await apiContext
            .PlayerUserData.Select(x => x.MaxDragonQuantity)
            .FirstAsync();
        int dragonCount = await apiContext.PlayerDragonData.CountAsync();

        if (dragonCount >= storageSpace)
        {
            return new GrantResult(RewardGrantResult.GiftBox);
        }

        apiContext.PlayerDragonData.Add(CreateDragonData(reward));

        return new GrantResult(RewardGrantResult.Added);
    }

    public async Task<IDictionary<TKey, GrantResult>> GrantRangeAsync<TKey>(
        IDictionary<TKey, DragonReward> rewards
    )
        where TKey : struct
    {
        int storageSpace = await apiContext
            .PlayerUserData.Select(x => x.MaxDragonQuantity)
            .FirstAsync();
        int dragonCount = await apiContext.PlayerDragonData.CountAsync();

        Dictionary<TKey, GrantResult> resultDict = [];

        foreach ((TKey key, DragonReward reward) in rewards)
        {
            if (dragonCount >= storageSpace)
            {
                resultDict.Add(key, new GrantResult(RewardGrantResult.GiftBox));
                continue;
            }

            apiContext.PlayerDragonData.Add(CreateDragonData(reward));
            resultDict.Add(key, new GrantResult(RewardGrantResult.Added));
            dragonCount++;
        }

        return resultDict;
    }

    private DbPlayerDragonData CreateDragonData(DragonReward reward)
    {
        if (!MasterAsset.DragonData.TryGetValue(reward.Id, out DragonData? dragonData))
        {
            throw new ArgumentException(
                $"ID of reward {reward} is not a valid dragon",
                nameof(reward)
            );
        }

        if (
            reward.Level < 1
            || reward.Level
                > DragonConstants.GetMaxLevelFor(dragonData.Rarity, reward.LimitBreakCount)
        )
        {
            throw new ArgumentOutOfRangeException(
                nameof(reward),
                $"Dragon level in reward {reward} is out of range"
            );
        }

        int exp = DragonConstants.MaxLevel[reward.Level];
        byte abilityLevel = checked((byte)(reward.LimitBreakCount + 5));

        return new DbPlayerDragonData()
        {
            ViewerId = playerIdentityService.ViewerId,
            DragonId = reward.Id,
            Level = checked((byte)reward.Level),
            LimitBreakCount = checked((byte)reward.LimitBreakCount),
            GetTime = timeProvider.GetUtcNow(),
            Exp = exp,
            Ability1Level = abilityLevel,
            Ability2Level = abilityLevel,
            Skill1Level = checked((byte)((reward.LimitBreakCount / 4) + 1)),
        };
    }
}
