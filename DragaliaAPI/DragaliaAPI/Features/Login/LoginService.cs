using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Features.Wall;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Login;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Login;

public class LoginService(
    IRewardService rewardService,
    TimeProvider dateTimeProvider,
    ApiContext apiContext,
    IWallService wallService,
    IPlayerIdentityService playerIdentityService,
    ILogger<LoginService> logger
) : ILoginService
{
    public async Task<IEnumerable<AtgenLoginBonusList>> RewardLoginBonus()
    {
        List<AtgenLoginBonusList> bonusList = new();

        DateTimeOffset time = dateTimeProvider.GetUtcNow();

        foreach (
            LoginBonusData bonusData in MasterAsset.LoginBonusData.Enumerable.Where(x =>
                x.StartTime <= time && time <= x.EndTime
            )
        )
        {
            DbLoginBonus? dbBonus = await apiContext.LoginBonuses.FirstOrDefaultAsync(x =>
                x.Id == bonusData.Id
            );

            if (dbBonus is null)
            {
                dbBonus = new DbLoginBonus()
                {
                    ViewerId = playerIdentityService.ViewerId,
                    Id = bonusData.Id,
                };
                apiContext.LoginBonuses.Add(dbBonus);
            }

            if (dbBonus.IsComplete)
            {
                logger.LogDebug(
                    "Player has already completed login bonus {@bonus}, skipping...",
                    dbBonus
                );
                continue;
            }

            int bonusCount = MasterAsset.LoginBonusReward.Enumerable.Count(x =>
                x.Gid == bonusData.Id
            );

            int dayId = bonusData.IsLoop ? dbBonus.CurrentDay % bonusCount : dbBonus.CurrentDay;

            dayId += 1;

            LoginBonusReward? reward = MasterAsset.LoginBonusReward.Enumerable.FirstOrDefault(x =>
                x.Gid == bonusData.Id && x.Day == dayId
            );

            if (reward == null)
            {
                logger.LogWarning(
                    "Failed to get reward for bonus data {bonusDataId} with day {dayId} (IsLoop: {isLoop})",
                    bonusData.Id,
                    dayId,
                    bonusData.IsLoop
                );
                continue;
            }

            dbBonus.CurrentDay = dayId;
            if (dbBonus.CurrentDay >= bonusCount && !bonusData.IsLoop)
                dbBonus.IsComplete = true;

            // TODO: Propagate this information up to the EntityResult
            _ = await rewardService.GrantReward(
                new Entity(
                    reward.EntityType,
                    reward.EntityId,
                    reward.EntityQuantity,
                    reward.EntityLimitBreakCount,
                    reward.EntityBuildupCount,
                    reward.EntityEquipableCount
                )
            );

            bonusList.Add(
                new AtgenLoginBonusList(
                    0,
                    bonusData.Id,
                    dbBonus.CurrentDay,
                    dayId,
                    reward.EntityType,
                    reward.EntityId,
                    reward.EntityQuantity,
                    reward.EntityLevel,
                    reward.EntityLimitBreakCount
                )
            );

            if (bonusData.EachDayEntityType != EntityTypes.None)
            {
                // TODO: Propagate this information up to the EntityResult
                _ = await rewardService.GrantReward(
                    new Entity(
                        bonusData.EachDayEntityType,
                        bonusData.EachDayEntityId,
                        bonusData.EachDayEntityQuantity
                    )
                );
            }
        }

        return bonusList;
    }

    public async Task<IList<AtgenMonthlyWallReceiveList>> GetWallMonthlyReceiveList()
    {
        if (!await wallService.CheckWallInitialized())
        {
            return [];
        }

        DateTimeOffset lastClaimDate = await apiContext
            .WallRewardDates.AsNoTracking()
            .Select(x => x.LastClaimDate)
            .FirstAsync();

        RewardStatus wallRewardStatus = wallService.CheckCanClaimReward(lastClaimDate)
            ? RewardStatus.Available
            : RewardStatus.Received;

        return
        [
            new()
            {
                QuestGroupId = WallService.WallQuestGroupId,
                IsReceiveReward = wallRewardStatus,
            }
        ];
    }
}
