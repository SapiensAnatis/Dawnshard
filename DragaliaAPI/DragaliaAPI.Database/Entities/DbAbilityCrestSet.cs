using System.Diagnostics.CodeAnalysis;
using DragaliaAPI.Database.Entities.Abstract;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

/// <summary>
/// Wyrmprint set database entity.
/// </summary>
[PrimaryKey(nameof(ViewerId), nameof(AbilityCrestSetNo))]
public class DbAbilityCrestSet : DbPlayerData
{
    public DbAbilityCrestSet() { }

    [SetsRequiredMembers]
    public DbAbilityCrestSet(long viewerId, int setNo)
    {
        this.ViewerId = viewerId;
        this.AbilityCrestSetNo = setNo;
    }

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
}
