using System.Collections.Frozen;
using System.Collections.Immutable;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Dmode;

namespace DragaliaAPI.Features.DmodeDungeon;

public static class DmodeHelper
{
    public static readonly ImmutableDictionary<
        DmodeServitorPassiveType,
        ImmutableDictionary<int, DmodeServitorPassiveLevel>
    > PassiveLevels = MasterAsset
        .DmodeServitorPassiveLevel.Enumerable.ToLookup(x => x.PassiveNum)
        .ToImmutableDictionary(x => x.Key, x => x.ToImmutableDictionary(y => y.Level, y => y));

    public static void AddDmodeItem(
        this ICollection<DmodeDungeonItemList> list,
        DmodeDungeonItemList item
    )
    {
        item.ItemNo = list.Count + 1;
        list.Add(item);
    }

    public static FrozenSet<int> AllowedSkillIds { get; } =
        new HashSet<int>()
        {
            104503022, // A Splash of Affection
            108503011, // Akashic Repose
            106505041, // Aria of the Sea
            109504023, // Armed 17: Scramble
            102404011, // Blossom Flash
            104504031, // Bursting Fury
            104505031, // Candy Cane Offensive
            109503021, // Dirge Shot
            108305011, // Impeccable Service
            103503011, // Ninja Bride Sweep
            105502042, // Perfect Order
            105304012, // Pride of the Forge
            101501031, // Road to Glory
            109502022, // Shanao Strike
            104501012, // Smith Shield
            109501011, // Sovereign Barrage
            104503041, // Splendid Spring
            107402011, // Starfrost Swell
            107501011, // Study Break
            106502011, // Vivid Volley
            105503011, // Woodland Spear
        }.ToFrozenSet();
}
