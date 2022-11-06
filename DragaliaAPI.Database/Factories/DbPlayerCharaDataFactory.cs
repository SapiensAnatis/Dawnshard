using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Database.Factories;

internal static class DbPlayerCharaDataFactory
{
    internal static DbPlayerCharaData Create(string deviceAccountId, DataAdventurer data)
    {
        byte rarity = (byte)data.Rarity;
        ushort rarityHp;
        ushort rarityAtk;

        switch (rarity)
        {
            case 3:
                rarityHp = (ushort)data.MinHp3;
                rarityAtk = (ushort)data.MinAtk3;
                break;
            case 4:
                rarityHp = (ushort)data.MinHp4;
                rarityAtk = (ushort)data.MinAtk4;
                break;
            case 5:
            default:
                rarityHp = (ushort)data.MinHp5;
                rarityAtk = (ushort)data.MinAtk5;
                break;
        }

        return new DbPlayerCharaData()
        {
            DeviceAccountId = deviceAccountId,
            CharaId = (Charas)data.IdLong,
            Rarity = rarity,
            Exp = 0,
            Level = 1,
            AdditionalMaxLevel = 0,
            HpPlusCount = 0,
            AttackPlusCount = 0,
            LimitBreakCount = 0,
            IsNew = true,
            Skill1Level = 1,
            Skill2Level = 1,
            Ability1Level = 1,
            Ability2Level = 1,
            Ability3Level = 1,
            BurstAttackLevel = 1,
            ComboBuildupCount = 0,
            Hp = rarityHp,
            Attack = rarityAtk,
            ExAbilityLevel = 1,
            ExAbility2Level = 1,
            IsTemporary = false,
            IsUnlockEditSkill = false,
            ManaCirclePieceIdList = new SortedSet<int>(),
            ListViewFlag = false,
            GetTime = DateTimeOffset.UtcNow
        };
    }
}
