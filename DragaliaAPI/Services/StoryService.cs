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

    public static readonly ImmutableDictionary<
        int,
        AtgenDuplicateEntityList
    > QuestStoryEntityRewards = new Dictionary<int, AtgenDuplicateEntityList>()
    {
        { 1000103, new(EntityTypes.Chara, (int)Charas.Elisanne) }
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

    public async Task<bool> CheckUnitStoryEligibility(int storyId)
    {
        if (!MasterAsset.StoryData.TryGetValue(storyId, out UnitStory? storyData))
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
            )?.State == StoryState.Read
        )
        {
            this.logger.LogWarning(
                "User did not meet quest story prerequisite for story {id}",
                storyId
            );

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
            this.logger.LogWarning(
                "User did not meet unit story prerequisite for story {id}",
                storyId
            );

            return false;
        }

        return true;
    }

    public async Task<IEnumerable<AtgenBuildEventRewardEntityList>> ReadUnitStory(int storyId)
    {
        UnitStory data = MasterAsset.StoryData[storyId];
        this.logger.LogInformation("Reading story {id} (type: {type})", storyId, data.Type);

        List<AtgenBuildEventRewardEntityList> rewards = new();

        DbPlayerStoryState story = await this.storyRepository.GetOrCreateStory(data.Type, storyId);

        if (story.State == StoryState.Read)
            return rewards;

        int wyrmiteReward;

        if (data.Type == StoryTypes.Chara)
            wyrmiteReward = data.IsFirstEpisode ? CharaStoryWyrmite1 : CharaStoryWyrmite2;
        else
            wyrmiteReward = DragonStoryWyrmite;

        await this.userDataRepository.GiveWyrmite(wyrmiteReward);
        story.State = StoryState.Read;

        rewards.Add(new() { entity_type = EntityTypes.Wyrmite, entity_quantity = wyrmiteReward });

        // TODO: Add epithets

        this.logger.LogInformation("Granted rewards for reading new story: {rewards}", rewards);

        return rewards;
    }

    public async Task<bool> CheckCastleStoryEligibility(int storyId)
    {
        DbPlayerStoryState story = await this.storyRepository.GetOrCreateStory(
            StoryTypes.Castle,
            storyId
        );

        if (story.State == StoryState.Read)
            return true;

        return await this.inventoryRepository.CheckQuantity(Materials.LookingGlass, 1);
    }

    public async Task<IEnumerable<AtgenBuildEventRewardEntityList>> ReadCastleStory(int storyId)
    {
        this.logger.LogInformation("Reading story {id} (type: {type})", storyId, StoryTypes.Castle);

        List<AtgenBuildEventRewardEntityList> rewardList = new(); // wtf is this type

        DbPlayerStoryState story = await this.storyRepository.GetOrCreateStory(
            StoryTypes.Castle,
            storyId
        );

        if (story.State == StoryState.Read)
            return rewardList;

        await this.inventoryRepository.UpdateQuantity(Materials.LookingGlass, -1);
        await this.userDataRepository.GiveWyrmite(CastleStoryWyrmite);
        story.State = StoryState.Read;

        rewardList.Add(
            new()
            {
                entity_type = EntityTypes.Wyrmite,
                entity_id = 0,
                entity_quantity = CastleStoryWyrmite
            }
        );

        this.logger.LogInformation("Granted rewards for reading new story: {rewards}", rewardList);

        return rewardList;
    }

    public async Task<IEnumerable<AtgenQuestStoryRewardList>> ReadQuestStory(int storyId)
    {
        this.logger.LogInformation("Reading story {id} (type: {type})", storyId, StoryTypes.Quest);

        List<AtgenQuestStoryRewardList> rewardList = new();

        DbPlayerStoryState story = await this.storyRepository.GetOrCreateStory(
            StoryTypes.Quest,
            storyId
        );

        if (story.State == StoryState.Read)
            return rewardList;

        if (storyId == MaxStoryId)
        {
            this.logger.LogInformation("Skipping tutorial at storyId {id}", storyId);
            await this.userDataRepository.SkipTutorial();
        }

        await this.userDataRepository.GiveWyrmite(QuestStoryWyrmite);
        rewardList.Add(
            new() { entity_type = EntityTypes.Wyrmite, entity_quantity = QuestStoryWyrmite }
        );

        if (!QuestStoryEntityRewards.TryGetValue(storyId, out AtgenDuplicateEntityList? entity))
        {
            this.logger.LogInformation(
                "Granted rewards for reading new story: {rewards}",
                rewardList
            );
            return rewardList;
        }

        rewardList.Add(
            new()
            {
                entity_id = entity.entity_id,
                entity_type = entity.entity_type,
                entity_quantity = 1,
                entity_level = 1,
                entity_limit_break_count = 0,
            }
        );

        switch (entity.entity_type)
        {
            case EntityTypes.Dragon:
                await this.unitRepository.AddDragons((Dragons)entity.entity_id);
                break;
            case EntityTypes.Chara:
                await this.unitRepository.AddCharas((Charas)entity.entity_id);
                break;
            default:
                throw new NotImplementedException(
                    $"Unsupported quest story reward entity type: {entity.entity_type}"
                );
        }

        this.logger.LogInformation("Granted rewards for reading new story: {rewards}", rewardList);
        return rewardList;
    }

    public static EntityResult GetEntityResult(IEnumerable<AtgenQuestStoryRewardList> rewardList)
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
}
