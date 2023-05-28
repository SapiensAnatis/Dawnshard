using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Entities.Scaffold;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Extensions;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Photon.Dto;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Services.Photon;

public class HeroParamService : IHeroParamService
{
    private readonly IUnitRepository unitRepository;
    private readonly IBonusService bonusService;
    private readonly IUserDataRepository userDataRepository;
    private readonly IPartyRepository partyRepository;

    public HeroParamService(
        IUnitRepository unitRepository,
        IBonusService bonusService,
        IUserDataRepository userDataRepository,
        IPartyRepository partyRepository
    )
    {
        this.unitRepository = unitRepository;
        this.bonusService = bonusService;
        this.userDataRepository = userDataRepository;
        this.partyRepository = partyRepository;
    }

    public async Task<IEnumerable<HeroParam>> GetHeroParam(long viewerId)
    {
        List<HeroParam> result = new();

        DbPlayerUserData userData = await this.userDataRepository
            .GetUserData(viewerId)
            .FirstAsync();

        List<DbDetailedPartyUnit> detailedPartyUnits = await this.unitRepository
            .BuildDetailedPartyUnit(
                userData.DeviceAccountId,
                partyRepository.GetPartyUnits(userData.DeviceAccountId, userData.MainPartyNo)
            )
            .ToListAsync();

        FortBonusList bonusList = await this.bonusService.GetBonusList(userData.DeviceAccountId);

        return detailedPartyUnits.Select(x => MapHeroParam(x, bonusList));
    }

    private static HeroParam MapHeroParam(DbDetailedPartyUnit unit, FortBonusList fortBonusList)
    {
        HeroParam result =
            new()
            {
                characterId = (int)unit.CharaData.CharaId,
                hp = unit.CharaData.Hp,
                attack = unit.CharaData.Attack,
                defence = 0, // ???
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
            result.weaponBodyAbility1Lv = unit.WeaponBodyData.Ability1Level;
            result.weaponBodyAbility2Lv = unit.WeaponBodyData.Ability2Level;
            result.weaponBodySkillLv = unit.WeaponBodyData.SkillLevel;
            result.weaponBodySkillNo = unit.WeaponBodyData.SkillNo;
            result.weaponPassiveAbilityIds = unit.GameWeaponPassiveAbilityList
                .Select(x => x.WeaponPassiveAbilityId)
                .ToArray();
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

        if (unit.EditSkill1CharaData is not null)
        {
            result.editSkillcharacterId1 = (int)unit.EditSkill1CharaData.CharaId;
            result.editSkillLv1 = unit.EditSkill1CharaData.EditSkillLevel;
        }

        if (unit.EditSkill2CharaData is not null)
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
        result.relativeAtkFort += paramBonus.attack;
        result.relativeHpFort += paramBonus.hp;

        AtgenElementBonus elementBonus = fortBonusList.element_bonus.First(
            x => x.elemental_type == charaData.ElementalType
        );
        result.relativeAtkFort += elementBonus.attack;
        result.relativeHpFort += elementBonus.hp;

        AtgenParamBonus paramBonusByWeapon = fortBonusList.param_bonus_by_weapon.First(
            x => x.weapon_type == charaData.WeaponType
        );
        result.relativeAtkFort += paramBonusByWeapon.attack;
        result.relativeHpFort += paramBonusByWeapon.hp;

        AtgenElementBonus charaAlbumBonus = fortBonusList.chara_bonus_by_album.First(
            x => x.elemental_type == charaData.ElementalType
        );
        result.relativeAtkAlbum += charaAlbumBonus.attack;
        result.relativeHpAlbum += charaAlbumBonus.hp;

        if (unit.DragonData is not null)
        {
            DragonData dragonData = MasterAsset.DragonData[unit.DragonData.DragonId];
            AtgenDragonBonus dragonBonus = fortBonusList.dragon_bonus.First(
                x => x.elemental_type == dragonData.ElementalType
            );
            AtgenElementBonus dragonAlbumBonus = fortBonusList.dragon_bonus_by_album.First(
                x => x.elemental_type == dragonData.ElementalType
            );

            result.dragonRelativeAtkFort += dragonBonus.attack;
            result.dragonRelativeHpFort += dragonBonus.hp;
            result.dragonTime += dragonBonus.dragon_bonus;
            result.dragonRelativeAtkAlbum += dragonAlbumBonus.attack;
            result.dragonRelativeHpAlbum += dragonAlbumBonus.hp;
        }
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
