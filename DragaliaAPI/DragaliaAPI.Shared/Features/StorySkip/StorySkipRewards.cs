using System.Collections.Frozen;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.Features.StorySkip;

public static class StorySkipRewards
{
    public readonly struct FortConfig(
        int level,
        int buildCount,
        int positionX = -1,
        int positionZ = -1
    )
    {
        public int BuildCount { get; } = buildCount;
        public int Level { get; } = level;
        public int PositionX { get; } = positionX;
        public int PositionZ { get; } = positionZ;
    }

    public static FrozenSet<Charas> CharasList { get; } =
        new List<Charas>()
        {
            Charas.Elisanne,
            Charas.Ranzal,
            Charas.Cleo,
            Charas.Luca,
            Charas.Alex,
        }.ToFrozenSet();

    public static FrozenSet<DragonId> DragonList { get; } =
        new List<DragonId>()
        {
            DragonId.Brunhilda,
            DragonId.Mercury,
            DragonId.Midgardsormr,
            DragonId.Jupiter,
            DragonId.Zodiark,
        }.ToFrozenSet();

    public static FrozenDictionary<FortPlants, FortConfig> FortConfigs { get; } =
        new Dictionary<FortPlants, FortConfig>()
        {
            [FortPlants.TheHalidom] = new(6, 1, 16, 17),
            [FortPlants.Smithy] = new(6, 1, 21, 3),
            [FortPlants.RupieMine] = new(15, 4),
            [FortPlants.Dragontree] = new(15, 1),
            [FortPlants.FlameAltar] = new(10, 2),
            [FortPlants.WaterAltar] = new(10, 2),
            [FortPlants.WindAltar] = new(10, 2),
            [FortPlants.LightAltar] = new(10, 2),
            [FortPlants.ShadowAltar] = new(10, 2),
            [FortPlants.SwordDojo] = new(10, 2),
            [FortPlants.BladeDojo] = new(10, 2),
            [FortPlants.DaggerDojo] = new(10, 2),
            [FortPlants.LanceDojo] = new(10, 2),
            [FortPlants.AxeDojo] = new(10, 2),
            [FortPlants.BowDojo] = new(10, 2),
            [FortPlants.WandDojo] = new(10, 2),
            [FortPlants.StaffDojo] = new(10, 2),
            [FortPlants.ManacasterDojo] = new(10, 2),
            [FortPlants.WindDracolith] = new(1, 1),
            [FortPlants.WaterDracolith] = new(1, 1),
            [FortPlants.FlameDracolith] = new(1, 1),
            [FortPlants.LightDracolith] = new(1, 1),
            [FortPlants.ShadowDracolith] = new(1, 1),
        }.ToFrozenDictionary();
}
