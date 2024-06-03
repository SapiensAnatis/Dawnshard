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
            Charas.Alex
        }.ToFrozenSet();

    public static FrozenSet<Dragons> DragonList { get; } =
        new List<Dragons>()
        {
            Dragons.Brunhilda,
            Dragons.Mercury,
            Dragons.Midgardsormr,
            Dragons.Jupiter,
            Dragons.Zodiark,
        }.ToFrozenSet();

    public static FrozenDictionary<FortPlants, FortConfig> FortConfigs { get; } =
        new Dictionary<FortPlants, FortConfig>()
        {
            [FortPlants.TheHalidom] = new FortConfig(6, 1, 16, 17),
            [FortPlants.Smithy] = new FortConfig(6, 1, 21, 3),
            [FortPlants.RupieMine] = new FortConfig(15, 4),
            [FortPlants.Dragontree] = new FortConfig(15, 1),
            [FortPlants.FlameAltar] = new FortConfig(10, 2),
            [FortPlants.WaterAltar] = new FortConfig(10, 2),
            [FortPlants.WindAltar] = new FortConfig(10, 2),
            [FortPlants.LightAltar] = new FortConfig(10, 2),
            [FortPlants.ShadowAltar] = new FortConfig(10, 2),
            [FortPlants.SwordDojo] = new FortConfig(10, 2),
            [FortPlants.BladeDojo] = new FortConfig(10, 2),
            [FortPlants.DaggerDojo] = new FortConfig(10, 2),
            [FortPlants.LanceDojo] = new FortConfig(10, 2),
            [FortPlants.AxeDojo] = new FortConfig(10, 2),
            [FortPlants.BowDojo] = new FortConfig(10, 2),
            [FortPlants.WandDojo] = new FortConfig(10, 2),
            [FortPlants.StaffDojo] = new FortConfig(10, 2),
            [FortPlants.ManacasterDojo] = new FortConfig(10, 2),
            [FortPlants.WindDracolith] = new FortConfig(1, 1),
            [FortPlants.WaterDracolith] = new FortConfig(1, 1),
            [FortPlants.FlameDracolith] = new FortConfig(1, 1),
            [FortPlants.LightDracolith] = new FortConfig(1, 1),
            [FortPlants.ShadowDracolith] = new FortConfig(1, 1),
        }.ToFrozenDictionary();
}
