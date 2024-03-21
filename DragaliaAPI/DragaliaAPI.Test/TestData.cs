using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Test;

public static class TestData
{
    public static readonly UserSupportList SupportListEuden =
        new()
        {
            ViewerId = 1000,
            Name = "Euden",
            Level = 10,
            LastLoginDate = DateTimeOffset.UtcNow - TimeSpan.FromDays(1),
            EmblemId = (Emblems)40000002,
            MaxPartyPower = 9999,
            SupportChara = new()
            {
                CharaId = Charas.ThePrince,
                Level = 10,
                AdditionalMaxLevel = 0,
                Rarity = 5,
                Hp = 60,
                Attack = 40,
                HpPlusCount = 0,
                AttackPlusCount = 0,
                Ability1Level = 0,
                Ability2Level = 0,
                Ability3Level = 0,
                ExAbilityLevel = 1,
                ExAbility2Level = 1,
                Skill1Level = 1,
                Skill2Level = 0,
                IsUnlockEditSkill = true
            },
            SupportDragon = new() { DragonId = Dragons.Midgardsormr },
            SupportWeaponBody = new() { WeaponBodyId = WeaponBodies.SoldiersBrand },
            SupportTalisman = new() { TalismanId = Talismans.ThePrince },
            SupportCrestSlotType1List = new List<AtgenSupportCrestSlotType1List>()
            {
                new() { AbilityCrestId = AbilityCrests.TheGreatestGift },
                new() { AbilityCrestId = 0 },
                new() { AbilityCrestId = 0 },
            },
            SupportCrestSlotType2List = new List<AtgenSupportCrestSlotType1List>()
            {
                new() { AbilityCrestId = AbilityCrests.ManaFount },
                new() { AbilityCrestId = 0 },
            },
            SupportCrestSlotType3List = new List<AtgenSupportCrestSlotType1List>()
            {
                new() { AbilityCrestId = 0 },
                new() { AbilityCrestId = 0 },
            },
            Guild = new() { GuildId = 0, GuildName = "Guild" }
        };

    public static readonly UserSupportList SupportListElisanne =
        new()
        {
            ViewerId = 1001,
            Name = "Elisanne",
            Level = 10,
            LastLoginDate = DateTimeOffset.UtcNow - TimeSpan.FromDays(1),
            EmblemId = (Emblems)40000002,
            MaxPartyPower = 9999,
            SupportChara = new()
            {
                CharaId = Charas.Elisanne,
                Level = 10,
                AdditionalMaxLevel = 0,
                Rarity = 5,
                Hp = 60,
                Attack = 40,
                HpPlusCount = 0,
                AttackPlusCount = 0,
                Ability1Level = 0,
                Ability2Level = 0,
                Ability3Level = 0,
                ExAbilityLevel = 1,
                ExAbility2Level = 1,
                Skill1Level = 1,
                Skill2Level = 0,
                IsUnlockEditSkill = true
            },
            SupportDragon = new() { DragonKeyId = 0, },
            SupportWeaponBody = new() { WeaponBodyId = 0, },
            SupportTalisman = new() { TalismanKeyId = 0, },
            SupportCrestSlotType1List = new List<AtgenSupportCrestSlotType1List>()
            {
                new() { AbilityCrestId = 0 },
                new() { AbilityCrestId = 0 },
                new() { AbilityCrestId = 0 },
            },
            SupportCrestSlotType2List = new List<AtgenSupportCrestSlotType1List>()
            {
                new() { AbilityCrestId = 0 },
                new() { AbilityCrestId = 0 },
            },
            SupportCrestSlotType3List = new List<AtgenSupportCrestSlotType1List>()
            {
                new() { AbilityCrestId = 0 },
                new() { AbilityCrestId = 0 },
            },
            Guild = new() { GuildId = 0, GuildName = "Guild" }
        };
}
