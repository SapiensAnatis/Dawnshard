using MessagePack;

namespace DragaliaAPI.Shared.MasterAsset.Models.Dmode;

public record DmodeEnemyTheme(
    int Id,
    // :pensive:
    int Param1,
    int Param2,
    int Param3,
    int Param4,
    int Param5,
    int Param6,
    int Param7,
    int Param8,
    int Param9,
    int Param10,
    int Param11,
    int Param12,
    int Param13,
    int Param14,
    int Param15,
    int Param16,
    int Param17,
    int Param18,
    int Param19,
    int Param20,
    int Param21,
    int Param22,
    int Param23,
    int Param24,
    int Param25,
    int Param26,
    int Param27,
    int Param28,
    int Param29,
    int Param30
)
{
    [IgnoreMember]
    public int[] AvailableParams =
    {
        Param1,
        Param2,
        Param3,
        Param4,
        Param5,
        Param6,
        Param7,
        Param8,
        Param9,
        Param10,
        Param11,
        Param12,
        Param13,
        Param14,
        Param15,
        Param16,
        Param17,
        Param18,
        Param19,
        Param20,
        Param21,
        Param22,
        Param23,
        Param24,
        Param25,
        Param26,
        Param27,
        Param28,
        Param29,
        Param30
    };
};
