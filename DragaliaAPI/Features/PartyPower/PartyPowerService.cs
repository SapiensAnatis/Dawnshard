using System.Diagnostics;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.PartyPower;

public class PartyPowerService(
    IUnitRepository unitRepository,
    IAbilityCrestRepository abilityCrestRepository,
    IBonusService bonusService
) : IPartyPowerService
{
    public async Task<int> CalculateCharacterPower(PartySettingList partyList)
    {
        if (partyList.chara_id == 0)
            return 0;

        FortBonusList bonus = await bonusService.GetBonusList();

        // TODO: Ability party power param
        double abilityPartyPowerParam = 0;

        DbPlayerCharaData chara =
            await unitRepository.FindCharaAsync(partyList.chara_id)
            ?? throw new UnreachableException();

        CharaData charaData = MasterAsset.CharaData[partyList.chara_id];

        double charaPowerParam = GetCharacterPower(ref chara, ref bonus);

        DbPlayerDragonData? dragon =
            partyList.equip_dragon_key_id == 0
                ? null
                : await unitRepository.Dragons.SingleAsync(
                    x => x.DragonKeyId == (long)partyList.equip_dragon_key_id
                );

        DbPlayerDragonReliability? reliability =
            dragon == null
                ? null
                : await unitRepository.DragonReliabilities.SingleOrDefaultAsync(
                    x => x.DragonId == dragon.DragonId
                );

        double dragonPowerParam = GetDragonPower(
            dragon,
            reliability,
            ref bonus,
            charaData.ElementalType
        );

        AbilityCrests[] crests =
        {
            partyList.equip_crest_slot_type_1_crest_id_1,
            partyList.equip_crest_slot_type_1_crest_id_2,
            partyList.equip_crest_slot_type_1_crest_id_3,
            partyList.equip_crest_slot_type_2_crest_id_1,
            partyList.equip_crest_slot_type_2_crest_id_2,
            partyList.equip_crest_slot_type_3_crest_id_1,
            partyList.equip_crest_slot_type_3_crest_id_2
        };

        ulong talismanId = partyList.equip_talisman_key_id;

        List<DbAbilityCrest> dbCrests = await abilityCrestRepository.AbilityCrests
            .Where(x => crests.Contains(x.AbilityCrestId))
            .ToListAsync();
        DbTalisman? talisman =
            talismanId == 0
                ? null
                : await unitRepository.Talismans.SingleAsync(
                    x => x.TalismanKeyId == (long)talismanId
                );

        double crestPowerParam = GetCrestPower(dbCrests, talisman);

        DbWeaponBody? weaponBody =
            partyList.equip_weapon_body_id == 0
                ? null
                : await unitRepository.WeaponBodies.SingleAsync(
                    x => x.WeaponBodyId == partyList.equip_weapon_body_id
                );

        double weaponPowerParam = GetWeaponPower(weaponBody, charaData.ElementalType);

        double exAbilityPowerParam = GetExAbilityPower(ref chara, ref charaData);

        double unionBonusPowerParam = GetUnionAbilityPower(dbCrests);

        double power =
            unionBonusPowerParam
            + crestPowerParam
            + dragonPowerParam
            + charaPowerParam
            + weaponPowerParam
            + abilityPartyPowerParam
            + exAbilityPowerParam;

        return CeilToInt(power);
    }

    private static double GetCharacterPower(ref DbPlayerCharaData dbChara, ref FortBonusList bonus)
    {
        if (dbChara.CharaId == 0)
            return 0;

        CharaData charaData = MasterAsset.CharaData[dbChara.CharaId];

        (int statusPlusAtk, int statusPlusHp) = GetStatusPlusParam(ref bonus);

        BonusParams bonusParams = BonusParams.GetBonus(ref bonus, charaData.Id);

        int normalAtk = dbChara.Attack + dbChara.AttackPlusCount;
        int normalHp = dbChara.Hp + dbChara.HpPlusCount;

        int fortAtk = CeilToInt((normalAtk * bonusParams.FortAtk) + statusPlusAtk);
        int fortHp = CeilToInt((normalHp * bonusParams.FortHp) + statusPlusHp);

        int albumAtk = CeilToInt(normalAtk * bonusParams.AlbumAtk);
        int albumHp = CeilToInt(normalHp * bonusParams.AlbumHp);

        int charaAtk = normalAtk + fortAtk + albumAtk;
        int charaHp = normalHp + fortHp + albumHp;

        double skillPower = (dbChara.Skill1Level + dbChara.Skill2Level) * 100.0;
        double burstPower = dbChara.BurstAttackLevel * 60.0;
        double comboPower = dbChara.ComboBuildupCount * 200.0;

        double charaPowerParam = charaAtk + charaHp + skillPower + burstPower + comboPower;

        return charaPowerParam;
    }

    private static double GetCrestPower(IEnumerable<DbAbilityCrest> crests, DbTalisman? talisman)
    {
        int totalCrestAtk = 0;
        int totalCrestHp = 0;

        foreach (DbAbilityCrest crest in crests)
        {
            (int crestAtk, int crestHp) = GetAbilityCrest(
                crest.AbilityCrestId,
                crest.BuildupCount,
                crest.AttackPlusCount,
                crest.HpPlusCount
            );
            totalCrestAtk += crestAtk;
            totalCrestHp += crestHp;
        }

        if (talisman != null)
        {
            totalCrestHp += 20 + talisman.AdditionalHp;
            totalCrestAtk += 10 + talisman.AdditionalAttack;
        }

        double crestPower = totalCrestHp + totalCrestAtk;

        return crestPower;
    }

    private static double GetDragonPower(
        DbPlayerDragonData? dbDragon,
        DbPlayerDragonReliability? reliability,
        ref FortBonusList bonus,
        UnitElement charaElement
    )
    {
        if (dbDragon == null)
            return 0;

        DragonData dragonData = MasterAsset.DragonData[dbDragon.DragonId];
        DragonRarity rarity = MasterAsset.DragonRarity[dragonData.Rarity];

        int maxLevel = rarity.Id == 5 ? rarity.LimitLevel04 : rarity.MaxLimitLevel;

        int levelMultiplier = Math.Min(dbDragon.Level, maxLevel);

        int dragonHp = CeilToInt(
            ((maxLevel + -1.0) / (levelMultiplier + -1.0) * (dragonData.MaxHp - dragonData.MinHp))
                + dragonData.MinHp
        );
        int dragonAtk = CeilToInt(
            ((maxLevel + -1.0) / (levelMultiplier + -1.0) * (dragonData.MaxAtk - dragonData.MinAtk))
                + dragonData.MinAtk
        );

        if (dragonData.MaxLimitBreakCount == 5)
        {
            dragonAtk +=
                (dragonData.AddMaxAtk1 - dragonData.MaxAtk)
                * (Math.Min(dbDragon.Level, rarity.LimitLevel05) - rarity.LimitLevel04)
                / (rarity.LimitLevel05 - rarity.LimitLevel04);

            dragonHp +=
                (dragonData.AddMaxHp1 - dragonData.MaxHp)
                * (Math.Min(dbDragon.Level, rarity.LimitLevel05) - rarity.LimitLevel04)
                / (rarity.LimitLevel05 - rarity.LimitLevel04);
        }

        double multiplier = dragonData.ElementalType == charaElement ? 1.5 : 1;

        dragonAtk = CeilToInt((dragonAtk + dbDragon.AttackPlusCount) * multiplier);
        dragonHp = CeilToInt((dragonHp + dbDragon.HpPlusCount) * multiplier);

        // set totalUnitAtk + totalUnitHp

        BonusParams bonusParams = BonusParams.GetBonus(ref bonus, dbDragon.DragonId);

        int fortAtk = CeilToInt(dragonAtk * bonusParams.FortAtk);
        int fortHp = CeilToInt(dragonHp * bonusParams.FortHp);

        int albumAtk = CeilToInt(dragonAtk * bonusParams.AlbumAtk);
        int albumHp = CeilToInt(dragonHp * bonusParams.AlbumHp);

        double dragonPowerParam =
            dragonAtk
            + fortAtk
            + albumAtk
            + dragonHp
            + fortHp
            + albumHp
            + (dbDragon.Skill1Level * 50.0)
            + ((reliability?.Level ?? 1) * 10.0)
            + rarity.RarityBasePartyPower
            + (rarity.LimitBreakCountPartyPowerWeight * dbDragon.LimitBreakCount);

        return dragonPowerParam;
    }

    private static double GetWeaponPower(DbWeaponBody? dbWeapon, UnitElement charaElement)
    {
        if (dbWeapon == null)
            return 0;

        WeaponBody weaponBody = MasterAsset.WeaponBody[dbWeapon.WeaponBodyId];

        // TODO: Weapon body calculations
        // TODO: Fix this calculation - very convoluted
        int weaponBodyHp = weaponBody.MaxHp2;
        int weaponBodyAtk = weaponBody.MaxAtk2;

        double weaponPower = weaponBodyHp + weaponBodyAtk;
        weaponPower *= weaponBody.ElementalType == charaElement ? 1.5 : 1;

        double weaponSkillPower = dbWeapon.SkillLevel * 50.0;

        double weaponBodyPowerParam = weaponPower + weaponSkillPower;

        if (dbWeapon.LimitOverCount >= 1)
            weaponBodyPowerParam += weaponBody.LimitOverCountPartyPower1;

        if (dbWeapon.LimitOverCount >= 2)
            weaponBodyPowerParam += weaponBody.LimitOverCountPartyPower2;

        return weaponBodyPowerParam;
    }

    private static double GetExAbilityPower(ref DbPlayerCharaData dbChara, ref CharaData charaData)
    {
        if (dbChara.ExAbilityLevel == 0)
            return 0;

        double power = MasterAsset.ExAbilityData[
            charaData.ExAbility[dbChara.ExAbilityLevel - 1]
        ].PartyPowerWeight;

        if (dbChara.ExAbility2Level == 0)
            return power;

        // yes this is intentionally AbilityData
        return power
            + MasterAsset.AbilityData[
                charaData.ExAbility2[dbChara.ExAbility2Level - 1]
            ].PartyPowerWeight;
    }

    private static double GetUnionAbilityPower(IEnumerable<DbAbilityCrest> crests)
    {
        double totalPower = 0;

        foreach (
            (int unionId, int unionCrestCount) in crests
                .Select(x => MasterAsset.AbilityCrest[x.AbilityCrestId].UnionAbilityGroupId)
                .Where(x => x != 0)
                .ToLookup(x => x)
                .ToDictionary(x => x.Key, x => x.Count())
        )
        {
            UnionAbility ability = MasterAsset.UnionAbility[unionId];

            double maxPower = 0;

            foreach ((int count, int abilityId, int power) in ability.Abilities)
            {
                if (abilityId == 0)
                    break;

                if (unionCrestCount >= count)
                    maxPower = power;
            }

            totalPower += maxPower;
        }

        return totalPower;
    }

    private static (int AtkPlus, int HpPlus) GetStatusPlusParam(ref FortBonusList bonus)
    {
        return (bonus.all_bonus.attack, bonus.all_bonus.hp);
    }

    private static int CeilToInt(double value, int digits = 3)
    {
        return (int)Math.Round(value, digits, MidpointRounding.ToPositiveInfinity);
    }

    private static (int Atk, int Hp) GetAbilityCrest(
        AbilityCrests id,
        int buildup,
        int atkPlus,
        int hpPlus
    )
    {
        if (id == 0)
            return (0, 0);

        AbilityCrest crest = MasterAsset.AbilityCrest[id];
        AbilityCrestRarity rarity = MasterAsset.AbilityCrestRarity[crest.Rarity];

        if (buildup == 0)
            return (crest.BaseAtk + atkPlus, crest.BaseHp + hpPlus);

        int atkDiff = crest.MaxAtk - crest.BaseAtk;
        int hpDiff = crest.MaxHp - crest.BaseHp;

        double multiplier = buildup;
        if (buildup > rarity.MaxLimitLevelByLimitBreak4)
            multiplier = rarity.MaxLimitLevelByLimitBreak4;

        int atk =
            crest.BaseAtk
            + atkPlus
            + CeilToInt(atkDiff * multiplier / rarity.MaxLimitLevelByLimitBreak4);

        int hp =
            crest.BaseHp
            + hpPlus
            + CeilToInt(hpDiff * multiplier / rarity.MaxLimitLevelByLimitBreak4);

        return (atk, hp);
    }
}

file record BonusParams(double FortAtk, double FortHp, double AlbumAtk, double AlbumHp)
{
    public static BonusParams GetBonus(ref FortBonusList bonus, Charas charaId)
    {
        CharaData data = MasterAsset.CharaData[charaId];

        AtgenParamBonus paramBonus = bonus.param_bonus.First(x => x.weapon_type == data.WeaponType);
        AtgenElementBonus elementBonus = bonus.element_bonus.First(
            x => x.elemental_type == data.ElementalType
        );
        AtgenParamBonus paramByWeaponBonus = bonus.param_bonus_by_weapon.First(
            x => x.weapon_type == data.WeaponType
        );

        double atk = (paramBonus.attack + elementBonus.attack + paramByWeaponBonus.attack) / 100.0;
        double hp = (paramBonus.hp + elementBonus.hp + paramByWeaponBonus.hp) / 100.0;

        AtgenElementBonus albumBonus = bonus.chara_bonus_by_album.First(
            x => x.elemental_type == data.ElementalType
        );

        return new BonusParams(atk, hp, albumBonus.attack / 100.0, albumBonus.hp / 100.0);
    }

    public static BonusParams GetBonus(ref FortBonusList bonus, Dragons dragonId)
    {
        DragonData data = MasterAsset.DragonData[dragonId];

        AtgenDragonBonus dragonBonus = bonus.dragon_bonus.First(
            x => x.elemental_type == data.ElementalType
        );

        double atk = dragonBonus.attack / 100.0;
        double hp = dragonBonus.hp / 100.0;

        AtgenElementBonus albumBonus = bonus.dragon_bonus_by_album.First(
            x => x.elemental_type == data.ElementalType
        );

        return new BonusParams(atk, hp, albumBonus.attack / 100.0, albumBonus.hp / 100.0);
    }
};
