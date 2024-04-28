using DragaliaAPI.MissionDesigner.Models.RegularMission;
using DragaliaAPI.Shared.Enums;

namespace DragaliaAPI.MissionDesigner.Models.Attributes;

public sealed class WallTypeAttribute(QuestWallTypes type) : ImplicitPropertyAttribute
{
    public override string Property => nameof(ClearWallLevelMission.WallType);

    public override object Value => type;
}
