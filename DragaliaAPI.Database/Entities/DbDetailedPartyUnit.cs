namespace DragaliaAPI.Database.Entities;

/// <summary>
/// Similar to DbPartyUnit, but contains actual chara/dragon objects instead of just IDs.
/// Not tracked in the database, assembled via repository methods. Used in dungeon_start
/// </summary>
public class DbDetailedPartyUnit
{
    public required string DeviceAccountId { get; set; }

    public required int Position { get; set; }

    public required DbPlayerCharaData CharaData { get; set; }

    public required DbPlayerDragonData? DragonData { get; set; }

    public required int DragonReliabilityLevel { get; set; }

    public required DbWeaponBody? WeaponBodyData { get; set; }

    public required IEnumerable<DbAbilityCrest> CrestSlotType1CrestList { get; set; }

    public required IEnumerable<DbAbilityCrest> CrestSlotType2CrestList { get; set; }

    public required IEnumerable<DbAbilityCrest> CrestSlotType3CrestList { get; set; }

    public object? WeaponSkinData { get; set; } = null;

    public object? TalismanData { get; set; } = null;

    public required DbEditSkillData? EditSkill1CharaData { get; set; }

    public required DbEditSkillData? EditSkill2CharaData { get; set; }

    public IEnumerable<object> GameWeaponPassiveAbilityList { get; set; } = new List<object> { };
}
