using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Database.Factories;

public static class DbPlayerDragonDataFactory
{
    public static DbPlayerDragonData Create(string deviceAccountId, Dragons id)
    {
        return new()
        {
            DeviceAccountId = deviceAccountId,
            DragonId = id,
            Exp = 0,
            Level = 1,
            HpPlusCount = 0,
            AttackPlusCount = 0,
            LimitBreakCount = 0,
            IsLock = false,
            IsNew = true,
            Skill1Level = 1,
            Ability1Level = 1,
            Ability2Level = 1,
            GetTime = DateTimeOffset.UtcNow
        };
    }
}
