using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Missions;
using DragaliaAPI.Shared.MasterAsset.Models.Wall;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Wall;

public class WallService(
    IWallRepository wallRepository,
    ILogger<WallService> logger,
    IMapper mapper,
    IRewardService rewardService,
    IMissionService missionService,
    IMissionProgressionService missionProgressionService,
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
        if (questWall.WallLevel >= MaximumQuestWallLevel)
            return;

        questWall.WallLevel++;
        questWall.IsStartNextLevel = false;

        missionProgressionService.EnqueueEvent(
            MissionCompleteType.WallLevelCleared,
            value: 1,
            parameter: questWall.WallLevel,
            parameter2: questWall.WallId
        );
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

    public Task<Dictionary<QuestWallTypes, int>> GetWallLevelMap()
    {
        return wallRepository.QuestWalls.ToDictionaryAsync(
            x => (QuestWallTypes)x.WallId,
            x => x.WallLevel
        );
    }

    public async Task InitializeWall()
    {
        if (await wallRepository.QuestWalls.AnyAsync())
            return;

        logger.LogInformation("Initializing wall.");

        await wallRepository.AddInitialWall();
        await this.InitializeWallMissions();
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
            switch (entity.EntityType)
            {
                case EntityTypes.Rupies:
                    totalRupies += entity.EntityQuantity;
                    break;
                case EntityTypes.Mana:
                    totalMana += entity.EntityQuantity;
                    break;
                case EntityTypes.Dew:
                    totalEldwater += entity.EntityQuantity;
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

    public List<AtgenBuildEventRewardEntityList> GetMonthlyRewardEntityList(int levelTotal)
    {
        List<AtgenBuildEventRewardEntityList> rewardList = new();

        for (int level = 1; level <= levelTotal; level++)
        {
            QuestWallMonthlyReward reward = MasterAsset.QuestWallMonthlyReward.Get(level);
            rewardList.Add(
                new AtgenBuildEventRewardEntityList()
                {
                    EntityType = reward.RewardEntityType,
                    EntityId = reward.RewardEntityId,
                    EntityQuantity = reward.RewardEntityQuantity
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
                QuestGroupId = WallQuestGroupId,
                SumWallLevel = levelTotal,
                LastRewardDate = DateTimeOffset.UtcNow,
                RewardStatus = rewardStatus
            };
        return new[] { rewardList };
    }

    public async Task InitializeWallMissions()
    {
        const int clearAnyMission = 10010101; // Clear The Mercurial Gauntlet
        await missionService.StartMission(MissionType.Normal, clearAnyMission);

        int[] elementalMissionStarts =
        [
            10010201, // Clear Flame level 1
            10010301, // Clear Water level 1
            10010401, // Clear Wind level 1
            10010501, // Clear Light level 1
            10010601, // Clear Shadow level 1
        ];

        foreach (int missionId in elementalMissionStarts)
            await missionService.StartMission(MissionType.Normal, missionId);

        const int allMissionStart = 10010701; // Clear Lv. 2 of The Mercurial Gauntlet in All Elements
        await missionService.StartMission(MissionType.Normal, allMissionStart);
    }
}
