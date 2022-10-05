using System.Diagnostics;
using System.Linq;

namespace DragaliaAPI.Models;

public static class TutorialFlagUtil
{
    [Flags]
    public enum TutorialFlags
    {
        Clear = 0,
        Flag1001 = 1001,
        Flag1002 = 1002,
        Flag1003 = 1003,
        Flag1004 = 1004,
        Flag1005 = 1005,
        Flag1006 = 1006,
        Flag1007 = 1007,
        Flag1008 = 1008,
        Flag1009 = 1009,
        Flag1010 = 1010,
        Flag1011 = 1011,
        Flag1012 = 1012,
        Flag1013 = 1013,
        Flag1014 = 1014,
        Flag1015 = 1015,
        Flag1016 = 1016,
        Flag1017 = 1017,
        Flag1018 = 1018,
        Flag1019 = 1019,
        Flag1020 = 1020,
        Flag1021 = 1021,
        Flag1022 = 1022,
        Flag1023 = 1023,
        Flag1024 = 1024,
        Flag1025 = 1025,
        Flag1026 = 1026,
        Flag1027 = 1027,
        Flag1028 = 1028,
        Flag1029 = 1029,
        Flag1030 = 1030
    }

    public static ISet<TutorialFlags> ConvertIntToFlagList(int flags)
    {
        return ConvertIntToFlagIntList(flags).Select(v => (TutorialFlags)v).ToHashSet();
    }

    public static ISet<int> ConvertIntToFlagIntList(int flags)
    {
        ISet<int> setFlags = new HashSet<int>();
        if (flags != (int)TutorialFlags.Clear)
        {
            for (int i = 0; i < 30; i++)
            {
                int _flagNr = 1001 + i;
                int _flag = 1 << i;
                if ((flags & _flag) == _flag)
                {
                    setFlags.Add(_flagNr);
                }
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

    public static int ConvertFlagIntListToInt(ISet<int> flagList, int flags)
    {
        if (flagList.Count == 0 || flagList.Contains((int)TutorialFlags.Clear))
        {
            return (int)TutorialFlags.Clear;
        }
        foreach (int _flagNr in flagList)
        {
            if (!(_flagNr < (int)TutorialFlags.Flag1001 || _flagNr > (int)TutorialFlags.Flag1030))
            {
                int _flag = _flagNr - 1001;
                flags |= 1 << _flag;
            }
        }
        return flags;
    }
}
