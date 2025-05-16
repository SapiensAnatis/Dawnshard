using DragaliaAPI.Models.Generated;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Features.Dungeon.Start;

[Mapper]
public static partial class DungeonStartMapper
{
    [MapperIgnoreTarget(nameof(CharaList.Exp))]
    [MapperIgnoreTarget(nameof(CharaList.IsNew))]
    [MapperIgnoreTarget(nameof(CharaList.BurstAttackLevel))]
    [MapperIgnoreTarget(nameof(CharaList.LimitBreakCount))]
    [MapperIgnoreTarget(nameof(CharaList.ComboBuildupCount))]
    [MapperIgnoreTarget(nameof(CharaList.GetTime))]
    [MapperIgnoreTarget(nameof(CharaList.ManaCirclePieceIdList))]
    [MapperIgnoreTarget(nameof(CharaList.IsTemporary))]
    [MapperIgnoreTarget(nameof(CharaList.ListViewFlag))]
    public static partial CharaList ToCharaList(this AtgenSupportChara supportChara);

    [MapperIgnoreTarget(nameof(DragonList.Exp))]
    [MapperIgnoreTarget(nameof(DragonList.IsLock))]
    [MapperIgnoreTarget(nameof(DragonList.IsNew))]
    [MapperIgnoreTarget(nameof(DragonList.GetTime))]
    [MapperIgnoreSource(nameof(AtgenSupportDragon.Hp))]
    [MapperIgnoreSource(nameof(AtgenSupportDragon.Attack))]
    public static partial DragonList ToDragonList(this AtgenSupportDragon supportDragon);

    [MapperIgnoreTarget(nameof(GameWeaponBody.SkillNo))]
    [MapperIgnoreTarget(nameof(GameWeaponBody.SkillLevel))]
    [MapperIgnoreTarget(nameof(GameWeaponBody.Ability1Level))]
    [MapperIgnoreTarget(nameof(GameWeaponBody.Ability2Level))]
    public static partial GameWeaponBody ToGameWeaponBody(
        this AtgenSupportWeaponBody supportWeaponBody
    );

    [MapperIgnoreTarget(nameof(GameAbilityCrest.Ability1Level))]
    [MapperIgnoreTarget(nameof(GameAbilityCrest.Ability2Level))]
    public static partial GameAbilityCrest ToGameAbilityCrest(
        this AtgenSupportCrestSlotType1List supportCrest
    );

    [MapperIgnoreTarget(nameof(TalismanList.IsNew))]
    [MapperIgnoreTarget(nameof(TalismanList.IsLock))]
    [MapperIgnoreTarget(nameof(TalismanList.GetTime))]
    public static partial TalismanList ToTalismanList(this AtgenSupportTalisman supportTalisman);
}
