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
        {
            return second;
        }

        first.reward_record.take_coin += second.reward_record.take_coin;
        first.reward_record.drop_all.AddRange(second.reward_record.drop_all);

        first.grow_record.take_chara_exp += second.grow_record.take_chara_exp;
        first.grow_record.take_player_exp += second.grow_record.take_player_exp;
        first.grow_record.take_mana += second.grow_record.take_mana;

        first.reward_record.quest_bonus_list = first.reward_record.quest_bonus_list.Concat(
            second.reward_record.quest_bonus_list
        );

        first.reward_record.player_level_up_fstone += second.reward_record.player_level_up_fstone;

        return first;
    }

    public static UpdateDataList CombineWith(this UpdateDataList? first, UpdateDataList second)
    {
        if (first == null)
        {
            first = second;
            return first;
        }

        /*
         * Combining a massive type like UpdateDataList is a huge endeavour, but here we are only going to combine
         * things that are relevant for dungeon clears.
         *
         * TODO:
         * - Mission notices
         * - Event data
         */

        first.material_list = CombineNullableLists(first.material_list, second.material_list);
        first.ability_crest_list = CombineNullableLists(
            first.ability_crest_list,
            second.ability_crest_list
        );

        return first;
    }

    private static List<TElement>? CombineNullableLists<TElement>(
        List<TElement>? first,
        IEnumerable<TElement>? second
    )
    {
        if (first == null)
            return second?.ToList();

        if (second == null)
            return first;

        first.AddRange(second);
        return first;
    }
}
