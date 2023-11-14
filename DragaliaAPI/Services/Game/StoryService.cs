using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Fort;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Features.Shop;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Event;
using DragaliaAPI.Shared.MasterAsset.Models.Story;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Services.Game;

public class StoryService(
    IStoryRepository storyRepository,
    ILogger<StoryService> logger,
    IUserDataRepository userDataRepository,
    IInventoryRepository inventoryRepository,
    ITutorialService tutorialService,
    IFortRepository fortRepository,
    IMissionProgressionService missionProgressionService,
    IRewardService rewardService,
    IPaymentService paymentService
) : IStoryService
{
    private const int DragonStoryWyrmite = 25;
    private const int CastleStoryWyrmite = 50;
    private const int CharaStoryWyrmite1 = 25;
    private const int CharaStoryWyrmite2 = 10;
    private const int QuestStoryWyrmite = 25;
    private const int DmodeStoryWyrmite = 25;

    #region Eligibility check methods
    public async Task<bool> CheckStoryEligibility(StoryTypes type, int storyId)
    {
        logger.LogDebug("Checking eligibility for story {id} of type: {type}", storyId, type);
        DbPlayerStoryState story = await storyRepository.GetOrCreateStory(type, storyId);

        if (story.State == StoryState.Read)
        {
            logger.LogDebug("Story was already read");
            return true;
        }

        return type switch
        {
            StoryTypes.Chara or StoryTypes.Dragon => await CheckUnitStoryEligibility(storyId),
            StoryTypes.Castle => await CheckCastleStoryEligibility(storyId),
            StoryTypes.Quest => true,
            StoryTypes.Event => true,
            _ => throw new NotImplementedException($"Stories of type {type} are not implemented")
        };
    }

    private async Task<bool> CheckUnitStoryEligibility(int storyId)
    {
        if (!MasterAsset.UnitStory.TryGetValue(storyId, out UnitStory? storyData))
        {
            logger.LogWarning("Non-existent unit story id {id}", storyId);
            return false;
        }

        if (
            storyData.UnlockQuestStoryId != default
            && (
                await storyRepository.QuestStories.FirstOrDefaultAsync(
                    x => x.StoryId == storyData.UnlockQuestStoryId
                )
            )?.State != StoryState.Read
        )
        {
            logger.LogWarning("Player was missing required quest story id");
            return false;
        }

        if (
            storyData.UnlockTriggerStoryId != default
            && (
                await storyRepository.UnitStories.FirstOrDefaultAsync(
                    x => x.StoryId == storyData.UnlockTriggerStoryId
                )
            )?.State != StoryState.Read
        )
        {
            logger.LogWarning("Player was missing required unit story id");
            return false;
        }

        return true;
    }

    private async Task<bool> CheckCastleStoryEligibility(int storyId)
    {
        return await inventoryRepository.CheckQuantity(Materials.LookingGlass, 1);
    }

    #endregion

    #region Reading methods

    public async Task<IEnumerable<AtgenBuildEventRewardEntityList>> ReadStory(
        StoryTypes type,
        int storyId
    )
    {
        logger.LogInformation("Reading story {id} of type {type}", storyId, type);

        DbPlayerStoryState story = await storyRepository.GetOrCreateStory(type, storyId);

        if (story.State == StoryState.Read)
        {
            logger.LogDebug("Story was already read");
            return Enumerable.Empty<AtgenBuildEventRewardEntityList>();
        }

        story.State = StoryState.Read;

        IEnumerable<AtgenBuildEventRewardEntityList> rewards = type switch
        {
            StoryTypes.Chara or StoryTypes.Dragon => await ReadUnitStory(storyId),
            StoryTypes.Castle => await ReadCastleStory(storyId),
            StoryTypes.Quest => await ReadQuestStory(storyId),
            StoryTypes.Event => await ReadEventStory(storyId),
            StoryTypes.DungeonMode => await ReadDmodeStory(storyId),
            _ => throw new NotImplementedException($"Stories of type {type} are not implemented")
        };

        logger.LogDebug("Player earned story rewards: {@rewards}", rewards);
        return rewards;
    }

    private async Task<IEnumerable<AtgenBuildEventRewardEntityList>> ReadUnitStory(int storyId)
    {
        UnitStory data = MasterAsset.UnitStory[storyId];
        StoryData charaStory;
        List<AtgenBuildEventRewardEntityList> rewardList = new();
        DbPlayerStoryState story = await storyRepository.GetOrCreateStory(data.Type, storyId);
        int wyrmiteReward;
        int emblemRewardId;
        Entity emblemRewardEntity;

        if (data.Type == StoryTypes.Chara)
        {
            wyrmiteReward = data.IsFirstEpisode ? CharaStoryWyrmite1 : CharaStoryWyrmite2;
            charaStory = MasterAsset.CharaStories[data.ReleaseTriggerId];
            if (data.Id == charaStory.storyIds.Last())
            {
                emblemRewardId = data.ReleaseTriggerId;
                emblemRewardEntity = new Entity(EntityTypes.Title, emblemRewardId, 1);
                await rewardService.GrantReward(emblemRewardEntity);
                rewardList.Add(emblemRewardEntity.ToBuildEventRewardEntityList());
            }
        }
        else
        {
            wyrmiteReward = DragonStoryWyrmite;
        }

        await userDataRepository.GiveWyrmite(wyrmiteReward);
        story.State = StoryState.Read;
        rewardList.Add(
            new() { entity_type = EntityTypes.Wyrmite, entity_quantity = wyrmiteReward }
        );
        return rewardList;
    }

    private async Task<IEnumerable<AtgenBuildEventRewardEntityList>> ReadCastleStory(int storyId)
    {
        await inventoryRepository.UpdateQuantity(Materials.LookingGlass, -1);
        await userDataRepository.GiveWyrmite(CastleStoryWyrmite);

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
            await paymentService.ProcessPayment(
                new Entity(story.PayEntityType, story.PayEntityId, story.PayEntityQuantity)
            );
        }

        await tutorialService.OnStoryQuestRead(storyId);
        missionProgressionService.OnQuestStoryCleared(storyId);

        await userDataRepository.GiveWyrmite(QuestStoryWyrmite);
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
                {
                    await fortRepository.AddToStorage((FortPlants)reward.Id, reward.Quantity, true);
                }
                else
                {
                    await rewardService.GrantReward(
                        new Entity(reward.Type, reward.Id, reward.Quantity)
                    );
                }

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

        if (
            MasterAsset.EventData.TryGetValue(story.GroupId, out EventData? eventData)
            && eventData.IsMemoryEvent // Real events need to set is_temporary and do friendship points
            && eventData.GuestJoinStoryId == storyId
        )
        {
            logger.LogDebug("Granting memory event character {chara}", eventData.EventCharaId);

            await rewardService.GrantReward(
                new Entity(EntityTypes.Chara, Id: (int)eventData.EventCharaId)
            );
            rewardList.Add(
                new AtgenBuildEventRewardEntityList()
                {
                    entity_id = (int)eventData.EventCharaId,
                    entity_quantity = 1,
                    entity_type = EntityTypes.Chara
                }
            );
        }

        return rewardList;
    }

    private async Task<IEnumerable<AtgenBuildEventRewardEntityList>> ReadEventStory(int storyId)
    {
        EventStory story = MasterAsset.EventStory[storyId];
        if (story.PayEntityType != EntityTypes.None)
        {
            await paymentService.ProcessPayment(
                new Entity(story.PayEntityType, story.PayEntityId, story.PayEntityQuantity)
            );
        }

        await userDataRepository.GiveWyrmite(QuestStoryWyrmite);
        List<AtgenBuildEventRewardEntityList> rewardList =
            new()
            {
                new() { entity_type = EntityTypes.Wyrmite, entity_quantity = QuestStoryWyrmite }
            };

        // TODO(Events): ??? This is not used for compendium (maybe for collect events)

        return rewardList;
    }

    private async Task<IEnumerable<AtgenBuildEventRewardEntityList>> ReadDmodeStory(int storyId)
    {
        await userDataRepository.GiveWyrmite(DmodeStoryWyrmite);

        // Temporary measure to make fafnir upgrades more obtainable until endeavours are added
        Entity dmodePoint1Entity =
            new(EntityTypes.DmodePoint, Id: (int)DmodePoint.Point1, Quantity: 5_000);
        Entity dmodePoint2Entity =
            new(EntityTypes.DmodePoint, Id: (int)DmodePoint.Point2, Quantity: 1_000);
        await rewardService.GrantReward(dmodePoint1Entity);
        await rewardService.GrantReward(dmodePoint2Entity);

        List<AtgenBuildEventRewardEntityList> rewardList =
            new()
            {
                new()
                {
                    entity_type = EntityTypes.Wyrmite,
                    entity_id = 0,
                    entity_quantity = DmodeStoryWyrmite
                },
                dmodePoint1Entity.ToBuildEventRewardEntityList(),
                dmodePoint2Entity.ToBuildEventRewardEntityList()
            };

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
