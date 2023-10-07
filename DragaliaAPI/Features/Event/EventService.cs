using System.Diagnostics.CodeAnalysis;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Helpers;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Definitions.Enums.EventItemTypes;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.MasterAsset.Models.Event;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Event;

public class EventService(
    ILogger<EventService> logger,
    IEventRepository eventRepository,
    IRewardService rewardService,
    IQuestRepository questRepository
) : IEventService
{
    public async Task<bool> GetCustomEventFlag(int eventId)
    {
        DbPlayerEventData eventData = await GetEventData(eventId);

        return eventData.CustomEventFlag;
    }

    /*public async Task<UserEventItemData> GetUserEventItemData(int eventId)
    {
        IEnumerable<DbPlayerEventItem> items = await eventRepository.GetEventItemsAsync(eventId);

        return new UserEventItemData(items.Select(x => new AtgenUserMazeEventItemList__2(x.ItemId, x.Quantity)));
    }*/

    public async Task<IEnumerable<T>> GetEventRewardList<T>(
        int eventId,
        bool isLocationReward = false
    )
        where T : IEventRewardList<T>
    {
        IEnumerable<DbPlayerEventReward> rewards = await eventRepository.GetEventRewardsAsync(
            eventId,
            isLocationReward
        );

        return rewards.Select(T.FromDatabase);
    }

    public async Task<EventPassiveList> GetEventPassiveList(int eventId)
    {
        IEnumerable<DbPlayerEventPassive> passives = await eventRepository.GetEventPassivesAsync(
            eventId
        );

        return new EventPassiveList(
            eventId,
            passives.Select(x => new AtgenEventPassiveUpList(x.PassiveId, x.Progress))
        );
    }

    public async Task<IEnumerable<AtgenBuildEventRewardEntityList>> ReceiveEventRewards(
        int eventId,
        IEnumerable<int>? rewardIds = null
    )
    {
        EventData data = MasterAsset.EventData[eventId];

        List<AtgenBuildEventRewardEntityList> rewardEntities = new();

        Dictionary<int, int> eventItemQuantities = (
            await eventRepository.GetEventItemsAsync(eventId)
        ).ToDictionary(x => x.Id, x => x.Quantity);

        Dictionary<int, IEventReward> rewards = data.GetEventRewards();

        IEnumerable<int> alreadyObtainedRewardIds = await eventRepository.GetEventRewardIdsAsync(
            eventId
        );

        List<IEventReward> availableRewards = (
            rewardIds != null
                ? rewardIds.Select(x => rewards[x])
                : rewards.Values
                    .ExceptBy(alreadyObtainedRewardIds, x => x.Id)
                    .Where(
                        x =>
                            eventItemQuantities.ContainsKey(x.EventItemId)
                            && eventItemQuantities[x.EventItemId] >= x.EventItemQuantity
                    )
        ).ToList();

        if (availableRewards.Count == 0)
        {
            return rewardEntities;
        }

        logger.LogDebug(
            "Granting rewards for event {eventId}: {@rewards}",
            eventId,
            availableRewards.Select(x => x.Id)
        );

        foreach (IEventReward reward in availableRewards)
        {
            Entity entity =
                new(reward.RewardEntityType, reward.RewardEntityId, reward.RewardEntityQuantity);
            await rewardService.GrantReward(entity);

            eventRepository.CreateEventReward(eventId, reward.Id);
            rewardEntities.Add(entity.ToBuildEventRewardEntityList());
        }

        return rewardEntities;
    }

    public async Task<IEnumerable<AtgenBuildEventRewardEntityList>> ReceiveEventLocationReward(
        int eventId,
        int locationId
    )
    {
        List<AtgenBuildEventRewardEntityList> rewardEntities = new();

        CombatEventLocation location = MasterAsset.CombatEventLocation[locationId];
        if (
            await questRepository.Quests.SingleOrDefaultAsync(
                x => x.QuestId == location.ClearQuestId
            )
            is not { State: 3 }
        )
        {
            return rewardEntities;
        }

        List<CombatEventLocationReward> rewards = MasterAsset.CombatEventLocationReward.Enumerable
            .Where(x => x.EventId == eventId && x.LocationRewardId == location.LocationRewardId)
            .ToList();

        logger.LogDebug(
            "Granted location reward {locationId} for event {eventId}: {@rewards}",
            locationId,
            eventId,
            rewards
        );

        foreach (CombatEventLocationReward reward in rewards)
        {
            Entity entity = new(reward.EntityType, reward.EntityId, reward.EntityQuantity);

            await rewardService.GrantReward(entity);

            rewardEntities.Add(entity.ToBuildEventRewardEntityList());
        }

        eventRepository.CreateEventReward(eventId, locationId);

        return rewardEntities;
    }

    private static readonly Dictionary<int, List<QuestData>> CombatEventQuestLookup =
        MasterAsset.EventData.Enumerable
            .Where(x => x.EventKindType == EventKindType.Combat)
            .Select(x => x.Id)
            .ToDictionary(
                x => x,
                x => MasterAsset.QuestData.Enumerable.Where(y => y.Gid == x).ToList()
            );

    public async Task CreateEventData(int eventId)
    {
        bool firstEventEnter = false;

        if (await eventRepository.GetEventDataAsync(eventId) == null)
        {
            logger.LogInformation("Creating event data for event {eventId}", eventId);
            eventRepository.CreateEventData(eventId);
            firstEventEnter = true;
        }

        EventData data = MasterAsset.EventData[eventId];

        IEnumerable<int> items = await eventRepository.Items
            .Where(x => x.EventId == eventId)
            .Select(x => x.Id)
            .ToListAsync();

        List<(int Id, int Type)> itemIds = data.GetEventSpecificItemIds()
            .Zip(data.GetEventItemTypes(), (x, y) => (x, y))
            .ExceptBy(items, info => info.x)
            .ToList();

        if (itemIds.Count > 0)
            eventRepository.CreateEventItems(eventId, itemIds);

        IEnumerable<int> currentEventPassiveIds = await eventRepository.Passives
            .Where(x => x.EventId == eventId)
            .Select(x => x.PassiveId)
            .ToListAsync();

        List<int> neededEventPassiveIds = data.GetEventPassiveIds()
            .Except(currentEventPassiveIds)
            .ToList();

        if (neededEventPassiveIds.Count > 0)
            eventRepository.CreateEventPassives(eventId, neededEventPassiveIds);

        // Doing this at the end so that everything should've been created
        if (data.EventKindType == EventKindType.Combat && firstEventEnter)
        {
            HashSet<int> relevantQuestIds = CombatEventQuestLookup[data.Id]
                .Select(x => x.Id)
                .ToHashSet();

            List<int> completedQuestIds = await questRepository.Quests
                .Where(x => x.State == 3 && relevantQuestIds.Contains(x.QuestId))
                .Select(x => x.QuestId)
                .ToListAsync();

            foreach (
                (int entityId, int entityQuantity) in CombatEventQuestLookup[data.Id]
                    .Where(
                        x =>
                            completedQuestIds.Contains(x.Id)
                            && x
                                is {
                                    HoldEntityType: EntityTypes.CombatEventItem,
                                    HoldEntityQuantity: > 0
                                }
                    )
                    .ToLookup(x => x.HoldEntityId, x => x.HoldEntityQuantity)
                    .ToDictionary(x => x.Key, x => x.Max())
            )
            {
                logger.LogDebug(
                    "Granting {quantity}x {itemId} ({eventId}) due to completed quests",
                    entityQuantity,
                    entityId,
                    eventId
                );

                await rewardService.GrantReward(
                    new Entity(EntityTypes.CombatEventItem, entityId, entityQuantity)
                );
            }

            foreach (
                int locationId in MasterAsset.CombatEventLocation.Enumerable
                    .Where(x => x.EventId == eventId && completedQuestIds.Contains(x.ClearQuestId))
                    .Select(x => x.Id)
            )
            {
                logger.LogDebug(
                    "Completing location reward {rewardId} ({eventId}) due to completed quests",
                    locationId,
                    eventId
                );

                eventRepository.CreateEventReward(eventId, locationId);
            }
        }
    }

    private async Task<DbPlayerEventData> GetEventData(int eventId)
    {
        return await eventRepository.GetEventDataAsync(eventId)
            ?? throw new DragaliaException(ResultCode.CommonDbError, "No event data found");
    }

    #region User Data Providers

    private async Task<Dictionary<int, int>> GetEventItemDictionary(int eventId)
    {
        Dictionary<int, int> itemDict = await eventRepository.GetEventItemQuantityAsync(eventId);

        EventData data = MasterAsset.EventData[eventId];

        foreach (int itemId in data.GetEventItemTypes().Where(x => !itemDict.ContainsKey(x)))
        {
            itemDict[itemId] = 0;
        }

        return itemDict;
    }

    public async Task<BuildEventUserList> GetBuildEventUserData(int eventId)
    {
        IEnumerable<DbPlayerEventItem> eventItems = await eventRepository.GetEventItemsAsync(
            eventId
        );

        return new BuildEventUserList(
            eventId,
            eventItems.Select(x => new AtgenUserBuildEventItemList(x.Type, x.Quantity))
        );
    }

    public async Task<RaidEventUserList> GetRaidEventUserData(int eventId)
    {
        Dictionary<int, int> itemDict = await GetEventItemDictionary(eventId);

        return new RaidEventUserList(
            eventId,
            itemDict[(int)RaidEventItemType.SummonPoint],
            itemDict[(int)RaidEventItemType.RaidPoint1],
            itemDict[(int)RaidEventItemType.RaidPoint2],
            itemDict[(int)RaidEventItemType.RaidPoint3],
            itemDict[(int)RaidEventItemType.AdventItem1],
            itemDict[(int)RaidEventItemType.AdventItem2],
            itemDict[(int)RaidEventItemType.UltimateItem1],
            itemDict[(int)RaidEventItemType.ExchangeItem1],
            itemDict[(int)RaidEventItemType.ExchangeItem2]
        );
    }

    public async Task<Clb01EventUserList> GetClb01EventUserData(int eventId)
    {
        Dictionary<int, int> itemDict = await GetEventItemDictionary(eventId);

        return new Clb01EventUserList(
            eventId,
            itemDict.Select(x => new AtgenUserClb01EventItemList(x.Key, x.Value))
        );
    }

    public async Task<CollectEventUserList> GetCollectEventUserData(int eventId)
    {
        Dictionary<int, int> itemDict = await GetEventItemDictionary(eventId);

        return new CollectEventUserList(
            eventId,
            itemDict.Select(x => new AtgenUserCollectEventItemList(x.Key, x.Value))
        );
    }

    public async Task<CombatEventUserList> GetCombatEventUserData(int eventId)
    {
        Dictionary<int, int> itemDict = await GetEventItemDictionary(eventId);

        return new CombatEventUserList(
            eventId,
            itemDict[(int)CombatEventItemType.EventPoint],
            itemDict[(int)CombatEventItemType.ExchangeItem],
            itemDict[(int)CombatEventItemType.QuestUnlock],
            itemDict[(int)CombatEventItemType.StoryUnlock],
            itemDict[(int)CombatEventItemType.AdventItem]
        );
    }

    public async Task<EarnEventUserList> GetEarnEventUserData(int eventId)
    {
        Dictionary<int, int> itemDict = await GetEventItemDictionary(eventId);

        return new EarnEventUserList(
            eventId,
            itemDict[(int)EarnEventItemType.EarnPoint],
            itemDict[(int)EarnEventItemType.ExchangeItem1],
            itemDict[(int)EarnEventItemType.ExchangeItem2],
            itemDict[(int)EarnEventItemType.AdventItem]
        );
    }

    public async Task<ExHunterEventUserList> GetExHunterEventUserData(int eventId)
    {
        Dictionary<int, int> itemDict = await GetEventItemDictionary(eventId);

        return new ExHunterEventUserList(
            eventId,
            itemDict[(int)ExHunterEventItemType.SummonPoint],
            itemDict[(int)ExHunterEventItemType.ExHunterPoint1],
            itemDict[(int)ExHunterEventItemType.ExHunterPoint2],
            itemDict[(int)ExHunterEventItemType.ExHunterPoint3],
            itemDict[(int)ExHunterEventItemType.AdventItem1],
            itemDict[(int)ExHunterEventItemType.AdventItem2],
            itemDict[(int)ExHunterEventItemType.UltimateItem],
            itemDict[(int)ExHunterEventItemType.ExchangeItem01],
            itemDict[(int)ExHunterEventItemType.ExchangeItem02]
        );
    }

    public async Task<ExRushEventUserList> GetExRushEventUserData(int eventId)
    {
        Dictionary<int, int> itemDict = await GetEventItemDictionary(eventId);

        return new ExRushEventUserList(
            eventId,
            itemDict[(int)ExRushEventItemType.ExRushPoint1],
            itemDict[(int)ExRushEventItemType.ExRushPoint2]
        );
    }

    public async Task<MazeEventUserList> GetMazeEventUserData(int eventId)
    {
        Dictionary<int, int> itemDict = await GetEventItemDictionary(eventId);

        return new MazeEventUserList(
            eventId,
            itemDict.Select(x => new AtgenUserMazeEventItemList(x.Key, x.Value))
        );
    }

    public async Task<SimpleEventUserList> GetSimpleEventUserData(int eventId)
    {
        Dictionary<int, int> itemDict = await GetEventItemDictionary(eventId);

        return new SimpleEventUserList(
            eventId,
            itemDict[(int)SimpleEventItemType.ExchangeItem1],
            itemDict[(int)SimpleEventItemType.ExchangeItem2],
            itemDict[(int)SimpleEventItemType.ExchangeItem3]
        );
    }

    #endregion
}
