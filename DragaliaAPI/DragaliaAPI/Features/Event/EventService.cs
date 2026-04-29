using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Features.Shared.Reward;
using DragaliaAPI.Infrastructure;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Definitions.Enums.EventItemTypes;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.MasterAsset.Models.Event;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Event;

public partial class EventService(
    ILogger<EventService> logger,
    ApiContext apiContext,
    IPlayerIdentityService playerIdentityService,
    IEventRepository eventRepository,
    IRewardService rewardService,
    IMissionProgressionService missionProgressionService,
    IMissionService missionService
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

        return new(
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
                : rewards
                    .Values.ExceptBy(alreadyObtainedRewardIds, x => x.Id)
                    .Where(x =>
                        eventItemQuantities.ContainsKey(x.EventItemId)
                        && eventItemQuantities[x.EventItemId] >= x.EventItemQuantity
                    )
        ).ToList();

        if (availableRewards.Count == 0)
        {
            return rewardEntities;
        }

        Log.GrantingRewardsForEvent(logger, eventId, availableRewards.Select(x => x.Id));

        foreach (IEventReward reward in availableRewards)
        {
            Entity entity = new(
                reward.RewardEntityType,
                reward.RewardEntityId,
                reward.RewardEntityQuantity
            );
            _ = await rewardService.GrantReward(entity);

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
            await apiContext.PlayerQuests.SingleOrDefaultAsync(x =>
                x.QuestId == location.ClearQuestId
            )
            is not { State: 3 }
        )
        {
            return rewardEntities;
        }

        List<CombatEventLocationReward> rewards = MasterAsset
            .CombatEventLocationReward.Enumerable.Where(x =>
                x.EventId == eventId && x.LocationRewardId == location.LocationRewardId
            )
            .ToList();

        Log.GrantedLocationRewardForEvent(logger, locationId, eventId, rewards);

        foreach (CombatEventLocationReward reward in rewards)
        {
            Entity entity = new(reward.EntityType, reward.EntityId, reward.EntityQuantity);

            _ = await rewardService.GrantReward(entity);

            rewardEntities.Add(entity.ToBuildEventRewardEntityList());
        }

        eventRepository.CreateEventReward(eventId, locationId);

        return rewardEntities;
    }

    private static readonly Dictionary<int, List<QuestData>> CombatEventQuestLookup = MasterAsset
        .EventData.Enumerable.Where(x => x.EventKindType == EventKindType.Combat)
        .Select(x => x.Id)
        .ToDictionary(
            x => x,
            x => MasterAsset.QuestData.Enumerable.Where(y => y.Gid == x).ToList()
        );

    public async Task CreateEventData(int eventId)
    {
        missionProgressionService.OnEventParticipation(eventId);

        bool firstEventEnter = false;
        EventData data = MasterAsset.EventData[eventId];

        if (await eventRepository.GetEventDataAsync(eventId) == null)
        {
            Log.CreatingEventDataForEvent(logger, eventId);
            eventRepository.CreateEventData(eventId);

            if (data.IsMemoryEvent)
            {
                await missionService.UnlockMemoryEventMissions(eventId);
            }
            else
            {
                await missionService.UnlockEventMissions(eventId);
            }

            firstEventEnter = true;
        }

        IEnumerable<int> items = await eventRepository
            .Items.Where(x => x.EventId == eventId)
            .Select(x => x.Id)
            .ToListAsync();

        IEnumerable<(int Id, int Type)> eventSpecificItemIds = data.GetEventSpecificItemIds();

        List<(int Id, int Type)> itemIds = eventSpecificItemIds
            .ExceptBy(items, info => info.Id)
            .ToList();

        if (itemIds.Count > 0)
        {
            eventRepository.CreateEventItems(eventId, itemIds);
        }

        IEnumerable<int> currentEventPassiveIds = await eventRepository
            .Passives.Where(x => x.EventId == eventId)
            .Select(x => x.PassiveId)
            .ToListAsync();

        List<int> neededEventPassiveIds = data.GetEventPassiveIds()
            .Except(currentEventPassiveIds)
            .ToList();

        if (neededEventPassiveIds.Count > 0)
        {
            eventRepository.CreateEventPassives(eventId, neededEventPassiveIds);
        }

        // Doing this at the end so that everything should've been created
        if (data.EventKindType == EventKindType.Combat && firstEventEnter)
        {
            await GrantAlreadyEarnedLocationRewards(eventId);
        }

        if (firstEventEnter && MasterAsset.BoxSummonData.Enumerable.Any(x => x.EventId == eventId))
        {
            InitializeEventSummon(eventId);
        }
    }

    private async Task GrantAlreadyEarnedLocationRewards(int eventId)
    {
        HashSet<int> relevantQuestIds = CombatEventQuestLookup[eventId]
            .Select(x => x.Id)
            .ToHashSet();

        List<int> completedQuestIds = await apiContext
            .PlayerQuests.Where(x => x.State == 3 && relevantQuestIds.Contains(x.QuestId))
            .Select(x => x.QuestId)
            .ToListAsync();

        foreach (
            (int entityId, int entityQuantity) in CombatEventQuestLookup[eventId]
                .Where(x =>
                    completedQuestIds.Contains(x.Id)
                    && x is { HoldEntityType: EntityTypes.CombatEventItem, HoldEntityQuantity: > 0 }
                )
                .ToLookup(x => x.HoldEntityId, x => x.HoldEntityQuantity)
                .ToDictionary(x => x.Key, x => x.Max())
        )
        {
            Log.GrantingItemDueToCompletedQuests(logger, entityQuantity, entityId, eventId);

            _ = await rewardService.GrantReward(
                new Entity(EntityTypes.CombatEventItem, entityId, entityQuantity)
            );
        }

        foreach (
            int locationId in MasterAsset
                .CombatEventLocation.Enumerable.Where(x =>
                    x.EventId == eventId && completedQuestIds.Contains(x.ClearQuestId)
                )
                .Select(x => x.Id)
        )
        {
            Log.CompletingLocationRewardDueToCompletedQuests(logger, locationId, eventId);

            eventRepository.CreateEventReward(eventId, locationId);
        }
    }

    private void InitializeEventSummon(int eventId)
    {
        Log.CreatingEventSummonData(logger, eventId);

        apiContext.PlayerEventSummonData.Add(
            new() { ViewerId = playerIdentityService.ViewerId, EventId = eventId }
        );
    }

    private async Task<DbPlayerEventData> GetEventData(int eventId)
    {
        return await eventRepository.GetEventDataAsync(eventId)
            ?? throw new DragaliaException(ResultCode.CommonDbError, "No event data found");
    }

    public async Task<IEnumerable<CharaFriendshipList>> GetEventCharaFriendshipList(int eventId)
    {
        Charas eventCharaId = MasterAsset.EventData[eventId].EventCharaId;
        if (eventCharaId == Charas.Empty)
        {
            return [];
        }

        int maxFriendshipPoint = MasterAsset.CharaData[eventCharaId].MaxFriendshipPoint;

        var dbChara = await apiContext
            .PlayerCharaData.Where(x => x.CharaId == eventCharaId)
            .Select(x => new { x.IsTemporary, x.FriendshipPoint })
            .FirstOrDefaultAsync();

        if (dbChara is null)
        {
            return [];
        }

        return
        [
            new CharaFriendshipList()
            {
                CharaId = eventCharaId,
                AddPoint = 0,
                TotalPoint = dbChara.IsTemporary ? dbChara.FriendshipPoint : maxFriendshipPoint,
                IsTemporary = dbChara.IsTemporary,
            },
        ];
    }

    #region User Data Providers

    private async Task<Dictionary<int, int>> GetEventItemDictionary(int eventId)
    {
        Dictionary<int, int> itemDict = await eventRepository.GetEventItemQuantityAsync(eventId);

        return itemDict;
    }

    public async Task<BuildEventUserList?> GetBuildEventUserData(int eventId)
    {
        IEnumerable<DbPlayerEventItem> eventItems = await eventRepository.GetEventItemsAsync(
            eventId
        );

        return new(
            eventId,
            eventItems.Select(x => new AtgenUserBuildEventItemList(x.Type, x.Quantity))
        );
    }

    public async Task<RaidEventUserList?> GetRaidEventUserData(int eventId)
    {
        if (!await eventRepository.HasEventDataAsync(eventId))
        {
            // Send client to /entry
            return null;
        }

        Dictionary<int, int> itemDict = await GetEventItemDictionary(eventId);

        return new(
            raidEventId: eventId,
            boxSummonPoint: itemDict.GetValueOrDefault((int)RaidEventItemType.SummonPoint, 0),
            raidPoint1: itemDict.GetValueOrDefault((int)RaidEventItemType.RaidPoint1, 0),
            raidPoint2: itemDict.GetValueOrDefault((int)RaidEventItemType.RaidPoint2, 0),
            raidPoint3: itemDict.GetValueOrDefault((int)RaidEventItemType.RaidPoint3, 0),
            adventItemQuantity1: itemDict.GetValueOrDefault((int)RaidEventItemType.AdventItem1, 0),
            adventItemQuantity2: itemDict.GetValueOrDefault((int)RaidEventItemType.AdventItem2, 0),
            ultimateKeyCount: itemDict.GetValueOrDefault((int)RaidEventItemType.UltimateItem1, 0),
            exchangeItemCount: itemDict.GetValueOrDefault((int)RaidEventItemType.ExchangeItem1, 0),
            exchangeItemCount2: itemDict.GetValueOrDefault((int)RaidEventItemType.ExchangeItem2, 0)
        );
    }

    public async Task<Clb01EventUserList?> GetClb01EventUserData(int eventId)
    {
        if (!await eventRepository.HasEventDataAsync(eventId))
        {
            return null;
        }

        Dictionary<int, int> itemDict = await GetEventItemDictionary(eventId);

        return new(eventId, itemDict.Select(x => new AtgenUserClb01EventItemList(x.Key, x.Value)));
    }

    public async Task<CollectEventUserList?> GetCollectEventUserData(int eventId)
    {
        if (!await eventRepository.HasEventDataAsync(eventId))
        {
            return null;
        }

        Dictionary<int, int> itemDict = await GetEventItemDictionary(eventId);

        return new(
            eventId,
            itemDict.Select(x => new AtgenUserCollectEventItemList(x.Key, x.Value))
        );
    }

    public async Task<CombatEventUserList?> GetCombatEventUserData(int eventId)
    {
        if (!await eventRepository.HasEventDataAsync(eventId))
        {
            return null;
        }

        Dictionary<int, int> itemDict = await GetEventItemDictionary(eventId);

        return new(
            eventId,
            itemDict.GetValueOrDefault((int)CombatEventItemType.EventPoint, 0),
            itemDict.GetValueOrDefault((int)CombatEventItemType.ExchangeItem, 0),
            itemDict.GetValueOrDefault((int)CombatEventItemType.QuestUnlock, 0),
            itemDict.GetValueOrDefault((int)CombatEventItemType.StoryUnlock, 0),
            itemDict.GetValueOrDefault((int)CombatEventItemType.AdventItem, 0)
        );
    }

    public async Task<EarnEventUserList?> GetEarnEventUserData(int eventId)
    {
        if (!await eventRepository.HasEventDataAsync(eventId))
        {
            return null;
        }

        Dictionary<int, int> itemDict = await GetEventItemDictionary(eventId);

        return new(
            eventId,
            itemDict.GetValueOrDefault((int)EarnEventItemType.EarnPoint, 0),
            itemDict.GetValueOrDefault((int)EarnEventItemType.ExchangeItem1, 0),
            itemDict.GetValueOrDefault((int)EarnEventItemType.ExchangeItem2, 0),
            itemDict.GetValueOrDefault((int)EarnEventItemType.AdventItem, 0)
        );
    }

    public async Task<ExHunterEventUserList?> GetExHunterEventUserData(int eventId)
    {
        if (!await eventRepository.HasEventDataAsync(eventId))
        {
            return null;
        }

        Dictionary<int, int> itemDict = await GetEventItemDictionary(eventId);

        return new(
            eventId,
            itemDict.GetValueOrDefault((int)ExHunterEventItemType.SummonPoint, 0),
            itemDict.GetValueOrDefault((int)ExHunterEventItemType.ExHunterPoint1, 0),
            itemDict.GetValueOrDefault((int)ExHunterEventItemType.ExHunterPoint2, 0),
            itemDict.GetValueOrDefault((int)ExHunterEventItemType.ExHunterPoint3, 0),
            itemDict.GetValueOrDefault((int)ExHunterEventItemType.AdventItem1, 0),
            itemDict.GetValueOrDefault((int)ExHunterEventItemType.AdventItem2, 0),
            itemDict.GetValueOrDefault((int)ExHunterEventItemType.UltimateItem, 0),
            itemDict.GetValueOrDefault((int)ExHunterEventItemType.ExchangeItem01, 0),
            itemDict.GetValueOrDefault((int)ExHunterEventItemType.ExchangeItem02, 0)
        );
    }

    public async Task<ExRushEventUserList?> GetExRushEventUserData(int eventId)
    {
        if (!await eventRepository.HasEventDataAsync(eventId))
        {
            return null;
        }

        Dictionary<int, int> itemDict = await GetEventItemDictionary(eventId);

        return new(
            eventId,
            itemDict.GetValueOrDefault((int)ExRushEventItemType.ExRushPoint1, 0),
            itemDict.GetValueOrDefault((int)ExRushEventItemType.ExRushPoint2, 0)
        );
    }

    public async Task<MazeEventUserList?> GetMazeEventUserData(int eventId)
    {
        if (!await eventRepository.HasEventDataAsync(eventId))
        {
            return null;
        }

        Dictionary<int, int> itemDict = await GetEventItemDictionary(eventId);

        return new(eventId, itemDict.Select(x => new AtgenUserMazeEventItemList(x.Key, x.Value)));
    }

    public async Task<SimpleEventUserList?> GetSimpleEventUserData(int eventId)
    {
        if (!await eventRepository.HasEventDataAsync(eventId))
        {
            return null;
        }

        Dictionary<int, int> itemDict = await GetEventItemDictionary(eventId);

        return new(
            eventId,
            itemDict.GetValueOrDefault((int)SimpleEventItemType.ExchangeItem1, 0),
            itemDict.GetValueOrDefault((int)SimpleEventItemType.ExchangeItem2, 0),
            itemDict.GetValueOrDefault((int)SimpleEventItemType.ExchangeItem3, 0)
        );
    }

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Debug, "Granting rewards for event {eventId}: {@rewards}")]
        public static partial void GrantingRewardsForEvent(
            ILogger logger,
            int eventId,
            IEnumerable<int> rewards
        );

        [LoggerMessage(
            LogLevel.Debug,
            "Granted location reward {locationId} for event {eventId}: {@rewards}"
        )]
        public static partial void GrantedLocationRewardForEvent(
            ILogger logger,
            int locationId,
            int eventId,
            List<CombatEventLocationReward> rewards
        );

        [LoggerMessage(LogLevel.Information, "Creating event data for event {eventId}")]
        public static partial void CreatingEventDataForEvent(ILogger logger, int eventId);

        [LoggerMessage(
            LogLevel.Debug,
            "Granting {quantity}x {itemId} ({eventId}) due to completed quests"
        )]
        public static partial void GrantingItemDueToCompletedQuests(
            ILogger logger,
            int quantity,
            int itemId,
            int eventId
        );

        [LoggerMessage(
            LogLevel.Debug,
            "Completing location reward {rewardId} ({eventId}) due to completed quests"
        )]
        public static partial void CompletingLocationRewardDueToCompletedQuests(
            ILogger logger,
            int rewardId,
            int eventId
        );

        [LoggerMessage(LogLevel.Debug, "Creating event summon data for event ID {eventId}")]
        public static partial void CreatingEventSummonData(ILogger logger, int eventId);
    }

    #endregion
}
