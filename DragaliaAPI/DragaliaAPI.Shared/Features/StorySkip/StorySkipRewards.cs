using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.Features.StorySkip;

public static class StorySkipRewards
{
    public struct FortConfig
    {
        public int BuildCount { get; }
        public int Level { get; }
        public int PositionX { get; }
        public int PositionZ { get; }

        public FortConfig(int level, int buildCount, int positionX = -1, int positionZ = -1)
        {
            BuildCount = buildCount;
            Level = level;
            PositionX = positionX;
            PositionZ = positionZ;
        }
    }

    public static readonly List<Charas> CharasList =
        new() { Charas.Elisanne, Charas.Ranzal, Charas.Cleo, Charas.Luca, Charas.Alex };

    public static readonly List<Dragons> DragonList =
        new()
        {
            Dragons.Brunhilda,
            Dragons.Mercury,
            Dragons.Midgardsormr,
            Dragons.Jupiter,
            Dragons.Zodiark,
        };

    public static readonly Dictionary<FortPlants, FortConfig> FortConfigs =
        new()
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
            [FortPlants.ManacasterDojo] = new FortConfig(10, 2)
        };
}
