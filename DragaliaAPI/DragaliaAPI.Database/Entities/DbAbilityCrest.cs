using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Database.Entities.Abstract;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

/// <summary>
/// Wyrmprint database entity.
/// </summary>
[PrimaryKey(nameof(ViewerId), nameof(AbilityCrestId))]
public class DbAbilityCrest : DbPlayerData
{
    /// <summary>
    /// Gets or sets a value that dictates the wyrmprint's identity.
    /// </summary>
    public required AbilityCrests AbilityCrestId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating the wyrmprint's level.
    /// </summary>
    public int BuildupCount { get; set; } = 1;

    /// <summary>
    /// Gets or sets a value indicating the wyrmprint's unbind status.
    /// </summary>
    public int LimitBreakCount { get; set; }

    /// <summary>
    /// Gets or sets a value indicating how many copies of the wyrmprint are owned.
    /// </summary>
    public int EquipableCount { get; set; } = 1;

    /// <summary>
    /// Gets or sets the number of HP augments applied to the wyrmprint.
    /// </summary>
    public int HpPlusCount { get; set; }

    /// <summary>
    /// Gets or sets the number of strength augments applied to the wyrmprint.
    /// </summary>
    public int AttackPlusCount { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the wyrmprint is locked/favourited.
    /// </summary>
    [TypeConverter(typeof(BooleanConverter))]
    public bool IsFavorite { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the wyrmprint is new.
    /// </summary>
    [TypeConverter(typeof(BooleanConverter))]
    public bool IsNew { get; set; }

    /// <summary>
    /// Gets or sets the time at which the wyrmprint was received.
    /// </summary>
    [TypeConverter(typeof(DateTimeOffsetConverter))]
    public DateTimeOffset GetTime { get; set; } = DateTimeOffset.UtcNow;

    [NotMapped]
    public int AbilityLevel => (LimitBreakCount / 2) + 1;
}
