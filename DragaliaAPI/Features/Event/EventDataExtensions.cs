using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Definitions.Enums.EventItemTypes;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Event;

namespace DragaliaAPI.Features.Event;

internal static class EventDataExtensions
{
    public static IEnumerable<int> GetEventItemTypes(this EventData data)
    {
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

    public static IEnumerable<int> GetEventSpecificItemIds(this EventData data)
    {
        int eventId = data.Id;

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

    public static Dictionary<int, IEventReward> GetEventRewards(this EventData data)
    {
        int eventId = data.Id;

        return data.EventKindType switch
        {
            EventKindType.Raid
            or EventKindType.ExHunter
                => MasterAsset.RaidEventReward[eventId].Values
                    .Cast<IEventReward>()
                    .ToDictionary(x => x.Id, x => x),

            // BuildEventReward is the default
            _
                => MasterAsset.BuildEventReward[eventId].Values
                    .Cast<IEventReward>()
                    .ToDictionary(x => x.Id, x => x)
        };
    }

    public static IEnumerable<int> GetEventPassiveIds(this EventData data)
    {
        int eventId = data.Id;

        return MasterAsset.EventPassive.Enumerable
            .Where(x => x.EventId == eventId)
            .Select(x => x.Id);
    }
}
