using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DragaliaAPI.Database.Entities;

/// <summary>
/// Weapon database entity.
/// </summary>
[Index(nameof(DeviceAccountId))]
public class DbWeaponBody : IDbHasAccountId
{
    /// <inheritdoc />
    public virtual DbPlayer? Owner { get; set; }

    /// <inheritdoc />
    [ForeignKey(nameof(Owner))]
    public required string DeviceAccountId { get; set; }

    /// <summary>
    /// Gets or sets a value that dictates the weapon's identity.
    /// </summary>
    public WeaponBodies WeaponBodyId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating the weapon's level.
    /// </summary>
    public int BuildupCount { get; set; }

    /// <summary>
    /// Gets or sets a value indicating the weapon's unbind status.
    /// </summary>
    public int LimitBreakCount { get; set; } = 0;

    /// <summary>
    /// Gets or sets a value indicating how many times the weapon has been refined.
    /// </summary>
    public int LimitOverCount { get; set; } = 0;

    /// <summary>
    /// Gets or sets a value indicating how many copies of the weapon are owned.
    /// </summary>
    public int EquipableCount { get; set; } = 1;

    /// <summary>
    /// Gets or sets a value indicating how many additional 5(?)-star print slots have been unlocked.
    /// </summary>
    public int AdditionalCrestSlotType1Count { get; set; } = 0;

    /// <summary>
    /// Gets or sets a value indicating how many additional 4(?)-star print slots have been unlocked.
    /// </summary>
    public int AdditionalCrestSlotType2Count { get; set; } = 0;

    /// <summary>
    /// Gets or sets a value indicating how many additional SinDom(?) print slots have been unlocked.
    /// </summary>
    public int AdditionalCrestSlotType3Count { get; set; } = 0;

    /// <summary>
    /// Gets an unknown value.
    /// <remarks>Always 0 on my endgame savefile, for all 235 weapons.</remarks>
    /// </summary>
    [NotMapped]
    public int AdditionalEffectCount { get; } = 0;

    public string UnlockWeaponPassiveAbilityNoString { get; private set; } =
        "0,0,0,0,0,0,0,0,0,0,0,0,0,0,0";

    /// <summary>
    /// Gets or sets a list of passive abilities that are unlocked on the weapon.
    /// </summary>
    [NotMapped]
    public IEnumerable<int> UnlockWeaponPassiveAbilityNoList
    {
        // I will consider replacing this with a bitmask if we generate a unified helper for them
        // Currently opposed to having another util class
        get => this.UnlockWeaponPassiveAbilityNoString.Split(",").Select(int.Parse);
        set => this.UnlockWeaponPassiveAbilityNoString = string.Join(",", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the weapon bonus has been unlocked.
    /// </summary>
    public int FortPassiveCharaWeaponBuildupCount { get; set; } = 0;

    /// <summary>
    /// Gets or sets a value indicating whether the weapon is new.
    /// </summary>
    [TypeConverter(typeof(BooleanConverter))]
    public bool IsNew { get; set; } = false;

    /// <summary>
    /// Gets or sets the time at which the weapon was received.
    /// </summary>
    [TypeConverter(typeof(DateTimeOffsetConverter))]
    public DateTimeOffset GetTime { get; set; } = DateTimeOffset.UtcNow;
}

internal class DbWeaponBodyConfiguration : IEntityTypeConfiguration<DbWeaponBody>
{
    public void Configure(EntityTypeBuilder<DbWeaponBody> builder)
    {
        builder.HasKey(e => new { e.DeviceAccountId, e.WeaponBodyId });
    }
}
