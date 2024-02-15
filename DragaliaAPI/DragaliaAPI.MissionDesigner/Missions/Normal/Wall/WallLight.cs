using DragaliaAPI.MissionDesigner.Models;
using DragaliaAPI.MissionDesigner.Models.Attributes;

namespace DragaliaAPI.MissionDesigner.Missions.Normal.Wall;

[ContainsMissionList]
public class WallLight
{
    [MissionType(MissionType.Normal)]
    [WallType(QuestWallTypes.Light)]
    public static List<Mission> Missions { get; } =
        [
            // Clear The Mercurial Gauntlet (Light): Lv. 1
            new ClearWallLevelMission() { MissionId = 10010501, Level = 1 },
            // Clear The Mercurial Gauntlet (Light): Lv. 2
            new ClearWallLevelMission() { MissionId = 10010502, Level = 2 },
            // Clear The Mercurial Gauntlet (Light): Lv. 3
            new ClearWallLevelMission() { MissionId = 10010503, Level = 3 },
            // Clear The Mercurial Gauntlet (Light): Lv. 4
            new ClearWallLevelMission() { MissionId = 10010504, Level = 4 },
            // Clear The Mercurial Gauntlet (Light): Lv. 5
            new ClearWallLevelMission() { MissionId = 10010505, Level = 5 },
            // Clear The Mercurial Gauntlet (Light): Lv. 6
            new ClearWallLevelMission() { MissionId = 10010506, Level = 6 },
            // Clear The Mercurial Gauntlet (Light): Lv. 7
            new ClearWallLevelMission() { MissionId = 10010507, Level = 7 },
            // Clear The Mercurial Gauntlet (Light): Lv. 8
            new ClearWallLevelMission() { MissionId = 10010508, Level = 8 },
            // Clear The Mercurial Gauntlet (Light): Lv. 9
            new ClearWallLevelMission() { MissionId = 10010509, Level = 9 },
            // Clear The Mercurial Gauntlet (Light): Lv. 10
            new ClearWallLevelMission() { MissionId = 10010510, Level = 10 },
            // Clear The Mercurial Gauntlet (Light): Lv. 11
            new ClearWallLevelMission() { MissionId = 10010511, Level = 11 },
            // Clear The Mercurial Gauntlet (Light): Lv. 12
            new ClearWallLevelMission() { MissionId = 10010512, Level = 12 },
            // Clear The Mercurial Gauntlet (Light): Lv. 13
            new ClearWallLevelMission() { MissionId = 10010513, Level = 13 },
            // Clear The Mercurial Gauntlet (Light): Lv. 14
            new ClearWallLevelMission() { MissionId = 10010514, Level = 14 },
            // Clear The Mercurial Gauntlet (Light): Lv. 15
            new ClearWallLevelMission() { MissionId = 10010515, Level = 15 },
            // Clear The Mercurial Gauntlet (Light): Lv. 16
            new ClearWallLevelMission() { MissionId = 10010516, Level = 16 },
            // Clear The Mercurial Gauntlet (Light): Lv. 17
            new ClearWallLevelMission() { MissionId = 10010517, Level = 17 },
            // Clear The Mercurial Gauntlet (Light): Lv. 18
            new ClearWallLevelMission() { MissionId = 10010518, Level = 18 },
            // Clear The Mercurial Gauntlet (Light): Lv. 19
            new ClearWallLevelMission() { MissionId = 10010519, Level = 19 },
            // Clear The Mercurial Gauntlet (Light): Lv. 20
            new ClearWallLevelMission() { MissionId = 10010520, Level = 20 },
            // Clear The Mercurial Gauntlet (Light): Lv. 21
            new ClearWallLevelMission() { MissionId = 10010521, Level = 21 },
            // Clear The Mercurial Gauntlet (Light): Lv. 22
            new ClearWallLevelMission() { MissionId = 10010522, Level = 22 },
            // Clear The Mercurial Gauntlet (Light): Lv. 23
            new ClearWallLevelMission() { MissionId = 10010523, Level = 23 },
            // Clear The Mercurial Gauntlet (Light): Lv. 24
            new ClearWallLevelMission() { MissionId = 10010524, Level = 24 },
            // Clear The Mercurial Gauntlet (Light): Lv. 25
            new ClearWallLevelMission() { MissionId = 10010525, Level = 25 },
            // Clear The Mercurial Gauntlet (Light): Lv. 26
            new ClearWallLevelMission() { MissionId = 10010526, Level = 26 },
            // Clear The Mercurial Gauntlet (Light): Lv. 27
            new ClearWallLevelMission() { MissionId = 10010527, Level = 27 },
            // Clear The Mercurial Gauntlet (Light): Lv. 28
            new ClearWallLevelMission() { MissionId = 10010528, Level = 28 },
            // Clear The Mercurial Gauntlet (Light): Lv. 29
            new ClearWallLevelMission() { MissionId = 10010529, Level = 29 },
            // Clear The Mercurial Gauntlet (Light): Lv. 30
            new ClearWallLevelMission() { MissionId = 10010530, Level = 30 },
            // Clear The Mercurial Gauntlet (Light): Lv. 31
            new ClearWallLevelMission() { MissionId = 10010531, Level = 31 },
            // Clear The Mercurial Gauntlet (Light): Lv. 32
            new ClearWallLevelMission() { MissionId = 10010532, Level = 32 },
            // Clear The Mercurial Gauntlet (Light): Lv. 33
            new ClearWallLevelMission() { MissionId = 10010533, Level = 33 },
            // Clear The Mercurial Gauntlet (Light): Lv. 34
            new ClearWallLevelMission() { MissionId = 10010534, Level = 34 },
            // Clear The Mercurial Gauntlet (Light): Lv. 35
            new ClearWallLevelMission() { MissionId = 10010535, Level = 35 },
            // Clear The Mercurial Gauntlet (Light): Lv. 36
            new ClearWallLevelMission() { MissionId = 10010536, Level = 36 },
            // Clear The Mercurial Gauntlet (Light): Lv. 37
            new ClearWallLevelMission() { MissionId = 10010537, Level = 37 },
            // Clear The Mercurial Gauntlet (Light): Lv. 38
            new ClearWallLevelMission() { MissionId = 10010538, Level = 38 },
            // Clear The Mercurial Gauntlet (Light): Lv. 39
            new ClearWallLevelMission() { MissionId = 10010539, Level = 39 },
            // Clear The Mercurial Gauntlet (Light): Lv. 40
            new ClearWallLevelMission() { MissionId = 10010540, Level = 40 },
            // Clear The Mercurial Gauntlet (Light): Lv. 41
            new ClearWallLevelMission() { MissionId = 10010541, Level = 41 },
            // Clear The Mercurial Gauntlet (Light): Lv. 42
            new ClearWallLevelMission() { MissionId = 10010542, Level = 42 },
            // Clear The Mercurial Gauntlet (Light): Lv. 43
            new ClearWallLevelMission() { MissionId = 10010543, Level = 43 },
            // Clear The Mercurial Gauntlet (Light): Lv. 44
            new ClearWallLevelMission() { MissionId = 10010544, Level = 44 },
            // Clear The Mercurial Gauntlet (Light): Lv. 45
            new ClearWallLevelMission() { MissionId = 10010545, Level = 45 },
            // Clear The Mercurial Gauntlet (Light): Lv. 46
            new ClearWallLevelMission() { MissionId = 10010546, Level = 46 },
            // Clear The Mercurial Gauntlet (Light): Lv. 47
            new ClearWallLevelMission() { MissionId = 10010547, Level = 47 },
            // Clear The Mercurial Gauntlet (Light): Lv. 48
            new ClearWallLevelMission() { MissionId = 10010548, Level = 48 },
            // Clear The Mercurial Gauntlet (Light): Lv. 49
            new ClearWallLevelMission() { MissionId = 10010549, Level = 49 },
            // Clear The Mercurial Gauntlet (Light): Lv. 50
            new ClearWallLevelMission() { MissionId = 10010550, Level = 50 },
            // Clear The Mercurial Gauntlet (Light): Lv. 51
            new ClearWallLevelMission() { MissionId = 10010551, Level = 51 },
            // Clear The Mercurial Gauntlet (Light): Lv. 52
            new ClearWallLevelMission() { MissionId = 10010552, Level = 52 },
            // Clear The Mercurial Gauntlet (Light): Lv. 53
            new ClearWallLevelMission() { MissionId = 10010553, Level = 53 },
            // Clear The Mercurial Gauntlet (Light): Lv. 54
            new ClearWallLevelMission() { MissionId = 10010554, Level = 54 },
            // Clear The Mercurial Gauntlet (Light): Lv. 55
            new ClearWallLevelMission() { MissionId = 10010555, Level = 55 },
            // Clear The Mercurial Gauntlet (Light): Lv. 56
            new ClearWallLevelMission() { MissionId = 10010556, Level = 56 },
            // Clear The Mercurial Gauntlet (Light): Lv. 57
            new ClearWallLevelMission() { MissionId = 10010557, Level = 57 },
            // Clear The Mercurial Gauntlet (Light): Lv. 58
            new ClearWallLevelMission() { MissionId = 10010558, Level = 58 },
            // Clear The Mercurial Gauntlet (Light): Lv. 59
            new ClearWallLevelMission() { MissionId = 10010559, Level = 59 },
            // Clear The Mercurial Gauntlet (Light): Lv. 60
            new ClearWallLevelMission() { MissionId = 10010560, Level = 60 },
            // Clear The Mercurial Gauntlet (Light): Lv. 61
            new ClearWallLevelMission() { MissionId = 10010561, Level = 61 },
            // Clear The Mercurial Gauntlet (Light): Lv. 62
            new ClearWallLevelMission() { MissionId = 10010562, Level = 62 },
            // Clear The Mercurial Gauntlet (Light): Lv. 63
            new ClearWallLevelMission() { MissionId = 10010563, Level = 63 },
            // Clear The Mercurial Gauntlet (Light): Lv. 64
            new ClearWallLevelMission() { MissionId = 10010564, Level = 64 },
            // Clear The Mercurial Gauntlet (Light): Lv. 65
            new ClearWallLevelMission() { MissionId = 10010565, Level = 65 },
            // Clear The Mercurial Gauntlet (Light): Lv. 66
            new ClearWallLevelMission() { MissionId = 10010566, Level = 66 },
            // Clear The Mercurial Gauntlet (Light): Lv. 67
            new ClearWallLevelMission() { MissionId = 10010567, Level = 67 },
            // Clear The Mercurial Gauntlet (Light): Lv. 68
            new ClearWallLevelMission() { MissionId = 10010568, Level = 68 },
            // Clear The Mercurial Gauntlet (Light): Lv. 69
            new ClearWallLevelMission() { MissionId = 10010569, Level = 69 },
            // Clear The Mercurial Gauntlet (Light): Lv. 70
            new ClearWallLevelMission() { MissionId = 10010570, Level = 70 },
            // Clear The Mercurial Gauntlet (Light): Lv. 71
            new ClearWallLevelMission() { MissionId = 10010571, Level = 71 },
            // Clear The Mercurial Gauntlet (Light): Lv. 72
            new ClearWallLevelMission() { MissionId = 10010572, Level = 72 },
            // Clear The Mercurial Gauntlet (Light): Lv. 73
            new ClearWallLevelMission() { MissionId = 10010573, Level = 73 },
            // Clear The Mercurial Gauntlet (Light): Lv. 74
            new ClearWallLevelMission() { MissionId = 10010574, Level = 74 },
            // Clear The Mercurial Gauntlet (Light): Lv. 75
            new ClearWallLevelMission() { MissionId = 10010575, Level = 75 },
            // Clear The Mercurial Gauntlet (Light): Lv. 76
            new ClearWallLevelMission() { MissionId = 10010576, Level = 76 },
            // Clear The Mercurial Gauntlet (Light): Lv. 77
            new ClearWallLevelMission() { MissionId = 10010577, Level = 77 },
            // Clear The Mercurial Gauntlet (Light): Lv. 78
            new ClearWallLevelMission() { MissionId = 10010578, Level = 78 },
            // Clear The Mercurial Gauntlet (Light): Lv. 79
            new ClearWallLevelMission() { MissionId = 10010579, Level = 79 },
            // Clear The Mercurial Gauntlet (Light): Lv. 80
            new ClearWallLevelMission() { MissionId = 10010580, Level = 80 },
        ];
}
