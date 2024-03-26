using MessagePack;

namespace DragaliaAPI.Models.Generated;

public partial class WeaponBodyList
{
    /* These properties are not sent by the server, judging by the endgame_savefile.json in the API docs. */

    [Obsolete]
    [Key("skill_no")]
    public int SkillNo { get; set; }

    [Obsolete]
    [Key("skill_level")]
    public int SkillLevel { get; set; }

    [Obsolete]
    [Key("ability_1_level")]
    public int Ability1Level { get; set; }

    [Obsolete]
    [Key("ability_2_levell")]
    public int Ability2Levell { get; set; }
}
