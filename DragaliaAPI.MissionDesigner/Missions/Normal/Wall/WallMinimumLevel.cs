using DragaliaAPI.MissionDesigner.Models;
using DragaliaAPI.MissionDesigner.Models.Attributes;

namespace DragaliaAPI.MissionDesigner.Missions.Normal.Wall;

[ContainsMissionList]
public class WallMinimumLevel
{
    [MissionType(MissionType.Normal)]
    public static List<Mission> Missions { get; } =
        [
            // Clear The Mercurial Gauntlet
            new ClearWallLevelMission() { MissionId = 10010101, Level = 1 },
            // Clear Lv. 2 of The Mercurial Gauntlet in All Elements
            new ClearWallLevelMission() { MissionId = 10010701, Level = 2 },
            // Clear Lv. 4 of The Mercurial Gauntlet in All Elements
            new ClearWallLevelMission() { MissionId = 10010702, Level = 4 },
            // Clear Lv. 6 of The Mercurial Gauntlet in All Elements
            new ClearWallLevelMission() { MissionId = 10010703, Level = 6 },
            // Clear Lv. 8 of The Mercurial Gauntlet in All Elements
            new ClearWallLevelMission() { MissionId = 10010704, Level = 8 },
            // Clear Lv. 10 of The Mercurial Gauntlet in All Elements
            new ClearWallLevelMission() { MissionId = 10010705, Level = 10 },
            // Clear Lv. 12 of The Mercurial Gauntlet in All Elements
            new ClearWallLevelMission() { MissionId = 10010706, Level = 12 },
            // Clear Lv. 14 of The Mercurial Gauntlet in All Elements
            new ClearWallLevelMission() { MissionId = 10010707, Level = 14 },
            // Clear Lv. 16 of The Mercurial Gauntlet in All Elements
            new ClearWallLevelMission() { MissionId = 10010708, Level = 16 },
            // Clear Lv. 18 of The Mercurial Gauntlet in All Elements
            new ClearWallLevelMission() { MissionId = 10010709, Level = 18 },
            // Clear Lv. 20 of The Mercurial Gauntlet in All Elements
            new ClearWallLevelMission() { MissionId = 10010710, Level = 20 },
            // Clear Lv. 22 of The Mercurial Gauntlet in All Elements
            new ClearWallLevelMission() { MissionId = 10010711, Level = 22 },
            // Clear Lv. 24 of The Mercurial Gauntlet in All Elements
            new ClearWallLevelMission() { MissionId = 10010712, Level = 24 },
            // Clear Lv. 26 of The Mercurial Gauntlet in All Elements
            new ClearWallLevelMission() { MissionId = 10010713, Level = 26 },
            // Clear Lv. 28 of The Mercurial Gauntlet in All Elements
            new ClearWallLevelMission() { MissionId = 10010714, Level = 28 },
            // Clear Lv. 30 of The Mercurial Gauntlet in All Elements
            new ClearWallLevelMission() { MissionId = 10010715, Level = 30 },
            // Clear Lv. 32 of The Mercurial Gauntlet in All Elements
            new ClearWallLevelMission() { MissionId = 10010716, Level = 32 },
            // Clear Lv. 34 of The Mercurial Gauntlet in All Elements
            new ClearWallLevelMission() { MissionId = 10010717, Level = 34 },
            // Clear Lv. 36 of The Mercurial Gauntlet in All Elements
            new ClearWallLevelMission() { MissionId = 10010718, Level = 36 },
            // Clear Lv. 38 of The Mercurial Gauntlet in All Elements
            new ClearWallLevelMission() { MissionId = 10010719, Level = 38 },
            // Clear Lv. 40 of The Mercurial Gauntlet in All Elements
            new ClearWallLevelMission() { MissionId = 10010720, Level = 40 },
            // Clear Lv. 42 of The Mercurial Gauntlet in All Elements
            new ClearWallLevelMission() { MissionId = 10010721, Level = 42 },
            // Clear Lv. 44 of The Mercurial Gauntlet in All Elements
            new ClearWallLevelMission() { MissionId = 10010722, Level = 44 },
            // Clear Lv. 46 of The Mercurial Gauntlet in All Elements
            new ClearWallLevelMission() { MissionId = 10010723, Level = 46 },
            // Clear Lv. 48 of The Mercurial Gauntlet in All Elements
            new ClearWallLevelMission() { MissionId = 10010724, Level = 48 },
            // Clear Lv. 50 of The Mercurial Gauntlet in All Elements
            new ClearWallLevelMission() { MissionId = 10010725, Level = 50 },
            // Clear Lv. 52 of The Mercurial Gauntlet in All Elements
            new ClearWallLevelMission() { MissionId = 10010726, Level = 52 },
            // Clear Lv. 54 of The Mercurial Gauntlet in All Elements
            new ClearWallLevelMission() { MissionId = 10010727, Level = 54 },
            // Clear Lv. 56 of The Mercurial Gauntlet in All Elements
            new ClearWallLevelMission() { MissionId = 10010728, Level = 56 },
            // Clear Lv. 58 of The Mercurial Gauntlet in All Elements
            new ClearWallLevelMission() { MissionId = 10010729, Level = 58 },
            // Clear Lv. 60 of The Mercurial Gauntlet in All Elements
            new ClearWallLevelMission() { MissionId = 10010730, Level = 60 },
            // Clear Lv. 62 of The Mercurial Gauntlet in All Elements
            new ClearWallLevelMission() { MissionId = 10010731, Level = 62 },
            // Clear Lv. 64 of The Mercurial Gauntlet in All Elements
            new ClearWallLevelMission() { MissionId = 10010732, Level = 64 },
            // Clear Lv. 66 of The Mercurial Gauntlet in All Elements
            new ClearWallLevelMission() { MissionId = 10010733, Level = 66 },
            // Clear Lv. 68 of The Mercurial Gauntlet in All Elements
            new ClearWallLevelMission() { MissionId = 10010734, Level = 68 },
            // Clear Lv. 70 of The Mercurial Gauntlet in All Elements
            new ClearWallLevelMission() { MissionId = 10010735, Level = 70 },
            // Clear Lv. 72 of The Mercurial Gauntlet in All Elements
            new ClearWallLevelMission() { MissionId = 10010736, Level = 72 },
            // Clear Lv. 74 of The Mercurial Gauntlet in All Elements
            new ClearWallLevelMission() { MissionId = 10010737, Level = 74 },
            // Clear Lv. 76 of The Mercurial Gauntlet in All Elements
            new ClearWallLevelMission() { MissionId = 10010738, Level = 76 },
            // Clear Lv. 78 of The Mercurial Gauntlet in All Elements
            new ClearWallLevelMission() { MissionId = 10010739, Level = 78 },
            // Clear Lv. 80 of The Mercurial Gauntlet in All Elements
            new ClearWallLevelMission() { MissionId = 10010740, Level = 80 },
        ];
}
