using System.Collections.Immutable;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Services;

public class StoryService : IStoryService
{
    private readonly IStoryRepository storyRepository;
    private readonly IUserDataRepository userDataRepository;
    private readonly IInventoryRepository inventoryRepository;
    private readonly IUnitRepository unitRepository;
    private readonly ILogger<StoryService> logger;

    private const int DragonStoryWyrmite = 25;
    private const int CastleStoryWyrmite = 50;
    private const int CharaStoryWyrmite1 = 25;
    private const int CharaStoryWyrmite2 = 10;
    private const int QuestStoryWyrmite = 25;
    private const int MaxStoryId = 1000103;

    private static readonly ImmutableDictionary<int, Charas> QuestStoryCharaRewards =
        new Dictionary<int, Charas>()
        {
            { 1000103, Charas.Elisanne },
            { 1000106, Charas.Ranzal },
            { 1000111, Charas.Cleo },
            { 1000202, Charas.Luca },
            { 1000808, Charas.Alex },
            { 1001108, Charas.Laxi },
            { 1001410, Charas.Zena },
            { 1001610, Charas.Chelle },
        }.ToImmutableDictionary();

    private static readonly ImmutableDictionary<int, Dragons> QuestStoryDragonRewards =
        new Dictionary<int, Dragons>()
        {
            { 1000109, Dragons.Midgardsormr },
            { 1000210, Dragons.Mercury },
            { 1000311, Dragons.Brunhilda },
            { 1000412, Dragons.Jupiter },
            { 1000509, Dragons.Zodiark },
        }.ToImmutableDictionary();

    public StoryService(
        IStoryRepository storyRepository,
        ILogger<StoryService> logger,
        IUserDataRepository userDataRepository,
        IInventoryRepository inventoryRepository,
        IUnitRepository unitRepository
    )
    {
        this.storyRepository = storyRepository;
        this.logger = logger;
        this.userDataRepository = userDataRepository;
        this.inventoryRepository = inventoryRepository;
        this.unitRepository = unitRepository;
    }

    #region Eligibility check methods
    public async Task<bool> CheckStoryEligibility(StoryTypes type, int storyId)
    {
        this.logger.LogInformation("Reading story {id} (type: {type})", storyId, type);
        DbPlayerStoryState story = await this.storyRepository.GetOrCreateStory(type, storyId);

        if (story.State == StoryState.Read)
        {
            this.logger.LogDebug("Story was already read");
            return true;
        }

        return type switch
        {
            StoryTypes.Chara or StoryTypes.Dragon => await this.CheckUnitStoryEligibility(storyId),
            StoryTypes.Castle => await this.CheckCastleStoryEligibility(storyId),
            StoryTypes.Quest => true,
            _ => throw new NotImplementedException($"Stories of type {type} are not implemented")
        };
    }

    private async Task<bool> CheckUnitStoryEligibility(int storyId)
    {
        if (!MasterAsset.UnitStory.TryGetValue(storyId, out UnitStory? storyData))
        {
            this.logger.LogWarning("Non-existent unit story id {id}", storyId);
            return false;
        }

        if (
            storyData.UnlockQuestStoryId != default
            && (
                await this.storyRepository.QuestStories.FirstOrDefaultAsync(
                    x => x.StoryId == storyData.UnlockQuestStoryId
                )
            )?.State != StoryState.Read
        )
        {
            this.logger.LogWarning("Player was missing required quest story id");
            return false;
        }

        if (
            storyData.UnlockTriggerStoryId != default
            && (
                await this.storyRepository.UnitStories.FirstOrDefaultAsync(
                    x => x.StoryId == storyData.UnlockTriggerStoryId
                )
            )?.State != StoryState.Read
        )
        {
            this.logger.LogWarning("Player was missing required unit story id");
            return false;
        }

        return true;
    }

    private async Task<bool> CheckCastleStoryEligibility(int storyId)
    {
        return await this.inventoryRepository.CheckQuantity(Materials.LookingGlass, 1);
    }

    #endregion

    #region Reading methods

    public async Task<IEnumerable<AtgenBuildEventRewardEntityList>> ReadStory(
        StoryTypes type,
        int storyId
    )
    {
        this.logger.LogInformation("Reading story {id} of type {type}", storyId, type);

        DbPlayerStoryState story = await this.storyRepository.GetOrCreateStory(type, storyId);

        if (story.State == StoryState.Read)
        {
            this.logger.LogDebug("Story was already read");
            return Enumerable.Empty<AtgenBuildEventRewardEntityList>();
        }

        story.State = StoryState.Read;

        IEnumerable<AtgenBuildEventRewardEntityList> rewards = type switch
        {
            StoryTypes.Chara or StoryTypes.Dragon => await ReadUnitStory(storyId),
            StoryTypes.Castle => await ReadCastleStory(storyId),
            StoryTypes.Quest => await ReadQuestStory(storyId),
            _ => throw new NotImplementedException($"Stories of type {type} are not implemented")
        };

        this.logger.LogDebug("Player earned story rewards: {rewards}", rewards);
        return rewards;
    }

    private async Task<IEnumerable<AtgenBuildEventRewardEntityList>> ReadUnitStory(int storyId)
    {
        UnitStory data = MasterAsset.UnitStory[storyId];

        DbPlayerStoryState story = await this.storyRepository.GetOrCreateStory(data.Type, storyId);

        int wyrmiteReward;

        if (data.Type == StoryTypes.Chara)
            wyrmiteReward = data.IsFirstEpisode ? CharaStoryWyrmite1 : CharaStoryWyrmite2;
        else
            wyrmiteReward = DragonStoryWyrmite;

        await this.userDataRepository.GiveWyrmite(wyrmiteReward);
        story.State = StoryState.Read;

        // TODO: Add epithets

        return new List<AtgenBuildEventRewardEntityList>()
        {
            new() { entity_type = EntityTypes.Wyrmite, entity_quantity = wyrmiteReward }
        };
    }

    private async Task<IEnumerable<AtgenBuildEventRewardEntityList>> ReadCastleStory(int storyId)
    {
        await this.inventoryRepository.UpdateQuantity(Materials.LookingGlass, -1);
        await this.userDataRepository.GiveWyrmite(CastleStoryWyrmite);

        List<AtgenBuildEventRewardEntityList> rewardList =
            new()
            {
                new()
                {
                    entity_type = EntityTypes.Wyrmite,
                    entity_id = 0,
                    entity_quantity = CastleStoryWyrmite
                }
            };

        return rewardList;
    }

    private async Task<IEnumerable<AtgenBuildEventRewardEntityList>> ReadQuestStory(int storyId)
    {
        if (storyId == MaxStoryId)
        {
            this.logger.LogInformation("Skipping tutorial at story {id}", storyId);
            await this.userDataRepository.SkipTutorial();
        }

        await this.userDataRepository.GiveWyrmite(QuestStoryWyrmite);
        List<AtgenBuildEventRewardEntityList> rewardList =
            new()
            {
                new() { entity_type = EntityTypes.Wyrmite, entity_quantity = QuestStoryWyrmite }
            };

        if (QuestStoryCharaRewards.TryGetValue(storyId, out Charas chara))
        {
            await this.unitRepository.AddCharas(chara);
            rewardList.Add(
                new()
                {
                    entity_id = (int)chara,
                    entity_type = EntityTypes.Chara,
                    entity_quantity = 1,
                }
            );
        }

        if (QuestStoryDragonRewards.TryGetValue(storyId, out Dragons dragon))
        {
            await this.unitRepository.AddDragons(dragon);
            rewardList.Add(
                new()
                {
                    entity_id = (int)dragon,
                    entity_type = EntityTypes.Dragon,
                    entity_quantity = 1,
                }
            );
        }

        this.logger.LogInformation("Granted rewards for reading new story: {rewards}", rewardList);
        return rewardList;
    }

    #endregion

    public static EntityResult GetEntityResult(
        IEnumerable<AtgenBuildEventRewardEntityList> rewardList
    )
    {
        IEnumerable<AtgenDuplicateEntityList> newGetEntityList = rewardList
            .Where(x => x.entity_type is EntityTypes.Dragon or EntityTypes.Chara)
            .Select(
                x =>
                    new AtgenDuplicateEntityList()
                    {
                        entity_id = x.entity_id,
                        entity_type = x.entity_type,
                    }
            );

        return new() { new_get_entity_list = newGetEntityList, };
    }

    public static AtgenQuestStoryRewardList ToQuestStoryReward(
        AtgenBuildEventRewardEntityList reward
    )
    {
        AtgenQuestStoryRewardList questReward =
            new()
            {
                entity_id = reward.entity_id,
                entity_type = reward.entity_type,
                entity_quantity = reward.entity_quantity
            };

        if (reward.entity_type is EntityTypes.Chara or EntityTypes.Dragon)
            questReward.entity_level = 1;

        return questReward;
    }
}
