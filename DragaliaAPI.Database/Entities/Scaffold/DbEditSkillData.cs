using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Database.Entities.Scaffold;

/// <summary>
/// Mapping template class for shared skill data. Not tracked in DB.
/// <remarks>Used in <see cref="DbDetailedPartyUnit"/>.</remake>
/// </summary>
public class DbEditSkillData
{
    public Charas CharaId { get; set; }

    public int EditSkillLevel { get; set; }
}
