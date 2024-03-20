using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Event;

namespace DragaliaAPI.Features.Event;

internal static class EventDataExtensions
{
    public static IEnumerable<(int Id, int Type)> GetEventSpecificItemIds(this EventData data)
    {
        int eventId = data.Id;

        return data.EventKindType switch
        {
            EventKindType.Build
                => MasterAsset
                    .BuildEventItem.Enumerable.Where(x => x.EventId == eventId)
                    .Select(x => (x.Id, (int)x.EventItemType)),
            EventKindType.Raid
                => MasterAsset
                    .RaidEventItem.Enumerable.Where(x => x.RaidEventId == eventId)
                    .Select(x => (x.Id, (int)x.RaidEventItemType)),
            EventKindType.Combat
                => MasterAsset
                    .CombatEventItem.Enumerable.Where(x => x.EventId == eventId)
                    .Select(x => (x.Id, (int)x.EventItemType)),
            EventKindType.BattleRoyal
                => MasterAsset
                    .BattleRoyalEventItem.Enumerable.Where(x => x.EventId == eventId)
                    .Select(x => (x.Id, (int)x.EventItemType)),
            EventKindType.Clb01
                => MasterAsset
                    .Clb01EventItem.Enumerable.Where(x => x.EventId == eventId)
                    .Select(x => (x.Id, (int)x.EventItemType)),
            EventKindType.Collect
                => MasterAsset
                    .CollectEventItem.Enumerable.Where(x => x.EventId == eventId)
                    .Select(x => (x.Id, (int)x.EventItemType)),
            EventKindType.Earn
                => MasterAsset
                    .EarnEventItem.Enumerable.Where(x => x.EventId == eventId)
                    .Select(x => (x.Id, (int)x.EventItemType)),
            EventKindType.ExHunter
                => MasterAsset
                    .ExHunterEventItem.Enumerable.Where(x => x.EventId == eventId)
                    .Select(x => (x.Id, (int)x.EventItemType)),
            EventKindType.ExRush
                => MasterAsset
                    .ExRushEventItem.Enumerable.Where(x => x.EventId == eventId)
                    .Select(x => (x.Id, (int)x.EventItemType)),
            EventKindType.Simple
                => MasterAsset
                    .SimpleEventItem.Enumerable.Where(x => x.EventId == eventId)
                    .Select(x => (x.Id, (int)x.EventItemType)),
            _ => [],
        };
    }

    public static Dictionary<int, IEventReward> GetEventRewards(this EventData data)
    {
        int eventId = data.Id;

        return data.EventKindType switch
        {
            EventKindType.Raid
            or EventKindType.ExHunter
                => MasterAsset
                    .RaidEventReward[eventId]
                    .Values.Cast<IEventReward>()
                    .ToDictionary(x => x.Id, x => x),

            // BuildEventReward is the default
            _
                => MasterAsset
                    .BuildEventReward[eventId]
                    .Values.Cast<IEventReward>()
                    .ToDictionary(x => x.Id, x => x)
        };
    }

    public static IEnumerable<int> GetEventPassiveIds(this EventData data)
    {
        int eventId = data.Id;

        return MasterAsset
            .EventPassive.Enumerable.Where(x => x.EventId == eventId)
            .Select(x => x.Id);
    }
}
