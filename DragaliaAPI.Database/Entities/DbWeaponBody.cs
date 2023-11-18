﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Database.Entities.Abstract;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DragaliaAPI.Database.Entities;

/// <summary>
/// Weapon database entity.
/// </summary>
[PrimaryKey(nameof(ViewerId), nameof(WeaponBodyId))]
public class DbWeaponBody : DbPlayerData
{
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
    public int LimitBreakCount { get; set; }

    /// <summary>
    /// Gets or sets a value indicating how many times the weapon has been refined.
    /// </summary>
    public int LimitOverCount { get; set; }

    /// <summary>
    /// Gets or sets a value indicating how many copies of the weapon are owned.
    /// </summary>
    public int EquipableCount { get; set; } = 1;

    /// <summary>
    /// Gets or sets a value indicating how many additional 5(?)-star print slots have been unlocked.
    /// </summary>
    public int AdditionalCrestSlotType1Count { get; set; }

    /// <summary>
    /// Gets or sets a value indicating how many additional 4(?)-star print slots have been unlocked.
    /// </summary>
    public int AdditionalCrestSlotType2Count { get; set; }

    /// <summary>
    /// Gets or sets a value indicating how many additional SinDom(?) print slots have been unlocked.
    /// </summary>
    public int AdditionalCrestSlotType3Count { get; set; }

    /// <summary>
    /// Gets an unknown value.
    /// <remarks>Always 0 on my endgame savefile, for all 235 weapons.</remarks>
    /// </summary>
    [NotMapped]
    public int AdditionalEffectCount { get; }

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
    public int FortPassiveCharaWeaponBuildupCount { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the weapon is new.
    /// </summary>
    [TypeConverter(typeof(BooleanConverter))]
    public bool IsNew { get; set; }

    /// <summary>
    /// Gets or sets the time at which the weapon was received.
    /// </summary>
    [TypeConverter(typeof(DateTimeOffsetConverter))]
    public DateTimeOffset GetTime { get; set; } = DateTimeOffset.UtcNow;

    [NotMapped]
    public int Ability1Level
    {
        get
        {
            if (!MasterAsset.WeaponBody.TryGetValue(this.WeaponBodyId, out WeaponBody? weaponData))
                throw new InvalidOperationException("Invalid weapon ID!");

            List<int> abilityIds =
                new() { weaponData.Abilities11, weaponData.Abilities12, weaponData.Abilities13 };

            return GetAbilityLevel(abilityIds);
        }
    }

    [NotMapped]
    public int Ability2Level
    {
        get
        {
            if (!MasterAsset.WeaponBody.TryGetValue(this.WeaponBodyId, out WeaponBody? weaponData))
                throw new InvalidOperationException("Invalid weapon ID!");

            List<int> abilityIds =
                new() { weaponData.Abilities21, weaponData.Abilities22, weaponData.Abilities23 };

            return GetAbilityLevel(abilityIds);
        }
    }

    [NotMapped]
    public int SkillNo
    {
        get
        {
            if (!MasterAsset.WeaponBody.TryGetValue(this.WeaponBodyId, out WeaponBody? weaponData))
                throw new InvalidOperationException("Invalid weapon ID!");

            List<int> skillIds =
                new()
                {
                    weaponData.ChangeSkillId1,
                    weaponData.ChangeSkillId2,
                    weaponData.ChangeSkillId3
                };

            return GetCurrentSkillNo(skillIds);
        }
    }

    [NotMapped]
    public int SkillLevel =>
        // On the second skill it takes 8 unbinds to level up the skill
        // On the first skill it's always 4
        (this.LimitBreakCount >= this.SkillNo * 4)
            ? 2
            : 1;

    private int GetAbilityLevel(List<int> inputAbilityIds)
    {
        // Match the limit break count to the highest defined (!= 0) ability id
        int result = this.LimitOverCount + 1;

        // Min return value: 0, so break when result == 0
        while (inputAbilityIds.ElementAtOrDefault(result - 1) == default && result > 0)
            result--;

        return result;
    }

    private int GetCurrentSkillNo(List<int> inputSkillIds)
    {
        // Match the limit break count to the highest defined (!= 0) skill id
        // except it must be distinct as Agito weapons have all 3 defined but have a max skill level of 2
        IEnumerable<int> distinctSkillIds = inputSkillIds.Where(x => x != 0).Distinct();

        // If the weapon has no skills
        if (!distinctSkillIds.Any())
            return 0;

        int result = this.LimitOverCount + 1;

        // Min return value: 1, so break when result == 1
        while (distinctSkillIds.ElementAtOrDefault(result - 1) == default && result > 1)
            result--;

        return result;
    }
}
