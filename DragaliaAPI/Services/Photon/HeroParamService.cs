using System.Collections.Immutable;
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

public class HeroParamService : IHeroParamService
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

    private readonly IDungeonRepository dungeonRepository;
    private readonly IWeaponRepository weaponRepository;
    private readonly IBonusService bonusService;
    private readonly IUserDataRepository userDataRepository;
    private readonly IPartyRepository partyRepository;
    private readonly ILogger<HeroParamService> logger;
    private readonly IPlayerIdentityService playerIdentityService;

    public HeroParamService(
        IDungeonRepository dungeonRepository,
        IWeaponRepository weaponRepository,
        IBonusService bonusService,
        IUserDataRepository userDataRepository,
        IPartyRepository partyRepository,
        ILogger<HeroParamService> logger,
        IPlayerIdentityService playerIdentityService
    )
    {
        this.dungeonRepository = dungeonRepository;
        this.weaponRepository = weaponRepository;
        this.bonusService = bonusService;
        this.userDataRepository = userDataRepository;
        this.partyRepository = partyRepository;
        this.logger = logger;
        this.playerIdentityService = playerIdentityService;
    }

    public async Task<List<HeroParam>> GetHeroParam(long viewerId, int partySlot)
    {
        this.logger.LogDebug("Fetching HeroParam for slot {partySlots}", partySlot);

        DbPlayerUserData userData = await this.userDataRepository.GetViewerData(viewerId)
            .SingleAsync();

        using IDisposable ctx = this.playerIdentityService.StartUserImpersonation(viewerId);

        List<DbDetailedPartyUnit> detailedPartyUnits =
            await this.dungeonRepository.BuildDetailedPartyUnit(
                partyRepository.GetPartyUnits(partySlot),
                partySlot
            )
                .ToListAsync();

        this.logger.LogDebug("Retrieved {n} party units", detailedPartyUnits.Count);

        foreach (DbDetailedPartyUnit unit in detailedPartyUnits)
        {
            if (unit.WeaponBodyData is not null)
            {
                unit.GameWeaponPassiveAbilityList = await this.weaponRepository.GetPassiveAbilities(
                    unit.WeaponBodyData.WeaponBodyId
                )
                    .ToListAsync();
            }
        }

        FortBonusList bonusList = await this.bonusService.GetBonusList();

        return detailedPartyUnits.Select(x => MapHeroParam(x, bonusList)).ToList();
    }

    private static HeroParam MapHeroParam(DbDetailedPartyUnit unit, FortBonusList fortBonusList)
    {
        CharaData charaData = MasterAsset.CharaData[unit.CharaData.CharaId];

        HeroParam result =
            new()
            {
                characterId = (int)unit.CharaData.CharaId,
                hp = unit.CharaData.Hp,
                attack = unit.CharaData.Attack,
                defence = 0, // Apparently meant to be zero
                ability1Lv = unit.CharaData.Ability1Level,
                ability2Lv = unit.CharaData.Ability2Level,
                ability3Lv = unit.CharaData.Ability3Level,
                skill1Lv = unit.CharaData.Skill1Level,
                skill2Lv = unit.CharaData.Skill2Level,
                level = unit.CharaData.Level,
                burstAttackLv = unit.CharaData.BurstAttackLevel,
                attackPlusCount = unit.CharaData.AttackPlusCount,
                hpPlusCount = unit.CharaData.HpPlusCount,
                exAbilityLv = unit.CharaData.ExAbilityLevel,
                exAbility2Lv = unit.CharaData.ExAbility2Level,
                comboBuildupCount = unit.CharaData.ComboBuildupCount,
                position = unit.Position,
                isEnemyTarget = true,
            };

        if (unit.DragonData is not null)
        {
            result.dragonId = (int)unit.DragonData.DragonId;
            result.dragonLevel = unit.DragonData.Level;
            result.dragonAbility1Lv = unit.DragonData.Ability1Level;
            result.dragonAbility2Lv = unit.DragonData.Ability2Level;
            result.dragonReliabilityLevel = unit.DragonReliabilityLevel;
            result.dragonAttackPlusCount = unit.DragonData.AttackPlusCount;
            result.dragonHpPlusCount = unit.DragonData.HpPlusCount;
            result.dragonSkill1Lv = unit.DragonData.Skill1Level;
            result.dragonSkill2Lv = 0; // ???
        }

        if (unit.WeaponBodyData is not null)
        {
            result.weaponBodyId = (int)unit.WeaponBodyData.WeaponBodyId;
            result.weaponBodyBuildupCount = unit.WeaponBodyData.BuildupCount;
            result.weaponBodyAbility1Lv = unit.WeaponBodyData.Ability1Level;
            result.weaponBodyAbility2Lv = unit.WeaponBodyData.Ability2Level;
            result.weaponBodySkillLv = unit.WeaponBodyData.SkillLevel;
            result.weaponBodySkillNo = unit.WeaponBodyData.SkillNo;
            result.weaponPassiveAbilityIds = unit.GameWeaponPassiveAbilityList.Select(
                x => x.WeaponPassiveAbilityId
            )
                .ToArray();
        }
        else
        {
            result.weaponBodyId = (int)DefaultWeapons[charaData.WeaponType];
        }

        if (unit.TalismanData is not null)
        {
            result.talismanId = (int)unit.TalismanData.TalismanId;
            result.talismanAbilityId1 = unit.TalismanData.TalismanAbilityId1;
            result.talismanAbilityId2 = unit.TalismanData.TalismanAbilityId2;
            result.talismanAbilityId3 = unit.TalismanData.TalismanAbilityId3;
            result.talismanAdditionalAttack = unit.TalismanData.AdditionalAttack;
            result.talismanAdditionalHp = unit.TalismanData.AdditionalHp;
        }

        if (unit.WeaponSkinData is not null)
        {
            result.weaponSkinId = unit.WeaponSkinData.WeaponSkinId;
        }

        // If AI units are given skill share data, it can cause softlocks for other players
        if (unit is { EditSkill1CharaData: not null, Position: 1 })
        {
            result.editSkillcharacterId1 = (int)unit.EditSkill1CharaData.CharaId;
            result.editSkillLv1 = unit.EditSkill1CharaData.EditSkillLevel;
        }

        if (unit is { EditSkill2CharaData: not null, Position: 1 })
        {
            result.editSkillcharacterId2 = (int)unit.EditSkill2CharaData.CharaId;
            result.editSkillLv2 = unit.EditSkill2CharaData.EditSkillLevel;
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
        CharaData charaData = MasterAsset.CharaData[unit.CharaData.CharaId];

        AtgenParamBonus paramBonus = fortBonusList.param_bonus.First(
            x => x.weapon_type == charaData.WeaponType
        );
        result.relativeAtkFort += paramBonus.attack / 100;
        result.relativeHpFort += paramBonus.hp / 100;

        AtgenElementBonus elementBonus = fortBonusList.element_bonus.First(
            x => x.elemental_type == charaData.ElementalType
        );
        result.relativeAtkFort += elementBonus.attack / 100;
        result.relativeHpFort += elementBonus.hp / 100;

        AtgenParamBonus paramBonusByWeapon = fortBonusList.param_bonus_by_weapon.First(
            x => x.weapon_type == charaData.WeaponType
        );
        result.relativeAtkFort += paramBonusByWeapon.attack / 100;
        result.relativeHpFort += paramBonusByWeapon.hp / 100;

        AtgenElementBonus charaAlbumBonus = fortBonusList.chara_bonus_by_album.First(
            x => x.elemental_type == charaData.ElementalType
        );
        result.relativeAtkAlbum += charaAlbumBonus.attack / 100;
        result.relativeHpAlbum += charaAlbumBonus.hp / 100;

        if (unit.DragonData is not null)
        {
            DragonData dragonData = MasterAsset.DragonData[unit.DragonData.DragonId];
            AtgenDragonBonus dragonBonus = fortBonusList.dragon_bonus.First(
                x => x.elemental_type == dragonData.ElementalType
            );
            AtgenElementBonus dragonAlbumBonus = fortBonusList.dragon_bonus_by_album.First(
                x => x.elemental_type == dragonData.ElementalType
            );

            result.dragonRelativeAtkFort += dragonBonus.attack / 100;
            result.dragonRelativeHpFort += dragonBonus.hp / 100;
            result.dragonRelativeDmg += dragonBonus.dragon_bonus / 100;
            result.dragonTime += fortBonusList.dragon_time_bonus.dragon_time_bonus;
            result.dragonRelativeAtkAlbum += dragonAlbumBonus.attack / 100;
            result.dragonRelativeHpAlbum += dragonAlbumBonus.hp / 100;
        }

        result.plusAtk += fortBonusList.all_bonus.attack / 100;
        result.plusHp += fortBonusList.all_bonus.hp / 100;
    }

    private static void MapCrests(DbDetailedPartyUnit unit, HeroParam result)
    {
        // I'm sorry.

        if (unit.CrestSlotType1CrestList.TryGetElementAt(0, out DbAbilityCrest? crest1))
        {
            result.abilityCrestId = (int)crest1.AbilityCrestId;
            result.abilityCrestAbility1Lv = crest1.AbilityLevel;
            result.abilityCrestAbility2Lv = crest1.AbilityLevel;
            result.abilityCrestBuildupCount = crest1.BuildupCount;
            result.abilityCrestHpPlusCount = crest1.HpPlusCount;
            result.abilityCrestAttackPlusCount = crest1.AttackPlusCount;
        }

        if (unit.CrestSlotType1CrestList.TryGetElementAt(1, out DbAbilityCrest? crest2))
        {
            result.abilityCrest2Id = (int)crest2.AbilityCrestId;
            result.abilityCrest2Ability1Lv = crest2.AbilityLevel;
            result.abilityCrest2Ability2Lv = crest2.AbilityLevel;
            result.abilityCrest2BuildupCount = crest2.BuildupCount;
            result.abilityCrest2HpPlusCount = crest2.HpPlusCount;
            result.abilityCrest2AttackPlusCount = crest2.AttackPlusCount;
        }

        if (unit.CrestSlotType1CrestList.TryGetElementAt(2, out DbAbilityCrest? crest3))
        {
            result.abilityCrest3Id = (int)crest3.AbilityCrestId;
            result.abilityCrest3Ability1Lv = crest3.AbilityLevel;
            result.abilityCrest3Ability2Lv = crest3.AbilityLevel;
            result.abilityCrest3BuildupCount = crest3.BuildupCount;
            result.abilityCrest3HpPlusCount = crest3.HpPlusCount;
            result.abilityCrest3AttackPlusCount = crest3.AttackPlusCount;
        }

        if (unit.CrestSlotType2CrestList.TryGetElementAt(0, out DbAbilityCrest? crest4))
        {
            result.abilityCrest4Id = (int)crest4.AbilityCrestId;
            result.abilityCrest4Ability1Lv = crest4.AbilityLevel;
            result.abilityCrest4Ability2Lv = crest4.AbilityLevel;
            result.abilityCrest4BuildupCount = crest4.BuildupCount;
            result.abilityCrest4HpPlusCount = crest4.HpPlusCount;
            result.abilityCrest4AttackPlusCount = crest4.AttackPlusCount;
        }

        if (unit.CrestSlotType2CrestList.TryGetElementAt(1, out DbAbilityCrest? crest5))
        {
            result.abilityCrest5Id = (int)crest5.AbilityCrestId;
            result.abilityCrest5Ability1Lv = crest5.AbilityLevel;
            result.abilityCrest5Ability2Lv = crest5.AbilityLevel;
            result.abilityCrest5BuildupCount = crest5.BuildupCount;
            result.abilityCrest5HpPlusCount = crest5.HpPlusCount;
            result.abilityCrest5AttackPlusCount = crest5.AttackPlusCount;
        }

        if (unit.CrestSlotType3CrestList.TryGetElementAt(0, out DbAbilityCrest? crest6))
        {
            result.abilityCrest6Id = (int)crest6.AbilityCrestId;
            result.abilityCrest6Ability1Lv = crest6.AbilityLevel;
            result.abilityCrest6Ability2Lv = crest6.AbilityLevel;
            result.abilityCrest6BuildupCount = crest6.BuildupCount;
            result.abilityCrest6HpPlusCount = crest6.HpPlusCount;
            result.abilityCrest6AttackPlusCount = crest6.AttackPlusCount;
        }

        if (unit.CrestSlotType3CrestList.TryGetElementAt(1, out DbAbilityCrest? crest7))
        {
            result.abilityCrest7Id = (int)crest7.AbilityCrestId;
            result.abilityCrest7Ability1Lv = crest7.AbilityLevel;
            result.abilityCrest7Ability2Lv = crest7.AbilityLevel;
            result.abilityCrest7BuildupCount = crest7.BuildupCount;
            result.abilityCrest7HpPlusCount = crest7.HpPlusCount;
            result.abilityCrest7AttackPlusCount = crest7.AttackPlusCount;
        }
    }
}
