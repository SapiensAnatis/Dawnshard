using DragaliaAPI.Features.Reward;
using DragaliaAPI.Helpers;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Login;

namespace DragaliaAPI.Features.Login;

public class LoginBonusService : ILoginBonusService
{
    private readonly IRewardService rewardService;
    private readonly IDateTimeProvider dateTimeProvider;
    private readonly ILogger<LoginBonusService> logger;

    public LoginBonusService(
        IRewardService rewardService,
        IDateTimeProvider dateTimeProvider,
        ILogger<LoginBonusService> logger
    )
    {
        this.rewardService = rewardService;
        this.dateTimeProvider = dateTimeProvider;
        this.logger = logger;
    }

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
            int currentDay = (int)(time - bonusData.StartTime).TotalDays + 1;
            int dayId = bonusData.IsLoop
                ? currentDay
                    % MasterAsset.LoginBonusReward.Enumerable.Count(x => x.Gid == bonusData.Id)
                : currentDay;

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
                    currentDay,
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
