using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Database.Entities.Abstract;

/// <summary>
/// Base class for party units with edit skills / weapon skins equipped.
/// </summary>
public abstract class DbPartyUnitBase : DbUnitBase
{
    public int EquipWeaponSkinId { get; set; }

    public Charas EditSkill1CharaId { get; set; }

    public Charas EditSkill2CharaId { get; set; }

    public required int UnitNo { get; set; }
}
