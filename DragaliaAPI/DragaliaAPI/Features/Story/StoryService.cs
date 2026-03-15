using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Fort;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Features.Player;
using DragaliaAPI.Features.Present;
using DragaliaAPI.Features.Shared.Reward;
using DragaliaAPI.Features.Shop;
using DragaliaAPI.Features.Tutorial;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Features.Presents;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Event;
using DragaliaAPI.Shared.MasterAsset.Models.Story;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Story;

public partial class StoryService(
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
        Log.CheckingEligibilityForStoryOfType(logger, storyId, type);
        DbPlayerStoryState story = await storyRepository.GetOrCreateStory(type, storyId);

        if (story.State == StoryState.Read)
        {
            Log.StoryWasAlreadyRead(logger);
            return true;
        }

        return type switch
        {
            StoryTypes.Chara or StoryTypes.Dragon => await this.CheckUnitStoryEligibility(storyId),
            StoryTypes.Castle => await this.CheckCastleStoryEligibility(),
            StoryTypes.Quest => true,
            StoryTypes.Event => true,
            _ => throw new NotImplementedException($"Stories of type {type} are not implemented"),
        };
    }

    private async Task<bool> CheckUnitStoryEligibility(int storyId)
    {
        if (!MasterAsset.UnitStory.TryGetValue(storyId, out UnitStory? storyData))
        {
            Log.NonExistentUnitStoryId(logger, storyId);
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
            Log.PlayerWasMissingRequiredQuestStoryId(logger);
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
            Log.PlayerWasMissingRequiredUnitStoryId(logger);
            return false;
        }

        return true;
    }

    private async Task<bool> CheckCastleStoryEligibility()
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
        Log.ReadingStoryOfType(logger, storyId, type);

        DbPlayerStoryState story = await storyRepository.GetOrCreateStory(type, storyId);

        if (story.State == StoryState.Read)
        {
            Log.StoryWasAlreadyRead(logger);
            return [];
        }

        story.State = StoryState.Read;

        List<AtgenBuildEventRewardEntityList> rewards = type switch
        {
            StoryTypes.Chara or StoryTypes.Dragon => await this.ReadUnitStory(storyId),
            StoryTypes.Castle => await this.ReadCastleStory(),
            StoryTypes.Quest => await this.ReadQuestStory(storyId),
            StoryTypes.Event => await this.ReadEventStory(storyId),
            StoryTypes.DungeonMode => await this.ReadDmodeStory(),
            _ => throw new NotImplementedException($"Stories of type {type} are not implemented"),
        };

        Log.PlayerEarnedStoryRewards(logger, rewards);
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
                emblemRewardEntity = new(EntityTypes.Title, emblemRewardId, 1);
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

    private async Task<List<AtgenBuildEventRewardEntityList>> ReadCastleStory()
    {
        await inventoryRepository.UpdateQuantity(Materials.LookingGlass, -1);
        await userDataRepository.GiveWyrmite(CastleStoryWyrmite);

        List<AtgenBuildEventRewardEntityList> rewardList = new()
        {
            new()
            {
                EntityType = EntityTypes.Wyrmite,
                EntityId = 0,
                EntityQuantity = CastleStoryWyrmite,
            },
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
        List<AtgenBuildEventRewardEntityList> rewardList = new()
        {
            new() { EntityType = EntityTypes.Wyrmite, EntityQuantity = QuestStoryWyrmite },
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
                    new(reward.Type, reward.Id, reward.Quantity)
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
                        reward is { Type: EntityTypes.Dragon, Id: (int)DragonId.Midgardsormr }
                        && !await apiContext.PlayerDragonReliability.AnyAsync(x =>
                            x.DragonId == DragonId.Midgardsormr
                        )
                    )
                    {
                        // The game doesn't handle it well if you send the Chapter 1 Midgardsormr to the gift box.
                        // You will later be forced to give him a gift in the dragon's roost tutorial, which will fail
                        // if he's in the gift box. Add the reliability manually as a hack to ensure he's always
                        // available in the dragon's roost.
                        apiContext.PlayerDragonReliability.Add(
                            new(playerIdentityService.ViewerId, DragonId.Midgardsormr)
                        );
                    }
                }
            }
        }

        if (
            MasterAsset.EventData.TryGetValue(story.GroupId, out EventData? eventData)
            && eventData.IsMemoryEvent // Real events need to set is_temporary and do friendship points
            && eventData.GuestJoinStoryId == storyId
        )
        {
            Log.GrantingMemoryEventCharacter(logger, eventData.EventCharaId);

            await rewardService.GrantReward(
                new(EntityTypes.Chara, Id: (int)eventData.EventCharaId)
            );
            rewardList.Add(
                new()
                {
                    EntityId = (int)eventData.EventCharaId,
                    EntityQuantity = 1,
                    EntityType = EntityTypes.Chara,
                }
            );
        }

        if (storyId == Chapter10LastStoryId)
        {
            Log.GrantingPlayerExperienceForChapter10Completion(logger);
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
        List<AtgenBuildEventRewardEntityList> rewardList = new()
        {
            new() { EntityType = EntityTypes.Wyrmite, EntityQuantity = QuestStoryWyrmite },
        };

        // TODO(Events): ??? This is not used for compendium (maybe for collect events)

        return rewardList;
    }

    private async Task<List<AtgenBuildEventRewardEntityList>> ReadDmodeStory()
    {
        await userDataRepository.GiveWyrmite(DmodeStoryWyrmite);

        // Temporary measure to make fafnir upgrades more obtainable until endeavours are added
        Entity dmodePoint1Entity = new(
            EntityTypes.DmodePoint,
            Id: (int)DmodePoint.Point1,
            Quantity: 5_000
        );
        Entity dmodePoint2Entity = new(
            EntityTypes.DmodePoint,
            Id: (int)DmodePoint.Point2,
            Quantity: 1_000
        );
        await rewardService.GrantReward(dmodePoint1Entity);
        await rewardService.GrantReward(dmodePoint2Entity);

        List<AtgenBuildEventRewardEntityList> rewardList = new()
        {
            new()
            {
                EntityType = EntityTypes.Wyrmite,
                EntityId = 0,
                EntityQuantity = DmodeStoryWyrmite,
            },
            dmodePoint1Entity.ToBuildEventRewardEntityList(),
            dmodePoint2Entity.ToBuildEventRewardEntityList(),
        };

        return rewardList;
    }

    #endregion

    public static AtgenQuestStoryRewardList ToQuestStoryReward(
        AtgenBuildEventRewardEntityList reward
    )
    {
        AtgenQuestStoryRewardList questReward = new()
        {
            EntityId = reward.EntityId,
            EntityType = reward.EntityType,
            EntityQuantity = reward.EntityQuantity,
        };

        if (reward.EntityType is EntityTypes.Chara or EntityTypes.Dragon)
        {
            questReward.EntityLevel = 1;
        }

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

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Debug, "Checking eligibility for story {id} of type: {type}")]
        public static partial void CheckingEligibilityForStoryOfType(
            ILogger logger,
            int id,
            StoryTypes type
        );

        [LoggerMessage(LogLevel.Debug, "Story was already read")]
        public static partial void StoryWasAlreadyRead(ILogger logger);

        [LoggerMessage(LogLevel.Warning, "Non-existent unit story id {id}")]
        public static partial void NonExistentUnitStoryId(ILogger logger, int id);

        [LoggerMessage(LogLevel.Warning, "Player was missing required quest story id")]
        public static partial void PlayerWasMissingRequiredQuestStoryId(ILogger logger);

        [LoggerMessage(LogLevel.Warning, "Player was missing required unit story id")]
        public static partial void PlayerWasMissingRequiredUnitStoryId(ILogger logger);

        [LoggerMessage(LogLevel.Information, "Reading story {id} of type {type}")]
        public static partial void ReadingStoryOfType(ILogger logger, int id, StoryTypes type);

        [LoggerMessage(LogLevel.Debug, "Player earned story rewards: {@rewards}")]
        public static partial void PlayerEarnedStoryRewards(
            ILogger logger,
            List<AtgenBuildEventRewardEntityList> rewards
        );

        [LoggerMessage(LogLevel.Debug, "Granting memory event character {chara}")]
        public static partial void GrantingMemoryEventCharacter(ILogger logger, Charas chara);

        [LoggerMessage(LogLevel.Debug, "Granting player experience for chapter 10 completion.")]
        public static partial void GrantingPlayerExperienceForChapter10Completion(ILogger logger);
    }
}
