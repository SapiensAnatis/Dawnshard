﻿using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models;

public record QuestData(
    int Id,
    QuestPlayModeTypes QuestPlayModeType,
    UnitElement LimitedElementalType,
    UnitElement LimitedElementalType2,
    int LimitedWeaponTypePatternId,
    int PayStaminaSingle,
    int PayStaminaMulti,
    DungeonTypes DungeonType,
    string Scene01,
    string AreaName01,
    string Scene02,
    string AreaName02,
    string Scene03,
    string AreaName03,
    string Scene04,
    string AreaName04,
    string Scene05,
    string AreaName05,
    string Scene06,
    string AreaName06
)
{
    public IEnumerable<AreaInfo> AreaInfo =>
        new List<AreaInfo>()
        {
            new(this.Scene01, this.AreaName01),
            new(this.Scene02, this.AreaName02),
            new(this.Scene03, this.AreaName03),
            new(this.Scene04, this.AreaName04),
            new(this.Scene05, this.AreaName05),
            new(this.Scene06, this.AreaName06),
        }.Where(x => !string.IsNullOrEmpty(x.ScenePath) && !string.IsNullOrEmpty(x.AreaName));
}
