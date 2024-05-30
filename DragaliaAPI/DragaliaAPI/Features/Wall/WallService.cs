using DragaliaAPI.Database;
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

public partial class WallService(
    ILogger<WallService> logger,
    IRewardService rewardService,
    IMissionService missionService,
    IMissionProgressionService missionProgressionService,
    TimeProvider timeProvider,
    ApiContext apiContext,
    IPlayerIdentityService playerIdentityService
) : IWallService
{
    public const int MaximumQuestWallTotalLevel = 400;
    public const int MaximumQuestWallLevel = 80;
    public const int WallQuestGroupId = 21601;
    public const int FlameWallId = 216010001;

    public async Task LevelupQuestWall(int wallId)
    {
        DbPlayerQuestWall questWall = await this.GetQuestWall(wallId);

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
        DbPlayerQuestWall questWall =
            await apiContext.PlayerQuestWalls.FirstOrDefaultAsync(x => x.WallId == wallId)
            ?? throw new InvalidOperationException($"Could not get questwall {wallId}");

        questWall.IsStartNextLevel = value;
    }

    public async Task<int> GetTotalWallLevel()
    {
        int levelTotal = await apiContext.PlayerQuestWalls.Take(5).SumAsync(x => x.WallLevel);

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

    public Task<Dictionary<QuestWallTypes, int>> GetWallLevelMap() =>
        apiContext.PlayerQuestWalls.ToDictionaryAsync(
            x => (QuestWallTypes)x.WallId,
            x => x.WallLevel
        );

    public async Task InitializeWall()
    {
        if (await this.CheckWallInitialized())
            return;

        logger.LogInformation("Initializing wall.");

        for (int element = 0; element < 5; element++)
        {
            apiContext.PlayerQuestWalls.Add(
                new DbPlayerQuestWall()
                {
                    ViewerId = playerIdentityService.ViewerId,
                    WallId = FlameWallId + element,
                    WallLevel = 0, // Indicates you have not completed level 1. Goes up to 80 upon completing level 80
                    IsStartNextLevel = false,
                }
            );
        }

        apiContext.WallRewardDates.Add(
            new DbWallRewardDate()
            {
                ViewerId = playerIdentityService.ViewerId,
                LastClaimDate = DateTimeOffset.UtcNow, // Make them wait until next month to claim
            }
        );

        await this.InitializeWallMissions();
    }

    public async Task GrantMonthlyRewardEntityList(IList<AtgenBuildEventRewardEntityList> rewards)
    {
        logger.LogInformation(
            "Granting wall monthly reward list with size: {@wallRewardListSize}",
            rewards.Count
        );

        int totalRupies = 0;
        int totalMana = 0;
        int totalEldwater = 0;
        int totalSand = 0;

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
                case EntityTypes.Material when entity.EntityId == (int)Materials.TwinklingSand:
                    totalSand += entity.EntityQuantity;
                    break;
            }
        }

        // We are not too concerned if we get a result other than RewardGrantResult.Added here, since this is all
        // just currency.
        if (totalRupies > 0)
        {
            _ = await rewardService.GrantReward(new Entity(EntityTypes.Rupies, 0, totalRupies));
        }

        if (totalMana > 0)
        {
            _ = await rewardService.GrantReward(new Entity(EntityTypes.Mana, 0, totalMana));
        }

        if (totalEldwater > 0)
        {
            _ = await rewardService.GrantReward(new Entity(EntityTypes.Dew, 0, totalEldwater));
        }

        if (totalSand > 0)
        {
            _ = await rewardService.GrantReward(
                new Entity(EntityTypes.Material, (int)Materials.TwinklingSand, totalSand)
            );
        }

        DbWallRewardDate? trackedRewardDate = apiContext.WallRewardDates.Local.FirstOrDefault();

        if (trackedRewardDate is null)
        {
            throw new InvalidOperationException(
                "No instance of DbWallRewardDate is being tracked - update failed"
            );
        }

        trackedRewardDate.LastClaimDate = timeProvider.GetUtcNow();
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

    public async Task<AtgenUserWallRewardList> GetUserWallRewardList()
    {
        int totalWallLevel = await this.GetTotalWallLevel();

        DateTimeOffset lastClaimDate = await apiContext
            .WallRewardDates.AsNoTracking()
            .Select(x => x.LastClaimDate)
            .FirstAsync();

        bool eligible = this.CheckCanClaimReward(lastClaimDate);

        return new()
        {
            QuestGroupId = WallQuestGroupId,
            SumWallLevel = totalWallLevel,
            LastRewardDate = lastClaimDate,
            RewardStatus = eligible ? RewardStatus.Available : RewardStatus.Received,
        };
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

    public async Task<DbWallRewardDate> GetLastRewardDate()
    {
        // Get AsTracking for GrantMonthlyRewardEntityList
        // Does not do anything for now -- but anticipates turning query tracking off later
        DbWallRewardDate? wallRewardDate = await apiContext
            .WallRewardDates.AsTracking()
            .FirstOrDefaultAsync();

        if (wallRewardDate is null)
        {
            // We expect, if this function is being called properly behind a CheckWallInitialized, that there
            // should be data here. Between InitializeWall and the V21Update, everyone using this data
            // should have a row in this table.
            throw new InvalidOperationException("Failed to fetch last DbWallRewardDate");
        }

        return wallRewardDate;
    }

    public async Task<bool> CheckWallInitialized()
    {
        bool initialized = await apiContext.PlayerQuestWalls.AnyAsync();

        Log.WallInitializedStatus(logger, initialized);

        return initialized;
    }

    public bool CheckCanClaimReward(DateTimeOffset lastClaimDate)
    {
        // The reward is available each month on the 15th.
        DateTimeOffset mostRecentRewardDate = timeProvider.GetLastWallRewardDate();

        Log.CheckingClaimDate(logger, lastClaimDate, mostRecentRewardDate);

        bool eligible = mostRecentRewardDate > lastClaimDate;

        Log.ClaimCheckResult(logger, eligible);

        return eligible;
    }

    public async Task<DbPlayerQuestWall> GetQuestWall(int wallId) =>
        await apiContext.PlayerQuestWalls.FirstOrDefaultAsync(x => x.WallId == wallId)
        ?? throw new InvalidOperationException($"Could not get questwall {wallId}");

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Information, "Wall initialization check: {WallInitialized}.")]
        public static partial void WallInitializedStatus(ILogger logger, bool wallInitialized);

        [LoggerMessage(
            LogLevel.Debug,
            "Checking wall monthly reward eligibility: last claim date: {ClaimDate}, most recent reward date {RewardDate}"
        )]
        public static partial void CheckingClaimDate(
            ILogger logger,
            DateTimeOffset claimDate,
            DateTimeOffset rewardDate
        );

        [LoggerMessage(
            LogLevel.Information,
            "Wall monthly reward eligibility check result: {CheckResult}"
        )]
        public static partial void ClaimCheckResult(ILogger logger, bool checkResult);
    }
}
