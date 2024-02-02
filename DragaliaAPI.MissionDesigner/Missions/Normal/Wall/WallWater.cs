using DragaliaAPI.MissionDesigner.Models;
using DragaliaAPI.MissionDesigner.Models.Attributes;

namespace DragaliaAPI.MissionDesigner.Missions.Normal.Wall;

[ContainsMissionList]
public class WallWater
{
    [MissionType(MissionType.Normal)]
    [WallType(QuestWallTypes.Water)]
    public static List<Mission> Missions { get; } =
        [
            // Clear The Mercurial Gauntlet (Water): Lv. 1
            new ClearWallLevelMission() { MissionId = 10010301, Level = 1 },
            // Clear The Mercurial Gauntlet (Water): Lv. 2
            new ClearWallLevelMission() { MissionId = 10010302, Level = 2 },
            // Clear The Mercurial Gauntlet (Water): Lv. 3
            new ClearWallLevelMission() { MissionId = 10010303, Level = 3 },
            // Clear The Mercurial Gauntlet (Water): Lv. 4
            new ClearWallLevelMission() { MissionId = 10010304, Level = 4 },
            // Clear The Mercurial Gauntlet (Water): Lv. 5
            new ClearWallLevelMission() { MissionId = 10010305, Level = 5 },
            // Clear The Mercurial Gauntlet (Water): Lv. 6
            new ClearWallLevelMission() { MissionId = 10010306, Level = 6 },
            // Clear The Mercurial Gauntlet (Water): Lv. 7
            new ClearWallLevelMission() { MissionId = 10010307, Level = 7 },
            // Clear The Mercurial Gauntlet (Water): Lv. 8
            new ClearWallLevelMission() { MissionId = 10010308, Level = 8 },
            // Clear The Mercurial Gauntlet (Water): Lv. 9
            new ClearWallLevelMission() { MissionId = 10010309, Level = 9 },
            // Clear The Mercurial Gauntlet (Water): Lv. 10
            new ClearWallLevelMission() { MissionId = 10010310, Level = 10 },
            // Clear The Mercurial Gauntlet (Water): Lv. 11
            new ClearWallLevelMission() { MissionId = 10010311, Level = 11 },
            // Clear The Mercurial Gauntlet (Water): Lv. 12
            new ClearWallLevelMission() { MissionId = 10010312, Level = 12 },
            // Clear The Mercurial Gauntlet (Water): Lv. 13
            new ClearWallLevelMission() { MissionId = 10010313, Level = 13 },
            // Clear The Mercurial Gauntlet (Water): Lv. 14
            new ClearWallLevelMission() { MissionId = 10010314, Level = 14 },
            // Clear The Mercurial Gauntlet (Water): Lv. 15
            new ClearWallLevelMission() { MissionId = 10010315, Level = 15 },
            // Clear The Mercurial Gauntlet (Water): Lv. 16
            new ClearWallLevelMission() { MissionId = 10010316, Level = 16 },
            // Clear The Mercurial Gauntlet (Water): Lv. 17
            new ClearWallLevelMission() { MissionId = 10010317, Level = 17 },
            // Clear The Mercurial Gauntlet (Water): Lv. 18
            new ClearWallLevelMission() { MissionId = 10010318, Level = 18 },
            // Clear The Mercurial Gauntlet (Water): Lv. 19
            new ClearWallLevelMission() { MissionId = 10010319, Level = 19 },
            // Clear The Mercurial Gauntlet (Water): Lv. 20
            new ClearWallLevelMission() { MissionId = 10010320, Level = 20 },
            // Clear The Mercurial Gauntlet (Water): Lv. 21
            new ClearWallLevelMission() { MissionId = 10010321, Level = 21 },
            // Clear The Mercurial Gauntlet (Water): Lv. 22
            new ClearWallLevelMission() { MissionId = 10010322, Level = 22 },
            // Clear The Mercurial Gauntlet (Water): Lv. 23
            new ClearWallLevelMission() { MissionId = 10010323, Level = 23 },
            // Clear The Mercurial Gauntlet (Water): Lv. 24
            new ClearWallLevelMission() { MissionId = 10010324, Level = 24 },
            // Clear The Mercurial Gauntlet (Water): Lv. 25
            new ClearWallLevelMission() { MissionId = 10010325, Level = 25 },
            // Clear The Mercurial Gauntlet (Water): Lv. 26
            new ClearWallLevelMission() { MissionId = 10010326, Level = 26 },
            // Clear The Mercurial Gauntlet (Water): Lv. 27
            new ClearWallLevelMission() { MissionId = 10010327, Level = 27 },
            // Clear The Mercurial Gauntlet (Water): Lv. 28
            new ClearWallLevelMission() { MissionId = 10010328, Level = 28 },
            // Clear The Mercurial Gauntlet (Water): Lv. 29
            new ClearWallLevelMission() { MissionId = 10010329, Level = 29 },
            // Clear The Mercurial Gauntlet (Water): Lv. 30
            new ClearWallLevelMission() { MissionId = 10010330, Level = 30 },
            // Clear The Mercurial Gauntlet (Water): Lv. 31
            new ClearWallLevelMission() { MissionId = 10010331, Level = 31 },
            // Clear The Mercurial Gauntlet (Water): Lv. 32
            new ClearWallLevelMission() { MissionId = 10010332, Level = 32 },
            // Clear The Mercurial Gauntlet (Water): Lv. 33
            new ClearWallLevelMission() { MissionId = 10010333, Level = 33 },
            // Clear The Mercurial Gauntlet (Water): Lv. 34
            new ClearWallLevelMission() { MissionId = 10010334, Level = 34 },
            // Clear The Mercurial Gauntlet (Water): Lv. 35
            new ClearWallLevelMission() { MissionId = 10010335, Level = 35 },
            // Clear The Mercurial Gauntlet (Water): Lv. 36
            new ClearWallLevelMission() { MissionId = 10010336, Level = 36 },
            // Clear The Mercurial Gauntlet (Water): Lv. 37
            new ClearWallLevelMission() { MissionId = 10010337, Level = 37 },
            // Clear The Mercurial Gauntlet (Water): Lv. 38
            new ClearWallLevelMission() { MissionId = 10010338, Level = 38 },
            // Clear The Mercurial Gauntlet (Water): Lv. 39
            new ClearWallLevelMission() { MissionId = 10010339, Level = 39 },
            // Clear The Mercurial Gauntlet (Water): Lv. 40
            new ClearWallLevelMission() { MissionId = 10010340, Level = 40 },
            // Clear The Mercurial Gauntlet (Water): Lv. 41
            new ClearWallLevelMission() { MissionId = 10010341, Level = 41 },
            // Clear The Mercurial Gauntlet (Water): Lv. 42
            new ClearWallLevelMission() { MissionId = 10010342, Level = 42 },
            // Clear The Mercurial Gauntlet (Water): Lv. 43
            new ClearWallLevelMission() { MissionId = 10010343, Level = 43 },
            // Clear The Mercurial Gauntlet (Water): Lv. 44
            new ClearWallLevelMission() { MissionId = 10010344, Level = 44 },
            // Clear The Mercurial Gauntlet (Water): Lv. 45
            new ClearWallLevelMission() { MissionId = 10010345, Level = 45 },
            // Clear The Mercurial Gauntlet (Water): Lv. 46
            new ClearWallLevelMission() { MissionId = 10010346, Level = 46 },
            // Clear The Mercurial Gauntlet (Water): Lv. 47
            new ClearWallLevelMission() { MissionId = 10010347, Level = 47 },
            // Clear The Mercurial Gauntlet (Water): Lv. 48
            new ClearWallLevelMission() { MissionId = 10010348, Level = 48 },
            // Clear The Mercurial Gauntlet (Water): Lv. 49
            new ClearWallLevelMission() { MissionId = 10010349, Level = 49 },
            // Clear The Mercurial Gauntlet (Water): Lv. 50
            new ClearWallLevelMission() { MissionId = 10010350, Level = 50 },
            // Clear The Mercurial Gauntlet (Water): Lv. 51
            new ClearWallLevelMission() { MissionId = 10010351, Level = 51 },
            // Clear The Mercurial Gauntlet (Water): Lv. 52
            new ClearWallLevelMission() { MissionId = 10010352, Level = 52 },
            // Clear The Mercurial Gauntlet (Water): Lv. 53
            new ClearWallLevelMission() { MissionId = 10010353, Level = 53 },
            // Clear The Mercurial Gauntlet (Water): Lv. 54
            new ClearWallLevelMission() { MissionId = 10010354, Level = 54 },
            // Clear The Mercurial Gauntlet (Water): Lv. 55
            new ClearWallLevelMission() { MissionId = 10010355, Level = 55 },
            // Clear The Mercurial Gauntlet (Water): Lv. 56
            new ClearWallLevelMission() { MissionId = 10010356, Level = 56 },
            // Clear The Mercurial Gauntlet (Water): Lv. 57
            new ClearWallLevelMission() { MissionId = 10010357, Level = 57 },
            // Clear The Mercurial Gauntlet (Water): Lv. 58
            new ClearWallLevelMission() { MissionId = 10010358, Level = 58 },
            // Clear The Mercurial Gauntlet (Water): Lv. 59
            new ClearWallLevelMission() { MissionId = 10010359, Level = 59 },
            // Clear The Mercurial Gauntlet (Water): Lv. 60
            new ClearWallLevelMission() { MissionId = 10010360, Level = 60 },
            // Clear The Mercurial Gauntlet (Water): Lv. 61
            new ClearWallLevelMission() { MissionId = 10010361, Level = 61 },
            // Clear The Mercurial Gauntlet (Water): Lv. 62
            new ClearWallLevelMission() { MissionId = 10010362, Level = 62 },
            // Clear The Mercurial Gauntlet (Water): Lv. 63
            new ClearWallLevelMission() { MissionId = 10010363, Level = 63 },
            // Clear The Mercurial Gauntlet (Water): Lv. 64
            new ClearWallLevelMission() { MissionId = 10010364, Level = 64 },
            // Clear The Mercurial Gauntlet (Water): Lv. 65
            new ClearWallLevelMission() { MissionId = 10010365, Level = 65 },
            // Clear The Mercurial Gauntlet (Water): Lv. 66
            new ClearWallLevelMission() { MissionId = 10010366, Level = 66 },
            // Clear The Mercurial Gauntlet (Water): Lv. 67
            new ClearWallLevelMission() { MissionId = 10010367, Level = 67 },
            // Clear The Mercurial Gauntlet (Water): Lv. 68
            new ClearWallLevelMission() { MissionId = 10010368, Level = 68 },
            // Clear The Mercurial Gauntlet (Water): Lv. 69
            new ClearWallLevelMission() { MissionId = 10010369, Level = 69 },
            // Clear The Mercurial Gauntlet (Water): Lv. 70
            new ClearWallLevelMission() { MissionId = 10010370, Level = 70 },
            // Clear The Mercurial Gauntlet (Water): Lv. 71
            new ClearWallLevelMission() { MissionId = 10010371, Level = 71 },
            // Clear The Mercurial Gauntlet (Water): Lv. 72
            new ClearWallLevelMission() { MissionId = 10010372, Level = 72 },
            // Clear The Mercurial Gauntlet (Water): Lv. 73
            new ClearWallLevelMission() { MissionId = 10010373, Level = 73 },
            // Clear The Mercurial Gauntlet (Water): Lv. 74
            new ClearWallLevelMission() { MissionId = 10010374, Level = 74 },
            // Clear The Mercurial Gauntlet (Water): Lv. 75
            new ClearWallLevelMission() { MissionId = 10010375, Level = 75 },
            // Clear The Mercurial Gauntlet (Water): Lv. 76
            new ClearWallLevelMission() { MissionId = 10010376, Level = 76 },
            // Clear The Mercurial Gauntlet (Water): Lv. 77
            new ClearWallLevelMission() { MissionId = 10010377, Level = 77 },
            // Clear The Mercurial Gauntlet (Water): Lv. 78
            new ClearWallLevelMission() { MissionId = 10010378, Level = 78 },
            // Clear The Mercurial Gauntlet (Water): Lv. 79
            new ClearWallLevelMission() { MissionId = 10010379, Level = 79 },
            // Clear The Mercurial Gauntlet (Water): Lv. 80
            new ClearWallLevelMission() { MissionId = 10010380, Level = 80 },
        ];
}
