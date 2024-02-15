using DragaliaAPI.MissionDesigner.Models;
using DragaliaAPI.MissionDesigner.Models.Attributes;

namespace DragaliaAPI.MissionDesigner.Missions.Normal.Wall;

[ContainsMissionList]
public class WallWind
{
    [MissionType(MissionType.Normal)]
    [WallType(QuestWallTypes.Wind)]
    public static List<Mission> Missions { get; } =
        [
            // Clear The Mercurial Gauntlet (Wind): Lv. 1
            new ClearWallLevelMission() { MissionId = 10010401, Level = 1 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 2
            new ClearWallLevelMission() { MissionId = 10010402, Level = 2 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 3
            new ClearWallLevelMission() { MissionId = 10010403, Level = 3 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 4
            new ClearWallLevelMission() { MissionId = 10010404, Level = 4 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 5
            new ClearWallLevelMission() { MissionId = 10010405, Level = 5 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 6
            new ClearWallLevelMission() { MissionId = 10010406, Level = 6 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 7
            new ClearWallLevelMission() { MissionId = 10010407, Level = 7 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 8
            new ClearWallLevelMission() { MissionId = 10010408, Level = 8 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 9
            new ClearWallLevelMission() { MissionId = 10010409, Level = 9 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 10
            new ClearWallLevelMission() { MissionId = 10010410, Level = 10 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 11
            new ClearWallLevelMission() { MissionId = 10010411, Level = 11 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 12
            new ClearWallLevelMission() { MissionId = 10010412, Level = 12 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 13
            new ClearWallLevelMission() { MissionId = 10010413, Level = 13 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 14
            new ClearWallLevelMission() { MissionId = 10010414, Level = 14 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 15
            new ClearWallLevelMission() { MissionId = 10010415, Level = 15 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 16
            new ClearWallLevelMission() { MissionId = 10010416, Level = 16 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 17
            new ClearWallLevelMission() { MissionId = 10010417, Level = 17 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 18
            new ClearWallLevelMission() { MissionId = 10010418, Level = 18 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 19
            new ClearWallLevelMission() { MissionId = 10010419, Level = 19 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 20
            new ClearWallLevelMission() { MissionId = 10010420, Level = 20 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 21
            new ClearWallLevelMission() { MissionId = 10010421, Level = 21 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 22
            new ClearWallLevelMission() { MissionId = 10010422, Level = 22 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 23
            new ClearWallLevelMission() { MissionId = 10010423, Level = 23 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 24
            new ClearWallLevelMission() { MissionId = 10010424, Level = 24 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 25
            new ClearWallLevelMission() { MissionId = 10010425, Level = 25 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 26
            new ClearWallLevelMission() { MissionId = 10010426, Level = 26 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 27
            new ClearWallLevelMission() { MissionId = 10010427, Level = 27 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 28
            new ClearWallLevelMission() { MissionId = 10010428, Level = 28 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 29
            new ClearWallLevelMission() { MissionId = 10010429, Level = 29 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 30
            new ClearWallLevelMission() { MissionId = 10010430, Level = 30 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 31
            new ClearWallLevelMission() { MissionId = 10010431, Level = 31 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 32
            new ClearWallLevelMission() { MissionId = 10010432, Level = 32 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 33
            new ClearWallLevelMission() { MissionId = 10010433, Level = 33 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 34
            new ClearWallLevelMission() { MissionId = 10010434, Level = 34 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 35
            new ClearWallLevelMission() { MissionId = 10010435, Level = 35 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 36
            new ClearWallLevelMission() { MissionId = 10010436, Level = 36 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 37
            new ClearWallLevelMission() { MissionId = 10010437, Level = 37 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 38
            new ClearWallLevelMission() { MissionId = 10010438, Level = 38 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 39
            new ClearWallLevelMission() { MissionId = 10010439, Level = 39 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 40
            new ClearWallLevelMission() { MissionId = 10010440, Level = 40 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 41
            new ClearWallLevelMission() { MissionId = 10010441, Level = 41 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 42
            new ClearWallLevelMission() { MissionId = 10010442, Level = 42 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 43
            new ClearWallLevelMission() { MissionId = 10010443, Level = 43 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 44
            new ClearWallLevelMission() { MissionId = 10010444, Level = 44 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 45
            new ClearWallLevelMission() { MissionId = 10010445, Level = 45 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 46
            new ClearWallLevelMission() { MissionId = 10010446, Level = 46 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 47
            new ClearWallLevelMission() { MissionId = 10010447, Level = 47 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 48
            new ClearWallLevelMission() { MissionId = 10010448, Level = 48 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 49
            new ClearWallLevelMission() { MissionId = 10010449, Level = 49 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 50
            new ClearWallLevelMission() { MissionId = 10010450, Level = 50 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 51
            new ClearWallLevelMission() { MissionId = 10010451, Level = 51 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 52
            new ClearWallLevelMission() { MissionId = 10010452, Level = 52 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 53
            new ClearWallLevelMission() { MissionId = 10010453, Level = 53 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 54
            new ClearWallLevelMission() { MissionId = 10010454, Level = 54 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 55
            new ClearWallLevelMission() { MissionId = 10010455, Level = 55 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 56
            new ClearWallLevelMission() { MissionId = 10010456, Level = 56 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 57
            new ClearWallLevelMission() { MissionId = 10010457, Level = 57 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 58
            new ClearWallLevelMission() { MissionId = 10010458, Level = 58 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 59
            new ClearWallLevelMission() { MissionId = 10010459, Level = 59 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 60
            new ClearWallLevelMission() { MissionId = 10010460, Level = 60 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 61
            new ClearWallLevelMission() { MissionId = 10010461, Level = 61 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 62
            new ClearWallLevelMission() { MissionId = 10010462, Level = 62 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 63
            new ClearWallLevelMission() { MissionId = 10010463, Level = 63 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 64
            new ClearWallLevelMission() { MissionId = 10010464, Level = 64 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 65
            new ClearWallLevelMission() { MissionId = 10010465, Level = 65 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 66
            new ClearWallLevelMission() { MissionId = 10010466, Level = 66 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 67
            new ClearWallLevelMission() { MissionId = 10010467, Level = 67 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 68
            new ClearWallLevelMission() { MissionId = 10010468, Level = 68 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 69
            new ClearWallLevelMission() { MissionId = 10010469, Level = 69 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 70
            new ClearWallLevelMission() { MissionId = 10010470, Level = 70 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 71
            new ClearWallLevelMission() { MissionId = 10010471, Level = 71 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 72
            new ClearWallLevelMission() { MissionId = 10010472, Level = 72 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 73
            new ClearWallLevelMission() { MissionId = 10010473, Level = 73 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 74
            new ClearWallLevelMission() { MissionId = 10010474, Level = 74 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 75
            new ClearWallLevelMission() { MissionId = 10010475, Level = 75 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 76
            new ClearWallLevelMission() { MissionId = 10010476, Level = 76 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 77
            new ClearWallLevelMission() { MissionId = 10010477, Level = 77 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 78
            new ClearWallLevelMission() { MissionId = 10010478, Level = 78 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 79
            new ClearWallLevelMission() { MissionId = 10010479, Level = 79 },
            // Clear The Mercurial Gauntlet (Wind): Lv. 80
            new ClearWallLevelMission() { MissionId = 10010480, Level = 80 },
        ];
}
