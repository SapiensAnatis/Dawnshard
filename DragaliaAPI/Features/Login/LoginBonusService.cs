using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Helpers;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Login;
using DragaliaAPI.Shared.PlayerDetails;

namespace DragaliaAPI.Features.Login;

public class LoginBonusService(
    IRewardService rewardService,
    IDateTimeProvider dateTimeProvider,
    ILoginBonusRepository loginBonusRepository,
    IPlayerIdentityService playerIdentityService,
    ILogger<LoginBonusService> logger
) : ILoginBonusService
{
    private readonly IRewardService rewardService = rewardService;
    private readonly IDateTimeProvider dateTimeProvider = dateTimeProvider;
    private readonly ILogger<LoginBonusService> logger = logger;

    public async Task<IEnumerable<AtgenLoginBonusList>> RewardLoginBonus()
    {
        List<AtgenLoginBonusList> bonusList = new();

        DateTimeOffset time = this.dateTimeProvider.UtcNow;

        foreach (
            LoginBonusData bonusData in MasterAsset.LoginBonusData.Enumerable.Where(
                x => x.StartTime <= time && time <= x.EndTime
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

            dbBonus.CurrentDay += 1;

            int bonusCount = MasterAsset.LoginBonusReward.Enumerable.Count(
                x => x.Gid == bonusData.Id
            );

            int dayId = bonusData.IsLoop ? dbBonus.CurrentDay % bonusCount : dbBonus.CurrentDay;

            LoginBonusReward? reward = MasterAsset.LoginBonusReward.Enumerable.FirstOrDefault(
                x => x.Gid == bonusData.Id && x.Day == dayId
            );

            if (reward == null)
            {
                this.logger.LogWarning(
                    "Failed to get reward for bonus data {bonusDataId} with day {dayId} (IsLoop: {isLoop})",
                    bonusData.Id,
                    dayId,
                    bonusData.IsLoop
                );
                continue;
            }

            if (dbBonus.CurrentDay >= bonusCount && !bonusData.IsLoop)
                dbBonus.IsComplete = true;

            await this.rewardService.GrantReward(
                new(
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
                await this.rewardService.GrantReward(
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
