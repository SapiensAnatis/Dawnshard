using DragaliaAPI.MissionDesigner.Models;
using DragaliaAPI.MissionDesigner.Models.Attributes;

namespace DragaliaAPI.MissionDesigner.Missions.Normal.Wall;

[ContainsMissionList]
public class WallFlame
{
    [MissionType(MissionType.Normal)]
    [WallType(QuestWallTypes.Flame)]
    public static List<Mission> Missions { get; } =
        [
            // Clear The Mercurial Gauntlet (Flame): Lv. 1
            new ClearWallLevelMission() { MissionId = 10010201, Level = 1 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 2
            new ClearWallLevelMission() { MissionId = 10010202, Level = 2 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 3
            new ClearWallLevelMission() { MissionId = 10010203, Level = 3 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 4
            new ClearWallLevelMission() { MissionId = 10010204, Level = 4 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 5
            new ClearWallLevelMission() { MissionId = 10010205, Level = 5 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 6
            new ClearWallLevelMission() { MissionId = 10010206, Level = 6 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 7
            new ClearWallLevelMission() { MissionId = 10010207, Level = 7 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 8
            new ClearWallLevelMission() { MissionId = 10010208, Level = 8 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 9
            new ClearWallLevelMission() { MissionId = 10010209, Level = 9 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 10
            new ClearWallLevelMission() { MissionId = 10010210, Level = 10 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 11
            new ClearWallLevelMission() { MissionId = 10010211, Level = 11 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 12
            new ClearWallLevelMission() { MissionId = 10010212, Level = 12 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 13
            new ClearWallLevelMission() { MissionId = 10010213, Level = 13 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 14
            new ClearWallLevelMission() { MissionId = 10010214, Level = 14 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 15
            new ClearWallLevelMission() { MissionId = 10010215, Level = 15 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 16
            new ClearWallLevelMission() { MissionId = 10010216, Level = 16 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 17
            new ClearWallLevelMission() { MissionId = 10010217, Level = 17 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 18
            new ClearWallLevelMission() { MissionId = 10010218, Level = 18 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 19
            new ClearWallLevelMission() { MissionId = 10010219, Level = 19 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 20
            new ClearWallLevelMission() { MissionId = 10010220, Level = 20 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 21
            new ClearWallLevelMission() { MissionId = 10010221, Level = 21 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 22
            new ClearWallLevelMission() { MissionId = 10010222, Level = 22 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 23
            new ClearWallLevelMission() { MissionId = 10010223, Level = 23 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 24
            new ClearWallLevelMission() { MissionId = 10010224, Level = 24 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 25
            new ClearWallLevelMission() { MissionId = 10010225, Level = 25 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 26
            new ClearWallLevelMission() { MissionId = 10010226, Level = 26 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 27
            new ClearWallLevelMission() { MissionId = 10010227, Level = 27 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 28
            new ClearWallLevelMission() { MissionId = 10010228, Level = 28 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 29
            new ClearWallLevelMission() { MissionId = 10010229, Level = 29 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 30
            new ClearWallLevelMission() { MissionId = 10010230, Level = 30 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 31
            new ClearWallLevelMission() { MissionId = 10010231, Level = 31 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 32
            new ClearWallLevelMission() { MissionId = 10010232, Level = 32 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 33
            new ClearWallLevelMission() { MissionId = 10010233, Level = 33 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 34
            new ClearWallLevelMission() { MissionId = 10010234, Level = 34 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 35
            new ClearWallLevelMission() { MissionId = 10010235, Level = 35 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 36
            new ClearWallLevelMission() { MissionId = 10010236, Level = 36 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 37
            new ClearWallLevelMission() { MissionId = 10010237, Level = 37 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 38
            new ClearWallLevelMission() { MissionId = 10010238, Level = 38 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 39
            new ClearWallLevelMission() { MissionId = 10010239, Level = 39 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 40
            new ClearWallLevelMission() { MissionId = 10010240, Level = 40 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 41
            new ClearWallLevelMission() { MissionId = 10010241, Level = 41 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 42
            new ClearWallLevelMission() { MissionId = 10010242, Level = 42 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 43
            new ClearWallLevelMission() { MissionId = 10010243, Level = 43 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 44
            new ClearWallLevelMission() { MissionId = 10010244, Level = 44 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 45
            new ClearWallLevelMission() { MissionId = 10010245, Level = 45 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 46
            new ClearWallLevelMission() { MissionId = 10010246, Level = 46 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 47
            new ClearWallLevelMission() { MissionId = 10010247, Level = 47 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 48
            new ClearWallLevelMission() { MissionId = 10010248, Level = 48 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 49
            new ClearWallLevelMission() { MissionId = 10010249, Level = 49 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 50
            new ClearWallLevelMission() { MissionId = 10010250, Level = 50 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 51
            new ClearWallLevelMission() { MissionId = 10010251, Level = 51 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 52
            new ClearWallLevelMission() { MissionId = 10010252, Level = 52 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 53
            new ClearWallLevelMission() { MissionId = 10010253, Level = 53 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 54
            new ClearWallLevelMission() { MissionId = 10010254, Level = 54 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 55
            new ClearWallLevelMission() { MissionId = 10010255, Level = 55 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 56
            new ClearWallLevelMission() { MissionId = 10010256, Level = 56 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 57
            new ClearWallLevelMission() { MissionId = 10010257, Level = 57 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 58
            new ClearWallLevelMission() { MissionId = 10010258, Level = 58 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 59
            new ClearWallLevelMission() { MissionId = 10010259, Level = 59 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 60
            new ClearWallLevelMission() { MissionId = 10010260, Level = 60 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 61
            new ClearWallLevelMission() { MissionId = 10010261, Level = 61 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 62
            new ClearWallLevelMission() { MissionId = 10010262, Level = 62 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 63
            new ClearWallLevelMission() { MissionId = 10010263, Level = 63 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 64
            new ClearWallLevelMission() { MissionId = 10010264, Level = 64 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 65
            new ClearWallLevelMission() { MissionId = 10010265, Level = 65 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 66
            new ClearWallLevelMission() { MissionId = 10010266, Level = 66 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 67
            new ClearWallLevelMission() { MissionId = 10010267, Level = 67 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 68
            new ClearWallLevelMission() { MissionId = 10010268, Level = 68 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 69
            new ClearWallLevelMission() { MissionId = 10010269, Level = 69 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 70
            new ClearWallLevelMission() { MissionId = 10010270, Level = 70 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 71
            new ClearWallLevelMission() { MissionId = 10010271, Level = 71 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 72
            new ClearWallLevelMission() { MissionId = 10010272, Level = 72 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 73
            new ClearWallLevelMission() { MissionId = 10010273, Level = 73 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 74
            new ClearWallLevelMission() { MissionId = 10010274, Level = 74 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 75
            new ClearWallLevelMission() { MissionId = 10010275, Level = 75 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 76
            new ClearWallLevelMission() { MissionId = 10010276, Level = 76 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 77
            new ClearWallLevelMission() { MissionId = 10010277, Level = 77 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 78
            new ClearWallLevelMission() { MissionId = 10010278, Level = 78 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 79
            new ClearWallLevelMission() { MissionId = 10010279, Level = 79 },
            // Clear The Mercurial Gauntlet (Flame): Lv. 80
            new ClearWallLevelMission() { MissionId = 10010280, Level = 80 },
        ];
}
