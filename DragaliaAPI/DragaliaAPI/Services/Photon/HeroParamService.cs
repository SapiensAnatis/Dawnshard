using System.Collections.Immutable;
using System.Diagnostics;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Entities.Scaffold;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Extensions;
using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Photon.Shared.Models;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Services.Photon;

public class HeroParamService(
    IDungeonRepository dungeonRepository,
    IWeaponRepository weaponRepository,
    IBonusService bonusService,
    IPartyRepository partyRepository,
    ILogger<HeroParamService> logger,
    IPlayerIdentityService playerIdentityService
) : IHeroParamService
{
    private static readonly ImmutableDictionary<WeaponTypes, WeaponBodies> DefaultWeapons =
        new Dictionary<WeaponTypes, WeaponBodies>()
        {
            { WeaponTypes.Sword, WeaponBodies.BattlewornSword },
            { WeaponTypes.Katana, WeaponBodies.BattlewornBlade },
            { WeaponTypes.Dagger, WeaponBodies.BattlewornDagger },
            { WeaponTypes.Axe, WeaponBodies.BattlewornAxe },
            { WeaponTypes.Lance, WeaponBodies.BattlewornLance },
            { WeaponTypes.Bow, WeaponBodies.BattlewornBow },
            { WeaponTypes.Rod, WeaponBodies.BattlewornWand },
            { WeaponTypes.Cane, WeaponBodies.BattlewornStaff },
            { WeaponTypes.Gun, WeaponBodies.BattlewornManacaster }
        }.ToImmutableDictionary();

    public async Task<List<HeroParam>> GetHeroParam(long viewerId, int partySlot)
    {
        logger.LogDebug("Fetching HeroParam for slot {partySlots}", partySlot);

        using IDisposable ctx = playerIdentityService.StartUserImpersonation(viewerId);

        List<DbDetailedPartyUnit> detailedPartyUnits = await dungeonRepository
            .BuildDetailedPartyUnit(partyRepository.GetPartyUnits(partySlot), partySlot)
            .ToListAsync();

        logger.LogDebug("Retrieved {n} party units", detailedPartyUnits.Count);

        foreach (DbDetailedPartyUnit unit in detailedPartyUnits)
        {
            if (unit.CharaData is not null)
            {
                unit.GameWeaponPassiveAbilityList = await weaponRepository
                    .GetPassiveAbilities(unit.CharaData.CharaId)
                    .ToListAsync();
            }
        }

        FortBonusList bonusList = await bonusService.GetBonusList();

        return detailedPartyUnits
            .Select(x => MapHeroParam(x, bonusList))
            .OfType<HeroParam>()
            .ToList();
    }

    private static HeroParam? MapHeroParam(DbDetailedPartyUnit unit, FortBonusList fortBonusList)
    {
        if (unit.CharaData is null)
            return null;

        CharaData charaData = MasterAsset.CharaData[unit.CharaData.CharaId];

        HeroParam result =
            new()
            {
                CharacterId = (int)unit.CharaData.CharaId,
                Hp = unit.CharaData.Hp,
                Attack = unit.CharaData.Attack,
                Defence = 0, // Apparently meant to be zero
                Ability1Lv = unit.CharaData.Ability1Level,
                Ability2Lv = unit.CharaData.Ability2Level,
                Ability3Lv = unit.CharaData.Ability3Level,
                Skill1Lv = unit.CharaData.Skill1Level,
                Skill2Lv = unit.CharaData.Skill2Level,
                Level = unit.CharaData.Level,
                BurstAttackLv = unit.CharaData.BurstAttackLevel,
                AttackPlusCount = unit.CharaData.AttackPlusCount,
                HpPlusCount = unit.CharaData.HpPlusCount,
                ExAbilityLv = unit.CharaData.ExAbilityLevel,
                ExAbility2Lv = unit.CharaData.ExAbility2Level,
                ComboBuildupCount = unit.CharaData.ComboBuildupCount,
                Position = unit.Position,
                IsEnemyTarget = true,
            };

        if (unit.DragonData is not null)
        {
            result.DragonId = (int)unit.DragonData.DragonId;
            result.DragonLevel = unit.DragonData.Level;
            result.DragonAbility1Lv = unit.DragonData.Ability1Level;
            result.DragonAbility2Lv = unit.DragonData.Ability2Level;
            result.DragonReliabilityLevel = unit.DragonReliabilityLevel;
            result.DragonAttackPlusCount = unit.DragonData.AttackPlusCount;
            result.DragonHpPlusCount = unit.DragonData.HpPlusCount;
            result.DragonSkill1Lv = unit.DragonData.Skill1Level;
            result.DragonSkill2Lv = 0; // ???
        }

        result.WeaponPassiveAbilityIds = unit
            .GameWeaponPassiveAbilityList.Select(x => x.WeaponPassiveAbilityId)
            .ToArray();

        if (unit.WeaponBodyData is not null)
        {
            result.WeaponBodyId = (int)unit.WeaponBodyData.WeaponBodyId;
            result.WeaponBodyBuildupCount = unit.WeaponBodyData.BuildupCount;
            result.WeaponBodyAbility1Lv = unit.WeaponBodyData.Ability1Level;
            result.WeaponBodyAbility2Lv = unit.WeaponBodyData.Ability2Level;
            result.WeaponBodySkillLv = unit.WeaponBodyData.SkillLevel;
            result.WeaponBodySkillNo = unit.WeaponBodyData.SkillNo;
        }
        else
        {
            result.WeaponBodyId = (int)DefaultWeapons[charaData.WeaponType];
        }

        if (unit.TalismanData is not null)
        {
            result.TalismanId = (int)unit.TalismanData.TalismanId;
            result.TalismanAbilityId1 = unit.TalismanData.TalismanAbilityId1;
            result.TalismanAbilityId2 = unit.TalismanData.TalismanAbilityId2;
            result.TalismanAbilityId3 = unit.TalismanData.TalismanAbilityId3;
            result.TalismanAdditionalAttack = unit.TalismanData.AdditionalAttack;
            result.TalismanAdditionalHp = unit.TalismanData.AdditionalHp;
        }

        if (unit.WeaponSkinData is not null)
        {
            result.WeaponSkinId = unit.WeaponSkinData.WeaponSkinId;
        }

        // If AI units are given skill share data, it can cause softlocks for other players
        if (unit is { EditSkill1CharaData: not null, Position: 1 })
        {
            result.EditSkillCharacterId1 = (int)unit.EditSkill1CharaData.CharaId;
            result.EditSkillLv1 = unit.EditSkill1CharaData.EditSkillLevel;
        }

        if (unit is { EditSkill2CharaData: not null, Position: 1 })
        {
            result.EditSkillCharacterId2 = (int)unit.EditSkill2CharaData.CharaId;
            result.EditSkillLv2 = unit.EditSkill2CharaData.EditSkillLevel;
        }

        MapBonuses(unit, fortBonusList, result);
        MapCrests(unit, result);

        return result;
    }

    private static void MapBonuses(
        DbDetailedPartyUnit unit,
        FortBonusList fortBonusList,
        HeroParam result
    )
    {
        if (unit.CharaData is null)
        {
            // Validated in above method
            throw new UnreachableException("CharaData is null");
        }

        CharaData charaData = MasterAsset.CharaData[unit.CharaData.CharaId];

        AtgenParamBonus paramBonus = fortBonusList.ParamBonus.First(x =>
            x.WeaponType == charaData.WeaponType
        );
        result.RelativeAtkFort += paramBonus.Attack / 100;
        result.RelativeHpFort += paramBonus.Hp / 100;

        AtgenElementBonus elementBonus = fortBonusList.ElementBonus.First(x =>
            x.ElementalType == charaData.ElementalType
        );
        result.RelativeAtkFort += elementBonus.Attack / 100;
        result.RelativeHpFort += elementBonus.Hp / 100;

        AtgenParamBonus paramBonusByWeapon = fortBonusList.ParamBonusByWeapon.First(x =>
            x.WeaponType == charaData.WeaponType
        );
        result.RelativeAtkFort += paramBonusByWeapon.Attack / 100;
        result.RelativeHpFort += paramBonusByWeapon.Hp / 100;

        AtgenElementBonus charaAlbumBonus = fortBonusList.CharaBonusByAlbum.First(x =>
            x.ElementalType == charaData.ElementalType
        );
        result.RelativeAtkAlbum += charaAlbumBonus.Attack / 100;
        result.RelativeHpAlbum += charaAlbumBonus.Hp / 100;

        if (unit.DragonData is not null)
        {
            DragonData dragonData = MasterAsset.DragonData[unit.DragonData.DragonId];
            AtgenDragonBonus dragonBonus = fortBonusList.DragonBonus.First(x =>
                x.ElementalType == dragonData.ElementalType
            );
            AtgenElementBonus dragonAlbumBonus = fortBonusList.DragonBonusByAlbum.First(x =>
                x.ElementalType == dragonData.ElementalType
            );

            result.DragonRelativeAtkFort += dragonBonus.Attack / 100;
            result.DragonRelativeHpFort += dragonBonus.Hp / 100;
            result.DragonRelativeDmg += dragonBonus.DragonBonus / 100;
            result.DragonTime += fortBonusList.DragonTimeBonus.DragonTimeBonus;
            result.DragonRelativeAtkAlbum += dragonAlbumBonus.Attack / 100;
            result.DragonRelativeHpAlbum += dragonAlbumBonus.Hp / 100;
        }

        result.PlusAtk += fortBonusList.AllBonus.Attack / 100;
        result.PlusHp += fortBonusList.AllBonus.Hp / 100;
    }

    private static void MapCrests(DbDetailedPartyUnit unit, HeroParam result)
    {
        // I'm sorry.

        if (unit.CrestSlotType1CrestList.TryGetElementAt(0, out DbAbilityCrest? crest1))
        {
            result.AbilityCrestId = (int)crest1.AbilityCrestId;
            result.AbilityCrestAbility1Lv = crest1.AbilityLevel;
            result.AbilityCrestAbility2Lv = crest1.AbilityLevel;
            result.AbilityCrestBuildupCount = crest1.BuildupCount;
            result.AbilityCrestHpPlusCount = crest1.HpPlusCount;
            result.AbilityCrestAttackPlusCount = crest1.AttackPlusCount;
        }

        if (unit.CrestSlotType1CrestList.TryGetElementAt(1, out DbAbilityCrest? crest2))
        {
            result.AbilityCrest2Id = (int)crest2.AbilityCrestId;
            result.AbilityCrest2Ability1Lv = crest2.AbilityLevel;
            result.AbilityCrest2Ability2Lv = crest2.AbilityLevel;
            result.AbilityCrest2BuildupCount = crest2.BuildupCount;
            result.AbilityCrest2HpPlusCount = crest2.HpPlusCount;
            result.AbilityCrest2AttackPlusCount = crest2.AttackPlusCount;
        }

        if (unit.CrestSlotType1CrestList.TryGetElementAt(2, out DbAbilityCrest? crest3))
        {
            result.AbilityCrest3Id = (int)crest3.AbilityCrestId;
            result.AbilityCrest3Ability1Lv = crest3.AbilityLevel;
            result.AbilityCrest3Ability2Lv = crest3.AbilityLevel;
            result.AbilityCrest3BuildupCount = crest3.BuildupCount;
            result.AbilityCrest3HpPlusCount = crest3.HpPlusCount;
            result.AbilityCrest3AttackPlusCount = crest3.AttackPlusCount;
        }

        if (unit.CrestSlotType2CrestList.TryGetElementAt(0, out DbAbilityCrest? crest4))
        {
            result.AbilityCrest4Id = (int)crest4.AbilityCrestId;
            result.AbilityCrest4Ability1Lv = crest4.AbilityLevel;
            result.AbilityCrest4Ability2Lv = crest4.AbilityLevel;
            result.AbilityCrest4BuildupCount = crest4.BuildupCount;
            result.AbilityCrest4HpPlusCount = crest4.HpPlusCount;
            result.AbilityCrest4AttackPlusCount = crest4.AttackPlusCount;
        }

        if (unit.CrestSlotType2CrestList.TryGetElementAt(1, out DbAbilityCrest? crest5))
        {
            result.AbilityCrest5Id = (int)crest5.AbilityCrestId;
            result.AbilityCrest5Ability1Lv = crest5.AbilityLevel;
            result.AbilityCrest5Ability2Lv = crest5.AbilityLevel;
            result.AbilityCrest5BuildupCount = crest5.BuildupCount;
            result.AbilityCrest5HpPlusCount = crest5.HpPlusCount;
            result.AbilityCrest5AttackPlusCount = crest5.AttackPlusCount;
        }

        if (unit.CrestSlotType3CrestList.TryGetElementAt(0, out DbAbilityCrest? crest6))
        {
            result.AbilityCrest6Id = (int)crest6.AbilityCrestId;
            result.AbilityCrest6Ability1Lv = crest6.AbilityLevel;
            result.AbilityCrest6Ability2Lv = crest6.AbilityLevel;
            result.AbilityCrest6BuildupCount = crest6.BuildupCount;
            result.AbilityCrest6HpPlusCount = crest6.HpPlusCount;
            result.AbilityCrest6AttackPlusCount = crest6.AttackPlusCount;
        }

        if (unit.CrestSlotType3CrestList.TryGetElementAt(1, out DbAbilityCrest? crest7))
        {
            result.AbilityCrest7Id = (int)crest7.AbilityCrestId;
            result.AbilityCrest7Ability1Lv = crest7.AbilityLevel;
            result.AbilityCrest7Ability2Lv = crest7.AbilityLevel;
            result.AbilityCrest7BuildupCount = crest7.BuildupCount;
            result.AbilityCrest7HpPlusCount = crest7.HpPlusCount;
            result.AbilityCrest7AttackPlusCount = crest7.AttackPlusCount;
        }
    }
}
