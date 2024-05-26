using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Fort;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Features.Player;
using DragaliaAPI.Features.Present;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Features.Shop;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Features.Presents;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Event;
using DragaliaAPI.Shared.MasterAsset.Models.Story;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Story;

public class StoryService(
    IStoryRepository storyRepository,
    ILogger<StoryService> logger,
    IUserDataRepository userDataRepository,
    IInventoryRepository inventoryRepository,
    IPresentService presentService,
    ITutorialService tutorialService,
    IFortRepository fortRepository,
    IMissionProgressionService missionProgressionService,
    IRewardService rewardService,
    IPaymentService paymentService,
    IUserService userService,
    ApiContext apiContext,
    IPlayerIdentityService playerIdentityService
) : IStoryService
{
    private const int DragonStoryWyrmite = 25;
    private const int CastleStoryWyrmite = 50;
    private const int CharaStoryWyrmite1 = 25;
    private const int CharaStoryWyrmite2 = 10;
    private const int QuestStoryWyrmite = 25;
    private const int DmodeStoryWyrmite = 25;
    private const int Chapter10LastStoryId = 1001009;

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
            logger.LogWarning("Non-existent unit story id {id}", storyId);
            return false;
        }

        if (
            storyData.UnlockQuestStoryId != default
            && (
                await storyRepository.QuestStories.FirstOrDefaultAsync(x =>
                    x.StoryId == storyData.UnlockQuestStoryId
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
                await storyRepository.UnitStories.FirstOrDefaultAsync(x =>
                    x.StoryId == storyData.UnlockTriggerStoryId
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

    public async Task<IList<AtgenBuildEventRewardEntityList>> ReadStory(
        StoryTypes type,
        int storyId
    )
    {
        logger.LogInformation("Reading story {id} of type {type}", storyId, type);

        DbPlayerStoryState story = await storyRepository.GetOrCreateStory(type, storyId);

        if (story.State == StoryState.Read)
        {
            logger.LogDebug("Story was already read");
            return [];
        }

        story.State = StoryState.Read;

        List<AtgenBuildEventRewardEntityList> rewards = type switch
        {
            StoryTypes.Chara or StoryTypes.Dragon => await this.ReadUnitStory(storyId),
            StoryTypes.Castle => await this.ReadCastleStory(storyId),
            StoryTypes.Quest => await this.ReadQuestStory(storyId),
            StoryTypes.Event => await this.ReadEventStory(storyId),
            StoryTypes.DungeonMode => await this.ReadDmodeStory(storyId),
            _ => throw new NotImplementedException($"Stories of type {type} are not implemented")
        };

        logger.LogDebug("Player earned story rewards: {@rewards}", rewards);
        return rewards;
    }

    private async Task<List<AtgenBuildEventRewardEntityList>> ReadUnitStory(int storyId)
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
            if (data.Id == charaStory.StoryIds.Last())
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
        rewardList.Add(new() { EntityType = EntityTypes.Wyrmite, EntityQuantity = wyrmiteReward });
        return rewardList;
    }

    private async Task<List<AtgenBuildEventRewardEntityList>> ReadCastleStory(int storyId)
    {
        await inventoryRepository.UpdateQuantity(Materials.LookingGlass, -1);
        await userDataRepository.GiveWyrmite(CastleStoryWyrmite);

        List<AtgenBuildEventRewardEntityList> rewardList =
            new()
            {
                new()
                {
                    EntityType = EntityTypes.Wyrmite,
                    EntityId = 0,
                    EntityQuantity = CastleStoryWyrmite
                }
            };

        return rewardList;
    }

    private async Task<List<AtgenBuildEventRewardEntityList>> ReadQuestStory(int storyId)
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
                new() { EntityType = EntityTypes.Wyrmite, EntityQuantity = QuestStoryWyrmite }
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
                rewardList.Add(
                    new()
                    {
                        EntityId = reward.Id,
                        EntityType = reward.Type,
                        EntityQuantity = reward.Quantity,
                    }
                );

                // We divert here as we care about quantity-restriction for story plants
                if (reward.Type == EntityTypes.FortPlant)
                {
                    await fortRepository.AddToStorage((FortPlants)reward.Id, reward.Quantity, true);
                    continue;
                }

                if (storyId == Chapter10LastStoryId)
                {
                    presentService.AddPresent(
                        new Present.Present(
                            PresentMessage.Chapter10Clear,
                            (EntityTypes)reward.Type,
                            reward.Id,
                            reward.Quantity
                        )
                    );

                    continue;
                }

                RewardGrantResult result = await rewardService.GrantReward(
                    new Entity(reward.Type, reward.Id, reward.Quantity)
                );

                if (result == RewardGrantResult.Limit)
                {
                    presentService.AddPresent(
                        new Present.Present(
                            PresentMessage.FirstViewReward,
                            reward.Type,
                            reward.Id,
                            reward.Quantity
                        )
                    );

                    if (
                        reward is { Type: EntityTypes.Dragon, Id: (int)Dragons.Midgardsormr }
                        && !await apiContext.PlayerDragonReliability.AnyAsync(x =>
                            x.DragonId == Dragons.Midgardsormr
                        )
                    )
                    {
                        // The game doesn't handle it well if you send the Chapter 1 Midgardsormr to the gift box.
                        // You will later be forced to give him a gift in the dragon's roost tutorial, which will fail
                        // if he's in the gift box. Add the reliability manually as a hack to ensure he's always
                        // available in the dragon's roost.
                        apiContext.PlayerDragonReliability.Add(
                            new(playerIdentityService.ViewerId, Dragons.Midgardsormr)
                        );
                    }
                }
            }
        }

        if (
            MasterAsset.EventData.TryGetValue(story.GroupId, out EventData? eventData)
            && eventData.IsMemoryEvent // Real events need to set is_temporary and do friendship points
            && eventData.GetActualGuestJoinStoryId() == storyId
        )
        {
            logger.LogDebug("Granting memory event character {chara}", eventData.EventCharaId);

            await rewardService.GrantReward(
                new Entity(EntityTypes.Chara, Id: (int)eventData.EventCharaId)
            );
            rewardList.Add(
                new AtgenBuildEventRewardEntityList()
                {
                    EntityId = (int)eventData.EventCharaId,
                    EntityQuantity = 1,
                    EntityType = EntityTypes.Chara
                }
            );
        }

        if (storyId == Chapter10LastStoryId)
        {
            logger.LogDebug("Granting player experience for chapter 10 completion.");
            await userService.AddExperience(69990);
        }

        return rewardList;
    }

    private async Task<List<AtgenBuildEventRewardEntityList>> ReadEventStory(int storyId)
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
                new() { EntityType = EntityTypes.Wyrmite, EntityQuantity = QuestStoryWyrmite }
            };

        // TODO(Events): ??? This is not used for compendium (maybe for collect events)

        return rewardList;
    }

    private async Task<List<AtgenBuildEventRewardEntityList>> ReadDmodeStory(int storyId)
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
                    EntityType = EntityTypes.Wyrmite,
                    EntityId = 0,
                    EntityQuantity = DmodeStoryWyrmite
                },
                dmodePoint1Entity.ToBuildEventRewardEntityList(),
                dmodePoint2Entity.ToBuildEventRewardEntityList()
            };

        return rewardList;
    }

    #endregion


    public static AtgenQuestStoryRewardList ToQuestStoryReward(
        AtgenBuildEventRewardEntityList reward
    )
    {
        AtgenQuestStoryRewardList questReward =
            new()
            {
                EntityId = reward.EntityId,
                EntityType = reward.EntityType,
                EntityQuantity = reward.EntityQuantity
            };

        if (reward.EntityType is EntityTypes.Chara or EntityTypes.Dragon)
            questReward.EntityLevel = 1;

        return questReward;
    }

    public EntityResult GetEntityResult()
    {
        EntityResult result = rewardService.GetEntityResult();
        result.OverPresentEntityList = presentService.AddedPresents.Select(x =>
            x.ToBuildEventRewardList()
        );

        return result;
    }
}
