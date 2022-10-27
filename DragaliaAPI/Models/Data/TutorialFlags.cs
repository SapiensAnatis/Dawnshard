using System.Diagnostics;
using System.Linq;

namespace DragaliaAPI.Models.Data;

[Flags]
public enum TutorialFlags
{
    Clear = 0,
    GrowthDragon = 1001,
    ReleaseShop = 1002,
    ReleaseTruthDragonBattle = 1003,
    PartyAttributeTutorial = 1004,
    CastleStoryTutorial = 1005,
    ServerTruthDragonBattle = 1006,
    VoidBattleTutorial = 1007,
    AmuletDoubleTutorial = 1008,
    QuestWallTutorial = 1009,
    ServerClearNinthChapter = 1010,
    AstralRaidTutorial = 1011,
    PlusCountTutorial = 1012,
    GuildTutorial = 1013,
    ServerClearTenthChapter = 1014,
    QuestVeryHardTutorial = 1015,
    ServerReleaseAgito = 1016,
    AgitoTutorial = 1017,
    QuestCarryTutorial = 1018,
    EditSkillTutorial = 1019,
    MissionDrillTutorial = 1020,
    DragonSphereTutorial = 1021,
    SummonPointTutorial = 1022,
    AbilityCrestGrowTutorial = 1023,
    WeaponBodyGrowTutorial = 1024,
    DiabolosTutorial = 1025,
    SubdueTutorial = 1026,
    AlbumTutorial = 1027,
    ServerMostDefinitelyDiabolos = 1028,
    SagaTutorial = 1029,
    ServerClearSixteenthChapter = 1030
}

public static class TutorialFlagUtil
{
    public static ISet<TutorialFlags> ConvertIntToFlagList(int flags)
    {
        return ConvertIntToFlagIntList(flags).Select(v => (TutorialFlags)v).ToHashSet();
    }

    public static ISet<int> ConvertIntToFlagIntList(int flags)
    {
        ISet<int> setFlags = new SortedSet<int>();
        if (flags != (int)TutorialFlags.Clear)
        {
            for (int i = 0; i < 30; i++)
            {
                int _flagNr = 1001 + i;
                int _flag = 1 << i;
                if ((flags & _flag) == _flag)
                    setFlags.Add(_flagNr);
            }
        }
        return setFlags;
    }

    public static int ConvertFlagListToInt(ISet<TutorialFlags> flagList)
    {
        return ConvertFlagListToInt(flagList, 0);
    }

    public static int ConvertFlagListToInt(ISet<TutorialFlags> flagList, int flags)
    {
        return ConvertFlagIntListToInt(flagList.Select(v => (int)v).ToHashSet(), flags);
    }

    public static int ConvertFlagIntListToInt(ISet<int> flagList)
    {
        return ConvertFlagIntListToInt(flagList, 0);
    }

    public static int ConvertFlagIntListToInt(IEnumerable<int> flagList, int flags)
    {
        if (!flagList.Any() || flagList.Contains((int)TutorialFlags.Clear))
            return (int)TutorialFlags.Clear;
        foreach (int _flagNr in flagList)
        {
            if (
                !(
                    _flagNr < (int)TutorialFlags.GrowthDragon
                    || _flagNr > (int)TutorialFlags.ServerClearSixteenthChapter
                )
            )
            {
                int _flag = _flagNr - 1001;
                flags |= 1 << _flag;
            }
        }
        return flags;
    }
}
