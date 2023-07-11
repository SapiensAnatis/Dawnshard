using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Definitions.Enums.EventItemTypes;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Event;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Event;

public class EventService(
    ILogger<EventService> logger,
    IEventRepository eventRepository,
    IRewardService rewardService
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

    public async Task<IEnumerable<AtgenBuildEventRewardEntityList>> ReceiveEventRewards(
        int eventId,
        IEnumerable<int>? rewardIds = null,
        bool isLocationReward = false
    )
    {
        List<AtgenBuildEventRewardEntityList> rewardEntities = new();

        DbPlayerEventItem buildPointItem = await eventRepository.GetEventItemAsync(
            eventId,
            Event.GetEventRewardItem(eventId)
        );

        Dictionary<int, IEventReward> rewards = Event.GetEventRewards(eventId);

        IEnumerable<int> alreadyObtainedRewardIds = await eventRepository.GetEventRewardIdsAsync(
            eventId,
            isLocationReward
        );

        List<IEventReward> availableRewards = (
            rewardIds != null
                ? rewardIds.Select(x => rewards[x])
                : rewards.Values
                    .ExceptBy(alreadyObtainedRewardIds, x => x.Id)
                    .Where(x => buildPointItem.Quantity >= x.EventItemQuantity)
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

    public async Task CreateEventData(int eventId)
    {
        if (await eventRepository.GetEventDataAsync(eventId) == null)
        {
            logger.LogInformation("Creating event data for event {eventId}", eventId);
            eventRepository.CreateEventData(eventId);
        }

        IEnumerable<int> items = await eventRepository.Items
            .Where(x => x.EventId == eventId)
            .Select(x => x.Id)
            .ToListAsync();

        List<(int Id, int Type)> itemIds = Event
            .GetEventSpecificItemIds(eventId)
            .Zip(Event.GetEventItemTypes(eventId), (x, y) => (x, y))
            .ExceptBy(items, info => info.x)
            .ToList();

        if (itemIds.Count > 0)
            eventRepository.CreateEventItems(eventId, itemIds);
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
        foreach (
            int itemId in Event.GetEventItemTypes(eventId).Where(x => !itemDict.ContainsKey(x))
        )
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

file static class Event
{
    public static IEnumerable<int> GetEventItemTypes(int eventId)
    {
        EventData data = MasterAsset.EventData[eventId];

        return (
            data.EventKindType switch
            {
                EventKindType.Build => Enum.GetValues<BuildEventItemType>().Cast<int>(),
                EventKindType.BattleRoyal => Enum.GetValues<BattleRoyalEventItemType>().Cast<int>(),
                EventKindType.Clb01 => Enum.GetValues<Clb01EventItemType>().Cast<int>(),
                EventKindType.Collect => Enum.GetValues<CollectEventItemType>().Cast<int>(),
                EventKindType.Combat => Enum.GetValues<CombatEventItemType>().Cast<int>(),
                EventKindType.Earn => Enum.GetValues<EarnEventItemType>().Cast<int>(),
                EventKindType.ExHunter => Enum.GetValues<ExHunterEventItemType>().Cast<int>(),
                EventKindType.ExRush => Enum.GetValues<ExRushEventItemType>().Cast<int>(),
                EventKindType.Raid => Enum.GetValues<RaidEventItemType>().Cast<int>(),
                EventKindType.Simple => Enum.GetValues<SimpleEventItemType>().Cast<int>(),
                _ => Enumerable.Empty<int>(),
            }
        ).Where(x => x != 0);
    }

    public static IEnumerable<int> GetEventSpecificItemIds(int eventId)
    {
        EventData data = MasterAsset.EventData[eventId];

        return data.EventKindType switch
        {
            EventKindType.Build
                => MasterAsset.BuildEventItem.Enumerable
                    .Where(x => x.EventId == eventId)
                    .Select(x => x.Id),
            EventKindType.Raid
                => MasterAsset.RaidEventItem.Enumerable
                    .Where(x => x.RaidEventId == eventId)
                    .Select(x => x.Id),
            EventKindType.Combat
                => MasterAsset.CombatEventItem.Enumerable
                    .Where(x => x.EventId == eventId)
                    .Select(x => x.Id),
            EventKindType.BattleRoyal
                => MasterAsset.BattleRoyalEventItem.Enumerable
                    .Where(x => x.EventId == eventId)
                    .Select(x => x.Id),
            EventKindType.Clb01
                => MasterAsset.Clb01EventItem.Enumerable
                    .Where(x => x.EventId == eventId)
                    .Select(x => x.Id),
            EventKindType.Collect
                => MasterAsset.CollectEventItem.Enumerable
                    .Where(x => x.EventId == eventId)
                    .Select(x => x.Id),
            EventKindType.Earn
                => MasterAsset.EarnEventItem.Enumerable
                    .Where(x => x.EventId == eventId)
                    .Select(x => x.Id),
            EventKindType.ExHunter
                => MasterAsset.ExHunterEventItem.Enumerable
                    .Where(x => x.EventId == eventId)
                    .Select(x => x.Id),
            EventKindType.ExRush
                => MasterAsset.ExRushEventItem.Enumerable
                    .Where(x => x.EventId == eventId)
                    .Select(x => x.Id),
            EventKindType.Simple
                => MasterAsset.SimpleEventItem.Enumerable
                    .Where(x => x.EventId == eventId)
                    .Select(x => x.Id),
            _ => Enumerable.Empty<int>(),
        };
    }

    public static Dictionary<int, IEventReward> GetEventRewards(int eventId)
    {
        EventData data = MasterAsset.EventData[eventId];

        return data.EventKindType switch
        {
            EventKindType.Raid
                => MasterAsset.RaidEventReward[eventId]
                    .Cast<IEventReward>()
                    .ToDictionary(x => x.Id, x => x),

            // BuildEventReward is the default
            _
                => MasterAsset.BuildEventReward[eventId]
                    .Cast<IEventReward>()
                    .ToDictionary(x => x.Id, x => x)
        };
    }

    public static int GetEventRewardItem(int eventId)
    {
        EventData data = MasterAsset.EventData[eventId];

        return data.EventKindType switch
        {
            EventKindType.Build => (int)BuildEventItemType.BuildEventPoint,
            EventKindType.Raid => (int)RaidEventItemType.SummonPoint,
            EventKindType.Combat => (int)CombatEventItemType.EventPoint,
            EventKindType.BattleRoyal => (int)BattleRoyalEventItemType.EventPoint,
            EventKindType.Clb01 => (int)Clb01EventItemType.Clb01EventPoint,
            EventKindType.Earn => (int)EarnEventItemType.EarnPoint,
            EventKindType.ExHunter => (int)ExHunterEventItemType.SummonPoint,
            /*
            EventKindType.Collect
                => Enum.GetValues<CollectEventItemType>().Where(x => x != 0).Cast<int>(),
            EventKindType.ExRush
                => Enum.GetValues<ExRushEventItemType>().Where(x => x != 0).Cast<int>(),
            EventKindType.Simple
                => Enum.GetValues<SimpleEventItemType>().Where(x => x != 0).Cast<int>(),*/
            _
                => 0 /* maybe 10101 */
            ,
        };
    }
}
