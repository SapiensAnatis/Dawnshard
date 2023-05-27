using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Entities.Scaffold;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Extensions;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Photon.Dto;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;

namespace DragaliaAPI.Services.Photon;

public class HeroParamService
{
    private readonly IUnitRepository unitRepository;
    private readonly IBonusService bonusService;

    public HeroParamService(IUnitRepository unitRepository, IBonusService bonusService)
    {
        this.unitRepository = unitRepository;
        this.bonusService = bonusService;
    }

    public async Task<IEnumerable<HeroParam>> GetHeroParam(string deviceAccountId) { }

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

        if (unit.WeaponSkinData is not null)
        {
            result.weaponSkinId = unit.WeaponSkinData.WeaponSkinId;
        }

        MapBonuses(unit, fortBonusList, result);

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
        if (unit.CrestSlotType1CrestList.TryGetElementAt(0, out DbAbilityCrest? crest1))
        {
            result.abilityCrestId = (int)crest1.AbilityCrestId;
        }
    }
}
