using System.Collections;
using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Fort;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Features.Player;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Features.Shop;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Models.Options;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Wall;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using static StackExchange.Redis.Role;

namespace DragaliaAPI.Features.Wall;

public class WallService(
    IWallRepository wallRepository,
    ILogger<WallService> logger,
    IMapper mapper
) : IWallService
{
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
        for (int i = 0; i < 5; i++)
        {
            levelTotal += (await wallRepository.GetQuestWall(FlameWallId + i)).WallLevel;
        }
        return levelTotal;
    }

    public IEnumerable<AtgenBuildEventRewardEntityList> GetMonthlyRewardEntityList(int levelTotal)
    {
        List<AtgenBuildEventRewardEntityList> rewardList = new();
        for (int level = 1; level <= levelTotal; level++)
        {
            QuestWallMonthlyReward reward = MasterAsset.QuestWallMonthlyReward.Get(level);
            rewardList.Add(
                new()
                {
                    entity_type = reward.RewardEntityType,
                    entity_id = reward.RewardEntityId,
                    entity_quantity = reward.RewardEntityQuantity
                }
            );
        }
        return rewardList;
    }

    public IEnumerable<AtgenUserWallRewardList> GetUserWallRewardList(int levelTotal, RewardStatus rewardStatus)
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
