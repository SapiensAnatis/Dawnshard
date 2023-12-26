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

        first.reward_record = first.reward_record.CombineWith(second.reward_record);
        first.grow_record = first.grow_record.CombineWith(second.grow_record);

        first.converted_entity_list = first
            .converted_entity_list.Concat(second.converted_entity_list)
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

        first.material_list = UnionNullableLists(
            second.material_list, // Second goes first for more up-to-date quantities
            first.material_list,
            m => m.material_id
        );
        first.ability_crest_list = UnionNullableLists(
            first.ability_crest_list,
            second.ability_crest_list,
            a => a.ability_crest_id
        );

        // Properties that will always be more up to date in the more recent list
        first.user_data = second.user_data;
        first.quest_list = second.quest_list;
        first.build_event_user_list = second.build_event_user_list;
        first.earn_event_user_list = second.earn_event_user_list;
        first.raid_event_user_list = second.raid_event_user_list;
        first.event_passive_list = second.event_passive_list;
        first.clb_01_event_user_list = second.clb_01_event_user_list;
        first.combat_event_user_list = second.combat_event_user_list;
        first.ex_hunter_event_user_list = second.ex_hunter_event_user_list;
        first.ex_rush_event_user_list = second.ex_rush_event_user_list;

        return first;
    }

    public static IEnumerable<ConvertedEntityList> Merge(
        this IEnumerable<ConvertedEntityList> source
    ) =>
        source
            .GroupBy(
                x =>
                    new
                    {
                        x.before_entity_id,
                        x.before_entity_type,
                        x.after_entity_id,
                        x.after_entity_type
                    }
            )
            .Select(
                group =>
                    group.Aggregate(
                        new ConvertedEntityList
                        {
                            before_entity_id = group.Key.before_entity_id,
                            before_entity_type = group.Key.before_entity_type,
                            after_entity_id = group.Key.after_entity_id,
                            after_entity_type = group.Key.after_entity_type
                        },
                        (acc, current) =>
                        {
                            acc.before_entity_quantity += current.before_entity_quantity;
                            acc.after_entity_quantity += current.after_entity_quantity;
                            return acc;
                        }
                    )
            );

    private static IEnumerable<AtgenDropAll> Merge(this IEnumerable<AtgenDropAll> source) =>
        source
            .GroupBy(x => new { x.type, x.id, })
            .Select(
                group =>
                    group.Aggregate(
                        new AtgenDropAll { id = group.Key.id, type = group.Key.type, },
                        (acc, current) =>
                        {
                            acc.quantity += current.quantity;
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
        first.take_coin += second.take_coin;
        first.drop_all = first.drop_all.Concat(second.drop_all).Merge().ToList();

        first.quest_bonus_list = first.quest_bonus_list.Concat(second.quest_bonus_list);

        first.take_accumulate_point += second.take_accumulate_point;
        first.take_boost_accumulate_point += second.take_boost_accumulate_point;

        first.player_level_up_fstone += second.player_level_up_fstone;

        return first;
    }

    private static GrowRecord CombineWith(this GrowRecord first, GrowRecord second)
    {
        first.take_chara_exp += second.take_chara_exp;
        first.take_player_exp += second.take_player_exp;
        first.take_mana += second.take_mana;

        return first;
    }
}
