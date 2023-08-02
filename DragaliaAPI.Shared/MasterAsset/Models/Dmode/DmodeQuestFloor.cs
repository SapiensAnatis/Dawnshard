﻿using System.Text.Json.Serialization;
using DragaliaAPI.Photon.Shared.Enums;
using DragaliaAPI.Shared.Json;

namespace DragaliaAPI.Shared.MasterAsset.Models.Dmode;

public record DmodeQuestFloor(
    int Id,
    int FloorNum,
    DungeonTypes DungeonType,
    [property: JsonConverter(typeof(MasterAssetDateTimeOffsetConverter))]
        DateTimeOffset ViewStartDate,
    [property: JsonConverter(typeof(MasterAssetDateTimeOffsetConverter))]
        DateTimeOffset ViewEndDate,
    int BaseEnemyLevel,
    int BaseBossEnemyLevel,
    int DrawDungeonThemeId1,
    int DrawDungeonThemeId2,
    int DrawDungeonThemeId3,
    int DrawDungeonThemeId4,
    int DrawDungeonThemeId5,
    int SkipClearFloorNum
)
{
    public int[] AvailableThemes =
    {
        DrawDungeonThemeId1,
        DrawDungeonThemeId2,
        DrawDungeonThemeId3,
        DrawDungeonThemeId4,
        DrawDungeonThemeId5
    };
};
