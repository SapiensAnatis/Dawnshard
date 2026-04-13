namespace DragaliaAPI.MissionDesigner.Models.RegularMission;

/// <summary>
/// Mission completed by unlocking mana nodes for a specific character.
/// Can optionally be scoped to a specific character via <see cref="CharaId"/> and/or element via
/// <see cref="Element"/>.
/// Example: "Unlock 10 of Sophie's Mana Nodes".
/// </summary>
public class CharacterManaNodeUnlockMission : Mission
{
    protected override MissionCompleteType CompleteType =>
        MissionCompleteType.CharacterManaNodeUnlock;

    public Charas? CharaId { get; init; }

    public UnitElement? Element { get; init; }

    protected override bool UseTotalValue => true;

    protected override int? Parameter => (int?)this.CharaId;

    protected override int? Parameter2 => (int?)this.Element;
}
