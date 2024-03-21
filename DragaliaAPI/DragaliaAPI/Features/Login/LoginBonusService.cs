using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Login;

namespace DragaliaAPI.Features.Login;

public class LoginBonusService(
    IRewardService rewardService,
    TimeProvider dateTimeProvider,
    ILoginBonusRepository loginBonusRepository,
    ILogger<LoginBonusService> logger
) : ILoginBonusService
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
            DbLoginBonus dbBonus = await loginBonusRepository.Get(bonusData.Id);
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

            await rewardService.GrantReward(
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
                await rewardService.GrantReward(
                    new Entity(
                        bonusData.EachDayEntityType,
                        bonusData.EachDayEntityId,
                        bonusData.EachDayEntityQuantity
                    )
                );
            }
        }

        //bonusList.Add(new AtgenLoginBonusList(1, 17, 540));
        //bonusList.Add(new AtgenLoginBonusList(1, 74, 2));

        return bonusList;
    }
}
