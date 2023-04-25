using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace DragaliaAPI.Database.Entities;

/// <summary>
/// Wyrmprint database entity.
/// </summary>
[Index(nameof(DeviceAccountId))]
public class DbAbilityCrest : IDbHasAccountId
{
    /// <inheritdoc />
    public virtual DbPlayer? Owner { get; set; }

    /// <inheritdoc />
    [ForeignKey(nameof(Owner))]
    public required string DeviceAccountId { get; set; }

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
    public int LimitBreakCount { get; set; } = 0;

    /// <summary>
    /// Gets or sets a value indicating how many copies of the wyrmprint are owned.
    /// </summary>
    public int EquipableCount { get; set; } = 1;

    /// <summary>
    /// Gets or sets the number of HP augments applied to the wyrmprint.
    /// </summary>
    public int HpPlusCount { get; set; } = 0;

    /// <summary>
    /// Gets or sets the number of strength augments applied to the wyrmprint.
    /// </summary>
    public int AttackPlusCount { get; set; } = 0;

    /// <summary>
    /// Gets or sets a value indicating whether the wyrmprint is locked/favourited.
    /// </summary>
    [TypeConverter(typeof(BooleanConverter))]
    public bool IsFavorite { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether the wyrmprint is new.
    /// </summary>
    [TypeConverter(typeof(BooleanConverter))]
    public bool IsNew { get; set; } = false;

    /// <summary>
    /// Gets or sets the time at which the wyrmprint was received.
    /// </summary>
    [TypeConverter(typeof(DateTimeOffsetConverter))]
    public DateTimeOffset GetTime { get; set; } = DateTimeOffset.UtcNow;

    [NotMapped]
    public int AbilityLevel => (LimitBreakCount / 2) + 1;

    /// <summary>
    /// EF Core / test constructor.
    /// </summary>
    public DbAbilityCrest() { }

    /// <summary>
    /// User-facing constructor.
    /// </summary>
    /// <param name="deviceAccountId">Primary key.</param>
    [SetsRequiredMembers]
    public DbAbilityCrest(string deviceAccountId, AbilityCrests id)
    {
        this.DeviceAccountId = deviceAccountId;
        this.AbilityCrestId = id;
    }
}

internal class DbAbilityCrestConfiguration : IEntityTypeConfiguration<DbAbilityCrest>
{
    public void Configure(EntityTypeBuilder<DbAbilityCrest> builder)
    {
        builder.HasKey(e => new { e.DeviceAccountId, e.AbilityCrestId });
    }
}
