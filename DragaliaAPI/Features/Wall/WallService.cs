using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Wall;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Wall;

public class WallService(
    IWallRepository wallRepository,
    ILogger<WallService> logger,
    IMapper mapper,
    IRewardService rewardService,
    IPlayerIdentityService playerIdentityService
) : IWallService
{
    public const int MaximumQuestWallTotalLevel = 400;
    public const int MaximumQuestWallLevel = 80;
    public const int WallQuestGroupId = 21601;
    public const int FlameWallId = 216010001;

    public async Task LevelupQuestWall(int wallId)
    {
        DbPlayerQuestWall questWall = await wallRepository.GetQuestWall(wallId);

        // Increment level if it's not at max
        if (questWall.WallLevel < MaximumQuestWallLevel)
        {
            questWall.WallLevel++;
            questWall.IsStartNextLevel = false;
        }
    }

    public async Task SetQuestWallIsStartNextLevel(int wallId, bool value)
    {
        DbPlayerQuestWall questWall = await wallRepository.GetQuestWall(wallId);
        questWall.IsStartNextLevel = value;
    }

    public async Task<IEnumerable<QuestWallList>> GetQuestWallList()
    {
        return (await wallRepository.QuestWalls.ToListAsync()).Select(mapper.Map<QuestWallList>);
    }

    public async Task<int> GetTotalWallLevel()
    {
        int levelTotal = 0;
        for (int element = 0; element < 5; element++)
        {
            levelTotal += (await wallRepository.GetQuestWall(FlameWallId + element)).WallLevel;
        }

        if (levelTotal > MaximumQuestWallTotalLevel)
        {
            logger.LogWarning(
                "User {@accountId} had a quest wall total level above the max of 400: {@levelTotal}",
                playerIdentityService.AccountId,
                levelTotal
            );
            return MaximumQuestWallTotalLevel;
        }
        return levelTotal;
    }

    public async Task GrantMonthlyRewardEntityList(
        IEnumerable<AtgenBuildEventRewardEntityList> rewards
    )
    {
        logger.LogInformation(
            "Granting wall monthly reward list with size: {@wallRewardListSize}",
            rewards.Count()
        );

        int totalRupies = 0;
        int totalMana = 0;
        int totalEldwater = 0;

        foreach (AtgenBuildEventRewardEntityList entity in rewards)
        {
            switch (entity.entity_type)
            {
                case EntityTypes.Rupies:
                    totalRupies += entity.entity_quantity;
                    break;
                case EntityTypes.Mana:
                    totalMana += entity.entity_quantity;
                    break;
                case EntityTypes.Dew:
                    totalEldwater += entity.entity_quantity;
                    break;
            }
        }

        if (totalRupies > 0)
        {
            await rewardService.GrantReward(new Entity(EntityTypes.Rupies, 0, totalRupies));
        }

        if (totalMana > 0)
        {
            await rewardService.GrantReward(new Entity(EntityTypes.Mana, 0, totalMana));
        }

        if (totalEldwater > 0)
        {
            await rewardService.GrantReward(new Entity(EntityTypes.Dew, 0, totalEldwater));
        }
    }

    public IEnumerable<AtgenBuildEventRewardEntityList> GetMonthlyRewardEntityList(int levelTotal)
    {
        List<AtgenBuildEventRewardEntityList> rewardList = new();

        for (int level = 1; level <= levelTotal; level++)
        {
            QuestWallMonthlyReward reward = MasterAsset.QuestWallMonthlyReward.Get(level);
            rewardList.Add(
                new AtgenBuildEventRewardEntityList()
                {
                    entity_type = reward.RewardEntityType,
                    entity_id = reward.RewardEntityId,
                    entity_quantity = reward.RewardEntityQuantity
                }
            );
        }
        return rewardList;
    }

    public IEnumerable<AtgenUserWallRewardList> GetUserWallRewardList(
        int levelTotal,
        RewardStatus rewardStatus
    )
    {
        AtgenUserWallRewardList rewardList =
            new()
            {
                quest_group_id = WallQuestGroupId,
                sum_wall_level = levelTotal,
                last_reward_date = DateTimeOffset.UtcNow,
                reward_status = rewardStatus
            };
        return new[] { rewardList };
    }
}
