using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;

namespace DragaliaAPI.Features.PartyPower;

public class PartyPowerService(
    IUnitRepository unitRepository,
    IAbilityCrestRepository abilityCrestRepository,
    IBonusService bonusService
) : IPartyPowerService
{
    public async Task<int> CalculatePartyPower(
        IEnumerable<PartySettingList> party,
        FortBonusList? bonusList = null
    )
    {
        int power = 0;

        bool isFirst = true;

        bonusList ??= await bonusService.GetBonusList();

        foreach (PartySettingList partySetting in party)
        {
            power += await CalculateCharacterPower(partySetting, isFirst, bonusList);
            isFirst = false;
        }

        return power;
    }

    public async Task<int> CalculatePartyPower(DbParty party, FortBonusList? bonusList = null)
    {
        int power = 0;

        bool isFirst = true;

        bonusList ??= await bonusService.GetBonusList();

        foreach (DbPartyUnit partyUnit in party.Units)
        {
            power += await CalculateCharacterPower(partyUnit, isFirst, bonusList);
            isFirst = false;
        }

        return power;
    }

    public async Task<int> CalculateCharacterPower(
        PartySettingList partySetting,
        bool shouldAddSkillBonus = true,
        FortBonusList? bonusList = null
    )
    {
        return await CalculateCharacterPower(
            partySetting.chara_id,
            (long)partySetting.equip_dragon_key_id,
            partySetting.equip_weapon_body_id,
            (long)partySetting.equip_talisman_key_id,
            partySetting.edit_skill_1_chara_id,
            partySetting.edit_skill_2_chara_id,
            partySetting.equip_crest_slot_type_1_crest_id_1,
            partySetting.equip_crest_slot_type_1_crest_id_2,
            partySetting.equip_crest_slot_type_1_crest_id_3,
            partySetting.equip_crest_slot_type_2_crest_id_1,
            partySetting.equip_crest_slot_type_2_crest_id_2,
            partySetting.equip_crest_slot_type_3_crest_id_1,
            partySetting.equip_crest_slot_type_3_crest_id_2,
            shouldAddSkillBonus,
            bonusList
        );
    }

    public async Task<int> CalculateCharacterPower(
        DbPartyUnit partyUnit,
        bool shouldAddSkillBonus = true,
        FortBonusList? bonusList = null
    )
    {
        return await CalculateCharacterPower(
            partyUnit.CharaId,
            partyUnit.EquipDragonKeyId,
            partyUnit.EquipWeaponBodyId,
            partyUnit.EquipTalismanKeyId,
            partyUnit.EditSkill1CharaId,
            partyUnit.EditSkill2CharaId,
            partyUnit.EquipCrestSlotType1CrestId1,
            partyUnit.EquipCrestSlotType1CrestId2,
            partyUnit.EquipCrestSlotType1CrestId3,
            partyUnit.EquipCrestSlotType2CrestId1,
            partyUnit.EquipCrestSlotType2CrestId2,
            partyUnit.EquipCrestSlotType3CrestId1,
            partyUnit.EquipCrestSlotType3CrestId2,
            shouldAddSkillBonus,
            bonusList
        );
    }

    private async Task<int> CalculateCharacterPower(
        Charas charaId,
        long dragonKeyId,
        WeaponBodies weaponBodyId,
        long talismanId,
        Charas editSkill1,
        Charas editSkill2,
        AbilityCrests crestType1No1,
        AbilityCrests crestType1No2,
        AbilityCrests crestType1No3,
        AbilityCrests crestType2No1,
        AbilityCrests crestType2No2,
        AbilityCrests crestType3No1,
        AbilityCrests crestType3No2,
        bool shouldAddSkillBonus = true,
        FortBonusList? bonus = null
    )
    {
        if (charaId == 0)
            return 0;

        bonus ??= await bonusService.GetBonusList();

        DbPlayerCharaData chara =
            await unitRepository.FindCharaAsync(charaId)
            ?? throw new DragaliaException(
                ResultCode.CommonDbError,
                "No chara found for power calculation"
            );

        CharaData charaData = MasterAsset.CharaData[charaId];

        DbPlayerDragonData? dragon = null;
        DbPlayerDragonReliability? reliability = null;
        DragonData? dragonData = null;
        DragonRarity? dragonRarity = null;

        if (dragonKeyId != 0)
        {
            dragon =
                await unitRepository.FindDragonAsync(dragonKeyId)
                ?? throw new DragaliaException(
                    ResultCode.CommonDbError,
                    "No dragon found for power calculation"
                );

            reliability =
                await unitRepository.FindDragonReliabilityAsync(dragon.DragonId)
                ?? throw new DragaliaException(
                    ResultCode.CommonDbError,
                    "No reliability found for power calculation"
                );

            dragonData = MasterAsset.DragonData[dragon.DragonId];
            dragonRarity = MasterAsset.DragonRarity[dragonData.Rarity];
        }

        DbWeaponBody? dbWeapon = null;
        WeaponBody? weaponBody = null;
        WeaponBodyRarity? weaponRarity = null;

        if (weaponBodyId != 0)
        {
            dbWeapon =
                await unitRepository.FindWeaponBodyAsync(weaponBodyId)
                ?? throw new DragaliaException(
                    ResultCode.CommonDbError,
                    "No weapon body found for power calculation"
                );

            weaponBody = MasterAsset.WeaponBody[dbWeapon.WeaponBodyId];
            weaponRarity = MasterAsset.WeaponBodyRarity[weaponBody.Rarity];
        }

        DbTalisman? talisman =
            talismanId == 0 ? null : await unitRepository.FindTalismanAsync(talismanId);

        AbilityCrests[] crests =
        {
            crestType1No1,
            crestType1No2,
            crestType1No3,
            crestType2No1,
            crestType2No2,
            crestType3No1,
            crestType3No2
        };

        HashSet<AbilityCrests> uniqueCrests = crests.Where(x => x != 0).ToHashSet();

        List<DbAbilityCrest> dbCrests = abilityCrestRepository.AbilityCrests
            .Where(x => uniqueCrests.Contains(x.AbilityCrestId))
            .ToList();

        double charaPowerParam = GetCharacterPower(
            ref chara,
            editSkill1,
            editSkill2,
            ref bonus,
            shouldAddSkillBonus
        );

        double dragonPowerParam = GetDragonPower(
            ref dragon,
            ref reliability,
            ref bonus,
            ref dragonData,
            ref dragonRarity,
            charaData.ElementalType
        );

        double crestPowerParam = GetCrestPower(dbCrests, talisman);

        double weaponPowerParam = GetWeaponPower(
            ref dbWeapon,
            ref weaponBody,
            ref weaponRarity,
            charaData.ElementalType
        );

        double exAbilityPowerParam = GetExAbilityPower(ref chara, ref charaData);

        double unionBonusPowerParam = GetUnionAbilityPower(dbCrests);

        double abilityPartyPowerParam = GetAbilityPartyPower(
            ref chara,
            ref charaData,
            ref dragon,
            ref dragonData,
            ref dbWeapon,
            ref weaponBody,
            dbCrests
        );

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

    private static int GetCharacterPower(
        ref DbPlayerCharaData dbChara,
        Charas editSkill1,
        Charas editSkill2,
        ref FortBonusList bonus,
        bool addSkillBonus
    )
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

        int skillPower = (dbChara.Skill1Level + dbChara.Skill2Level) * 100;
        int burstPower = dbChara.BurstAttackLevel * 60;
        int comboPower = dbChara.ComboBuildupCount * 200;

        int charaPowerParam = charaAtk + charaHp + skillPower + burstPower + comboPower;

        if (addSkillBonus)
        {
            if (editSkill1 != 0)
                charaPowerParam += 100;

            if (editSkill2 != 0)
                charaPowerParam += 100;
        }

        return charaPowerParam;
    }

    private static int GetCrestPower(IEnumerable<DbAbilityCrest> crests, DbTalisman? talisman)
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

        int crestPower = totalCrestHp + totalCrestAtk;

        return crestPower;
    }

    private static int GetDragonPower(
        ref DbPlayerDragonData? dbDragon,
        ref DbPlayerDragonReliability? reliability,
        ref FortBonusList bonus,
        ref DragonData? dragonData,
        ref DragonRarity? rarity,
        UnitElement charaElement
    )
    {
        if (dbDragon == null || reliability == null || dragonData == null || rarity == null)
            return 0;

        int maxLevel = rarity.Id == 5 ? rarity.LimitLevel04 : rarity.MaxLimitLevel;

        int currentLevel = Math.Min(dbDragon.Level, maxLevel);

        double levelMultiplier =
            currentLevel == 1 || maxLevel == 1 ? 0.0 : (maxLevel - 1.0) / (currentLevel - 1.0);

        int baseHp = CeilToInt(
            (levelMultiplier * (dragonData.MaxHp - dragonData.MinHp)) + dragonData.MinHp
        );
        int baseAtk = CeilToInt(
            (levelMultiplier * (dragonData.MaxAtk - dragonData.MinAtk)) + dragonData.MinAtk
        );

        if (dragonData.MaxLimitBreakCount == 5 && dbDragon.Level > rarity.LimitLevel04)
        {
            int limitBreak5Level =
                Math.Min(dbDragon.Level, rarity.LimitLevel05) - rarity.LimitLevel04;

            double limitBreak5LevelCount = rarity.LimitLevel05 - rarity.LimitLevel04;

            double limitBreak5Multiplier = limitBreak5Level / limitBreak5LevelCount;

            baseAtk += CeilToInt(
                (dragonData.AddMaxAtk1 - dragonData.MaxAtk) * limitBreak5Multiplier
            );

            baseHp += CeilToInt((dragonData.AddMaxHp1 - dragonData.MaxHp) * limitBreak5Multiplier);
        }

        double multiplier = dragonData.ElementalType == charaElement ? 1.5 : 1;

        int normalAtk = CeilToInt((baseAtk + dbDragon.AttackPlusCount) * multiplier);
        int normalHp = CeilToInt((baseHp + dbDragon.HpPlusCount) * multiplier);

        // set totalUnitAtk + totalUnitHp

        BonusParams bonusParams = BonusParams.GetBonus(ref bonus, dbDragon.DragonId);

        int fortAtk = CeilToInt(normalAtk * bonusParams.FortAtk);
        int fortHp = CeilToInt(normalHp * bonusParams.FortHp);

        int albumAtk = CeilToInt(normalAtk * bonusParams.AlbumAtk);
        int albumHp = CeilToInt(normalHp * bonusParams.AlbumHp);

        int dragonAtk = normalAtk + fortAtk + albumAtk;
        int dragonHp = normalHp + fortHp + albumHp;

        int dragonPowerParam =
            dragonAtk
            + dragonHp
            + (dbDragon.Skill1Level * 50)
            + (reliability.Level * 10)
            + rarity.RarityBasePartyPower
            + (rarity.LimitBreakCountPartyPowerWeight * dbDragon.LimitBreakCount);

        return dragonPowerParam;
    }

    private static int GetWeaponPower(
        ref DbWeaponBody? dbWeapon,
        ref WeaponBody? weaponBody,
        ref WeaponBodyRarity? weaponRarity,
        UnitElement charaElement
    )
    {
        if (dbWeapon == null || weaponBody == null || weaponRarity == null)
            return 0;

        int weaponBodyHp = 0;
        int weaponBodyAtk = 0;

        if (
            weaponRarity.MaxLimitLevelByLimitBreak4 != 0
            && dbWeapon.BuildupCount <= weaponRarity.MaxLimitLevelByLimitBreak4
        )
        {
            weaponBodyHp = CeilToInt(
                (double)dbWeapon.BuildupCount
                    / weaponRarity.MaxLimitLevelByLimitBreak4
                    * (weaponBody.MaxHp1 - weaponBody.BaseHp)
                    + weaponBody.BaseHp
            );
            weaponBodyAtk = CeilToInt(
                (double)dbWeapon.BuildupCount
                    / weaponRarity.MaxLimitLevelByLimitBreak4
                    * (weaponBody.MaxAtk1 - weaponBody.BaseAtk)
                    + weaponBody.BaseAtk
            );
        }
        else if (
            weaponRarity.MaxLimitLevelByLimitBreak4 < dbWeapon.BuildupCount
            && dbWeapon.BuildupCount <= weaponRarity.MaxLimitLevelByLimitBreak8
        )
        {
            weaponBodyHp = CeilToInt(
                (double)(dbWeapon.BuildupCount - weaponRarity.MaxLimitLevelByLimitBreak4)
                    / (
                        weaponRarity.MaxLimitLevelByLimitBreak8
                        - weaponRarity.MaxLimitLevelByLimitBreak4
                    )
                    * (weaponBody.MaxHp2 - weaponBody.MaxHp1)
                    + weaponBody.MaxHp1
            );
            weaponBodyAtk = CeilToInt(
                (double)(dbWeapon.BuildupCount - weaponRarity.MaxLimitLevelByLimitBreak4)
                    / (
                        weaponRarity.MaxLimitLevelByLimitBreak8
                        - weaponRarity.MaxLimitLevelByLimitBreak4
                    )
                    * (weaponBody.MaxAtk2 - weaponBody.MaxAtk1)
                    + weaponBody.MaxAtk1
            );
        }
        else if (
            weaponRarity.MaxLimitLevelByLimitBreak8 < dbWeapon.BuildupCount
            && dbWeapon.BuildupCount <= weaponRarity.MaxLimitLevelByLimitBreak9
        )
        {
            weaponBodyHp = CeilToInt(
                (double)(dbWeapon.BuildupCount - weaponRarity.MaxLimitLevelByLimitBreak8)
                    / (
                        weaponRarity.MaxLimitLevelByLimitBreak9
                        - weaponRarity.MaxLimitLevelByLimitBreak8
                    )
                    * (weaponBody.MaxHp3 - weaponBody.MaxHp2)
                    + weaponBody.MaxHp2
            );
            weaponBodyAtk = CeilToInt(
                (double)(dbWeapon.BuildupCount - weaponRarity.MaxLimitLevelByLimitBreak8)
                    / (
                        weaponRarity.MaxLimitLevelByLimitBreak9
                        - weaponRarity.MaxLimitLevelByLimitBreak8
                    )
                    * (weaponBody.MaxAtk3 - weaponBody.MaxAtk2)
                    + weaponBody.MaxAtk2
            );
        }

        double multiplier = weaponBody.ElementalType == charaElement ? 1.5 : 1;
        int weaponPower = CeilToInt((weaponBodyHp + weaponBodyAtk) * multiplier);

        int weaponSkillPower = dbWeapon.SkillNo == 0 ? 0 : dbWeapon.SkillLevel * 50;

        int weaponBodyPowerParam = weaponPower + weaponSkillPower;

        if (dbWeapon.LimitOverCount >= 1)
            weaponBodyPowerParam += weaponBody.LimitOverCountPartyPower1;

        // funnily enough the only weapons that can reach limit over count 2, agito weapons, don't have stat boosts for it
        if (dbWeapon.LimitOverCount >= 2)
            weaponBodyPowerParam += weaponBody.LimitOverCountPartyPower2;

        return weaponBodyPowerParam;
    }

    private static int GetExAbilityPower(ref DbPlayerCharaData dbChara, ref CharaData charaData)
    {
        if (dbChara.ExAbilityLevel == 0)
            return 0;

        int power = MasterAsset.ExAbilityData[
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

    private static int GetUnionAbilityPower(IEnumerable<DbAbilityCrest> crests)
    {
        int totalPower = 0;

        foreach (
            (int unionId, int unionCrestCount) in crests
                .Select(x => MasterAsset.AbilityCrest[x.AbilityCrestId].UnionAbilityGroupId)
                .Where(x => x != 0)
                .ToLookup(x => x)
                .ToDictionary(x => x.Key, x => x.Count())
        )
        {
            UnionAbility ability = MasterAsset.UnionAbility[unionId];

            int maxPower = 0;

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

    private static int GetAbilityPartyPower(
        ref DbPlayerCharaData dbChara,
        ref CharaData charaData,
        ref DbPlayerDragonData? dbDragon,
        ref DragonData? dragonData,
        ref DbWeaponBody? dbWeapon,
        ref WeaponBody? weaponData,
        IEnumerable<DbAbilityCrest> crests
    )
    {
        List<int> abilityIdList = new();

        int[] abilityIds =
        {
            charaData.GetAbility(1, dbChara.Ability1Level),
            charaData.GetAbility(2, dbChara.Ability2Level),
            charaData.GetAbility(3, dbChara.Ability3Level),
            dbDragon == null || dragonData == null
                ? 0
                : dragonData.GetAbility(1, dbDragon.Ability1Level),
            dbDragon == null || dragonData == null
                ? 0
                : dragonData.GetAbility(2, dbDragon.Ability2Level),
            dbWeapon == null || weaponData == null
                ? 0
                : weaponData.GetAbility(1, dbWeapon.Ability1Level),
            dbWeapon == null || weaponData == null
                ? 0
                : weaponData.GetAbility(2, dbWeapon.Ability2Level)
        };

        abilityIdList.AddRange(abilityIds);
        abilityIdList.AddRange(
            crests.SelectMany(
                x => MasterAsset.AbilityCrest[x.AbilityCrestId].GetAbilities(x.AbilityLevel)
            )
        );

        int power = abilityIdList
            .Where(x => x != 0)
            .Select(x => MasterAsset.AbilityData[x].PartyPowerWeight)
            .Sum();

        return power;
    }

    private static (int AtkPlus, int HpPlus) GetStatusPlusParam(ref FortBonusList bonus)
    {
        return (bonus.all_bonus.attack, bonus.all_bonus.hp);
    }

    private static int CeilToInt(double value)
    {
        return (int)Math.Ceiling(value);
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
