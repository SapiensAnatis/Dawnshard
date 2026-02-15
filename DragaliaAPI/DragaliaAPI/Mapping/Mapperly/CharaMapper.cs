using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using Riok.Mapperly.Abstractions;

namespace DragaliaAPI.Mapping.Mapperly;

[Mapper]
public static partial class CharaMapper
{
    [MapperRequiredMapping(RequiredMappingStrategy.Target)]
    [MapperIgnoreTarget(nameof(CharaList.StatusPlusCount))]
    public static partial CharaList ToCharaList(this DbPlayerCharaData dbModel);

    public static DbPlayerCharaData ToDbPlayerChara(this CharaList charaList, long viewerId)
    {
        CharaData masterAssetEntry = MasterAsset.CharaData[charaList.CharaId];

        int attackBase = CharaUtils.CalculateBaseAttack(
            masterAssetEntry,
            charaList.Level,
            charaList.Rarity
        );

        int attackNode = charaList.Attack - attackBase;

        int hpBase = CharaUtils.CalculateBaseHp(
            masterAssetEntry,
            charaList.Level,
            charaList.Rarity
        );

        int hpNode = charaList.Hp - hpBase;

#pragma warning disable CS0618 // Type or member is obsolete: This is a mapping method so we want control over every field.
        return new DbPlayerCharaData()
#pragma warning restore CS0618 // Type or member is obsolete
        {
            ViewerId = viewerId,
            CharaId = charaList.CharaId,
            Rarity = (byte)charaList.Rarity,
            Exp = charaList.Exp,
            Level = (byte)charaList.Level,
            HpPlusCount = (byte)charaList.HpPlusCount,
            AttackPlusCount = (byte)charaList.AttackPlusCount,
            LimitBreakCount = (byte)charaList.LimitBreakCount,
            IsNew = charaList.IsNew,
            Skill1Level = (byte)charaList.Skill1Level,
            Skill2Level = (byte)charaList.Skill2Level,
            Ability1Level = (byte)charaList.Ability1Level,
            Ability2Level = (byte)charaList.Ability2Level,
            Ability3Level = (byte)charaList.Ability3Level,
            BurstAttackLevel = (byte)charaList.BurstAttackLevel,
            ComboBuildupCount = charaList.ComboBuildupCount,
            HpBase = (ushort)hpBase,
            HpNode = (ushort)hpNode,
            AttackBase = (ushort)attackBase,
            AttackNode = (ushort)attackNode,
            ExAbilityLevel = (byte)charaList.ExAbilityLevel,
            ExAbility2Level = (byte)charaList.ExAbility2Level,
            IsTemporary = false, // Ignore characters imported with is_temporary = true; this renders them permanently inaccessible
            IsUnlockEditSkill = charaList.IsUnlockEditSkill,
            ListViewFlag = charaList.ListViewFlag,
            GetTime = charaList.GetTime,
            ManaCirclePieceIdList = new SortedSet<int>(charaList.ManaCirclePieceIdList),
        };
    }
}
