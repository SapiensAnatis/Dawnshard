using System.Collections.Immutable;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Fort;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Story;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Services.Game;

public class StoryService : IStoryService
{
    private readonly IStoryRepository storyRepository;
    private readonly IUserDataRepository userDataRepository;
    private readonly IInventoryRepository inventoryRepository;
    private readonly ILogger<StoryService> logger;
    private readonly ITutorialService tutorialService;
    private readonly IFortRepository fortRepository;
    private readonly IMissionProgressionService missionProgressionService;
    private readonly IRewardService rewardService;

    private const int DragonStoryWyrmite = 25;
    private const int CastleStoryWyrmite = 50;
    private const int CharaStoryWyrmite1 = 25;
    private const int CharaStoryWyrmite2 = 10;
    private const int QuestStoryWyrmite = 25;

    public StoryService(
        IStoryRepository storyRepository,
        ILogger<StoryService> logger,
        IUserDataRepository userDataRepository,
        IInventoryRepository inventoryRepository,
        ITutorialService tutorialService,
        IFortRepository fortRepository,
        IMissionProgressionService missionProgressionService,
        IRewardService rewardService
    )
    {
        this.storyRepository = storyRepository;
        this.logger = logger;
        this.userDataRepository = userDataRepository;
        this.inventoryRepository = inventoryRepository;
        this.tutorialService = tutorialService;
        this.fortRepository = fortRepository;
        this.missionProgressionService = missionProgressionService;
        this.rewardService = rewardService;
    }

    #region Eligibility check methods
    public async Task<bool> CheckStoryEligibility(StoryTypes type, int storyId)
    {
        this.logger.LogDebug("Checking eligibility for story {id} of type: {type}", storyId, type);
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
            StoryTypes.Event => true,
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
            StoryTypes.Event => await ReadEventStory(storyId),
            _ => throw new NotImplementedException($"Stories of type {type} are not implemented")
        };

        this.logger.LogDebug("Player earned story rewards: {@rewards}", rewards);
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
        QuestStory story = MasterAsset.QuestStory[storyId];
        if (story.PayEntityTargetType != PayTargetType.None)
        {
            // TODO(Events): Implement when mana circle pr gets merged
            throw new NotImplementedException("luke you forgot to implement quest story payments");
        }

        await this.tutorialService.OnStoryQuestRead(storyId);
        this.missionProgressionService.OnQuestCleared(storyId);

        await this.userDataRepository.GiveWyrmite(QuestStoryWyrmite);
        List<AtgenBuildEventRewardEntityList> rewardList =
            new()
            {
                new() { entity_type = EntityTypes.Wyrmite, entity_quantity = QuestStoryWyrmite }
            };

        if (
            MasterAsset.QuestStoryRewardInfo.TryGetValue(
                storyId,
                out QuestStoryRewardInfo? rewardInfo
            )
        )
        {
            foreach (QuestStoryReward reward in rewardInfo.Rewards)
            {
                // We divert here as we care about quantity-restriction for story plants
                if (reward.Type == EntityTypes.FortPlant)
                    await this.fortRepository.AddToStorage(
                        (FortPlants)reward.Id,
                        null,
                        reward.Quantity,
                        true
                    );
                else
                    await this.rewardService.GrantReward(
                        new Entity(reward.Type, reward.Id, reward.Quantity)
                    );

                rewardList.Add(
                    new()
                    {
                        entity_id = reward.Id,
                        entity_type = reward.Type,
                        entity_quantity = reward.Quantity,
                    }
                );
            }
        }

        this.logger.LogInformation("Granted rewards for reading new story: {rewards}", rewardList);
        return rewardList;
    }

    private async Task<IEnumerable<AtgenBuildEventRewardEntityList>> ReadEventStory(int storyId)
    {
        EventStory story = MasterAsset.EventStory[storyId];
        if (story.PayEntityType != EntityTypes.None)
        {
            // TODO(Events): Implement when mana circle pr gets merged
            throw new NotImplementedException("luke you forgot to implement event story payments");
        }

        await this.userDataRepository.GiveWyrmite(QuestStoryWyrmite);
        List<AtgenBuildEventRewardEntityList> rewardList =
            new()
            {
                new() { entity_type = EntityTypes.Wyrmite, entity_quantity = QuestStoryWyrmite }
            };

        // TODO(Events): ??? This is not used for compendium (maybe for collect events)

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
