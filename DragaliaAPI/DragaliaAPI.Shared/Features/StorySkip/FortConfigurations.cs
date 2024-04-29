using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.Features.StorySkip;

public static class FortConfigurations
{
    public struct FortConfig
    {
        public int BuildCount { get; set; }
        public int Level { get; set; }
        public FortConfig(int level, int buildCount)
        {
            BuildCount = buildCount;
            Level = level;
        }
    }

    public static readonly Dictionary<FortPlants, FortConfig> FortConfigs =
        new()
        {
            [FortPlants.TheHalidom] = new FortConfig(6, 1),
            [FortPlants.Smithy] = new FortConfig(6, 1),
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