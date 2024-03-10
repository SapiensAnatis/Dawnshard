using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.Dungeon;

public static class Extensions
{
    /// <summary>
    /// Combine two instances of <see cref="IngameResultData"/>. Modifies <paramref name="first"/> to include the data
    /// in both <paramref name="first"/> and <paramref name="second"/>.
    /// </summary>
    /// <param name="first">First instance of <see cref="IngameResultData"/>.</param>
    /// <param name="second">Second instance of <see cref="IngameResultData"/>.</param>
    /// <returns><paramref name="first"/> after combining.</returns>
    public static IngameResultData CombineWith(
        this IngameResultData? first,
        IngameResultData second
    )
    {
        if (first == null)
            return second;

        first.RewardRecord = first.RewardRecord.CombineWith(second.RewardRecord);
        first.GrowRecord = first.GrowRecord.CombineWith(second.GrowRecord);

        first.ConvertedEntityList = first
            .ConvertedEntityList.Concat(second.ConvertedEntityList)
            .Merge();

        return first;
    }

    public static UpdateDataList CombineWith(this UpdateDataList? first, UpdateDataList second)
    {
        if (first == null)
            return second;

        /*
         * Combining a massive type like UpdateDataList is a huge endeavour, but here we are only going to combine
         * things that are relevant for dungeon clears.
         *
         * TODO:
         * - Mission notices
         */

        first.MaterialList = UnionNullableLists(
            second.MaterialList, // Second goes first for more up-to-date quantities
            first.MaterialList,
            m => m.MaterialId
        )
            ?.ToList();
        first.AbilityCrestList = UnionNullableLists(
            first.AbilityCrestList,
            second.AbilityCrestList,
            a => a.AbilityCrestId
        )
            ?.ToList();

        // Properties that will always be more up to date in the more recent list
        first.UserData = second.UserData;
        first.QuestList = second.QuestList;
        first.BuildEventUserList = second.BuildEventUserList;
        first.EarnEventUserList = second.EarnEventUserList;
        first.RaidEventUserList = second.RaidEventUserList;
        first.EventPassiveList = second.EventPassiveList;
        first.Clb01EventUserList = second.Clb01EventUserList;
        first.CombatEventUserList = second.CombatEventUserList;
        first.ExHunterEventUserList = second.ExHunterEventUserList;
        first.ExRushEventUserList = second.ExRushEventUserList;

        return first;
    }

    public static IEnumerable<ConvertedEntityList> Merge(
        this IEnumerable<ConvertedEntityList> source
    ) =>
        source
            .GroupBy(x => new
            {
                before_entity_id = x.BeforeEntityId,
                before_entity_type = x.BeforeEntityType,
                after_entity_id = x.AfterEntityId,
                after_entity_type = x.AfterEntityType
            })
            .Select(group =>
                group.Aggregate(
                    new ConvertedEntityList
                    {
                        BeforeEntityId = group.Key.before_entity_id,
                        BeforeEntityType = group.Key.before_entity_type,
                        AfterEntityId = group.Key.after_entity_id,
                        AfterEntityType = group.Key.after_entity_type
                    },
                    (acc, current) =>
                    {
                        acc.BeforeEntityQuantity += current.BeforeEntityQuantity;
                        acc.AfterEntityQuantity += current.AfterEntityQuantity;
                        return acc;
                    }
                )
            );

    private static IEnumerable<AtgenDropAll> Merge(this IEnumerable<AtgenDropAll> source) =>
        source
            .GroupBy(x => new { type = x.Type, id = x.Id, })
            .Select(group =>
                group.Aggregate(
                    new AtgenDropAll { Id = group.Key.id, Type = group.Key.type, },
                    (acc, current) =>
                    {
                        acc.Quantity += current.Quantity;
                        return acc;
                    }
                )
            );

    private static IEnumerable<TElement>? UnionNullableLists<TElement, TKey>(
        IEnumerable<TElement>? first,
        IEnumerable<TElement>? second,
        Func<TElement, TKey> keySelector
    )
    {
        if (first == null)
            return second;

        if (second == null)
            return first;

        return first.UnionBy(second, keySelector);
    }

    private static RewardRecord CombineWith(this RewardRecord first, RewardRecord second)
    {
        first.TakeCoin += second.TakeCoin;
        first.DropAll = first.DropAll.Concat(second.DropAll).Merge().ToList();

        first.QuestBonusList = first.QuestBonusList.Concat(second.QuestBonusList);

        first.TakeAccumulatePoint += second.TakeAccumulatePoint;
        first.TakeBoostAccumulatePoint += second.TakeBoostAccumulatePoint;

        first.PlayerLevelUpFstone += second.PlayerLevelUpFstone;

        return first;
    }

    private static GrowRecord CombineWith(this GrowRecord first, GrowRecord second)
    {
        first.TakeCharaExp += second.TakeCharaExp;
        first.TakePlayerExp += second.TakePlayerExp;
        first.TakeMana += second.TakeMana;

        return first;
    }
}
