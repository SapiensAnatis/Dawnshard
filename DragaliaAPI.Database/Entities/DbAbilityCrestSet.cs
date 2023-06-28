using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace DragaliaAPI.Database.Entities;

/// <summary>
/// Wyrmprint set database entity.
/// </summary>
[Index(nameof(DeviceAccountId))]
[PrimaryKey(nameof(DeviceAccountId), nameof(AbilityCrestSetNo))]
public class DbAbilityCrestSet : IDbHasAccountId
{
    /// <inheritdoc />
    public virtual DbPlayer? Owner { get; set; }

    /// <inheritdoc />
    [ForeignKey(nameof(Owner))]
    public required string DeviceAccountId { get; set; }

    /// <summary>
    /// Gets or sets a value that dictates the wyrmprint set number.
    /// </summary>
    public required int AbilityCrestSetNo { get; set; }

    /// <summary>
    /// Gets or sets a string indicating the wyrmprint set's name.
    /// </summary>
    public string AbilityCrestSetName { get; set; } = "";

    /// <summary>
    /// Gets or sets a value indicating the wyrmprint set's first 5* wyrmprint slot.
    /// </summary>
    public AbilityCrests CrestSlotType1CrestId1 { get; set; } = 0;

    /// <summary>
    /// Gets or sets a value indicating the wyrmprint set's second 5* wyrmprint slot.
    /// </summary>
    public AbilityCrests CrestSlotType1CrestId2 { get; set; } = 0;

    /// <summary>
    /// Gets or sets a value indicating the wyrmprint set's third 5* wyrmprint slot.
    /// </summary>
    public AbilityCrests CrestSlotType1CrestId3 { get; set; } = 0;

    /// <summary>
    /// Gets or sets a value indicating the wyrmprint set's first 3*/4* wyrmprint slot.
    /// </summary>
    public AbilityCrests CrestSlotType2CrestId1 { get; set; } = 0;

    /// <summary>
    /// Gets or sets a value indicating the wyrmprint set's second 3*/4* wyrmprint slot.
    /// </summary>
    public AbilityCrests CrestSlotType2CrestId2 { get; set; } = 0;

    /// <summary>
    /// Gets or sets a value indicating the wyrmprint set's first sindom wyrmprint slot.
    /// </summary>
    public AbilityCrests CrestSlotType3CrestId1 { get; set; } = 0;

    /// <summary>
    /// Gets or sets a value indicating the wyrmprint set's second sindom wyrmprint slot.
    /// </summary>
    public AbilityCrests CrestSlotType3CrestId2 { get; set; } = 0;

    /// <summary>
    /// Gets or sets a value indicating the wyrmprint set's kaleidoscape print slot.
    /// </summary>
    public ulong TalismanKeyId { get; set; } = 0;

    /// <summary>
    /// EF Core / test constructor.
    /// </summary>
    public DbAbilityCrestSet() { }

    /// <summary>
    /// User-facing constructor.
    /// </summary>
    /// <param name="deviceAccountId">Primary key.</param>
    [SetsRequiredMembers]
    public DbAbilityCrestSet(string deviceAccountId, int id)
    {
        this.DeviceAccountId = deviceAccountId;
        this.AbilityCrestSetNo = id;
    }
}
