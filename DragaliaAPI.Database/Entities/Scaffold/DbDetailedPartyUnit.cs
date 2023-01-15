﻿namespace DragaliaAPI.Database.Entities.Scaffold;

/// <summary>
/// Similar to DbPartyUnit, but contains actual chara/dragon objects instead of just IDs.
/// Not tracked in the database, assembled via repository methods. Used in dungeon_start
/// </summary>
public class DbDetailedPartyUnit
{
    public required string DeviceAccountId { get; set; }

    public int Position { get; set; }

    public required DbPlayerCharaData CharaData { get; set; }

    public DbPlayerDragonData? DragonData { get; set; }

    public int DragonReliabilityLevel { get; set; }

    public DbWeaponBody? WeaponBodyData { get; set; }

    public IEnumerable<DbAbilityCrest> CrestSlotType1CrestList { get; set; } =
        Enumerable.Empty<DbAbilityCrest>();

    public IEnumerable<DbAbilityCrest> CrestSlotType2CrestList { get; set; } =
        Enumerable.Empty<DbAbilityCrest>();

    public IEnumerable<DbAbilityCrest> CrestSlotType3CrestList { get; set; } =
        Enumerable.Empty<DbAbilityCrest>();

    public object? WeaponSkinData { get; set; } = null;

    public DbTalisman? TalismanData { get; set; } = null;

    public DbEditSkillData? EditSkill1CharaData { get; set; }

    public DbEditSkillData? EditSkill2CharaData { get; set; }

    public IEnumerable<object> GameWeaponPassiveAbilityList { get; set; } = new List<object> { };
}
