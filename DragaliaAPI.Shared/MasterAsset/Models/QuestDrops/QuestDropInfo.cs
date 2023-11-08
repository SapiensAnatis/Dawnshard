﻿using System.Text.Json.Serialization;
using DragaliaAPI.Shared.Definitions.Enums;
using JetBrains.Annotations;

namespace DragaliaAPI.Shared.MasterAsset.Models.QuestDrops;

public record QuestDropInfo(int QuestId, DropEntity[] Drops);

public record DropEntity(
    int Id,
    [property: JsonConverter(typeof(JsonStringEnumConverter))] EntityTypes EntityType,
    double Quantity
)
{
    /// <summary>
    /// Gets a weight for FluentRandomPicker.
    /// </summary>
    /// <remarks>
    /// Loses any decimal points after 10e-4.
    /// </remarks>
    public int Weight => (int)Math.Round(Quantity * 100);
}
