using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.AutoMapper;

public class WeaponAbilityLevelResolver : IValueResolver<DbWeaponBody, GameWeaponBody, int>
{
    public int Resolve(
        DbWeaponBody source,
        GameWeaponBody destination,
        int destMember,
        ResolutionContext context
    )
    {
        return Math.Min(source.LimitOverCount + 1, 2);
    }
}

public class WeaponSkillLevelResolver : IValueResolver<DbWeaponBody, GameWeaponBody, int>
{
    public int Resolve(
        DbWeaponBody source,
        GameWeaponBody destination,
        int destMember,
        ResolutionContext context
    )
    {
        return (source.LimitBreakCount / 4) + 1;
    }
}
