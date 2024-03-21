using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset.Models.ManaCircle;

namespace DragaliaAPI.Shared.MasterAsset.Models;

public record CharaData(
    Charas Id,
    WeaponTypes WeaponType,
    int Rarity,
    int MaxLimitBreakCount,
    UnitElement ElementalType,
    bool IsPlayable,
    int MinHp3,
    int MinHp4,
    int MinHp5,
    int MaxHp,
    int AddMaxHp1,
    int PlusHp0,
    int PlusHp1,
    int PlusHp2,
    int PlusHp3,
    int PlusHp4,
    int PlusHp5,
    int McFullBonusHp5,
    int MinAtk3,
    int MinAtk4,
    int MinAtk5,
    int MaxAtk,
    int AddMaxAtk1,
    int PlusAtk0,
    int PlusAtk1,
    int PlusAtk2,
    int PlusAtk3,
    int PlusAtk4,
    int PlusAtk5,
    int McFullBonusAtk5,
    int Skill1,
    int Skill2,
    int HoldEditSkillCost,
    int EditSkillId,
    int EditSkillLevelNum,
    int EditSkillCost,
    string ManaCircleName,
    int CharaLimitBreak,
    int PieceElementGroupId,
    int PieceMaterialElementId,
    EntityTypes AwakeNeedEntityType4,
    int AwakeNeedEntityId4,
    int AwakeNeedEntityQuantity4,
    EntityTypes AwakeNeedEntityType5,
    int AwakeNeedEntityId5,
    int AwakeNeedEntityQuantity5,
    Materials UniqueGrowMaterialId1,
    Materials UniqueGrowMaterialId2,
    Materials GrowMaterialId,
    DateTimeOffset GrowMaterialOnlyStartDate,
    DateTimeOffset GrowMaterialOnlyEndDate,
    int DefaultAbility1Level,
    int DefaultAbility2Level,
    int DefaultAbility3Level,
    int DefaultBurstAttackLevel,
    int Abilities11,
    int Abilities12,
    int Abilities13,
    int Abilities14,
    int Abilities21,
    int Abilities22,
    int Abilities23,
    int Abilities24,
    int Abilities31,
    int Abilities32,
    int Abilities33,
    int Abilities34,
    int MinDef,
    int ExAbilityData1,
    int ExAbilityData2,
    int ExAbilityData3,
    int ExAbilityData4,
    int ExAbilityData5,
    int ExAbility2Data1,
    int ExAbility2Data2,
    int ExAbility2Data3,
    int ExAbility2Data4,
    int ExAbility2Data5,
    EntityTypes EditReleaseEntityType1,
    int EditReleaseEntityId1,
    int EditReleaseEntityQuantity1,
    int BaseId,
    int VariationId
) : IUnitData
{
    public bool HasManaSpiral => this.MaxLimitBreakCount > 4;

    public byte MaxLevel => (byte)(MaxLimitBreakCount * 20);

    public ushort MaxBaseHp => (ushort)(HasManaSpiral ? AddMaxHp1 : MaxHp);
    public ushort MaxBaseAtk => (ushort)(HasManaSpiral ? AddMaxAtk1 : MaxAtk);

    public ManaNodes MaxManaNodes => HasManaSpiral ? ManaNodes.Circle7 : ManaNodesUtil.MaxManaNodes;

    public int MaxAbility1Level =
        Abilities14 != 0
            ? 4
            : Abilities13 != 0
                ? 3
                : Abilities12 != 0
                    ? 2
                    : Abilities11 != 0
                        ? 1
                        : 0;

    public int MaxAbility2Level =
        Abilities24 != 0
            ? 4
            : Abilities23 != 0
                ? 3
                : Abilities22 != 0
                    ? 2
                    : Abilities21 != 0
                        ? 1
                        : 0;

    public int MaxAbility3Level =
        Abilities34 != 0
            ? 4
            : Abilities33 != 0
                ? 3
                : Abilities32 != 0
                    ? 2
                    : Abilities31 != 0
                        ? 1
                        : 0;

    public ManaNode GetManaNode(int num)
    {
        string MC_1 = this.ManaCircleName[3..];
        int MC_0 = int.Parse($"{MC_1}{num}");

        return MasterAsset.ManaNode.Get(MC_0);
    }

    public IEnumerable<ManaNode> GetManaNodes()
    {
        // There is a quirk in the DB where every mana circle has a node with index 0 and type 0, such that
        // characters always have 51/71 nodes instead of 50/70 as expected.
        // These are scattered through the data so it is easier to check in code than modify the auto-generated JSON.
        return MasterAsset.ManaNode.Enumerable.Where(x =>
            x.ManaCircleName == this.ManaCircleName && x.ManaPieceType != ManaNodeTypes.None
        );
    }

    public readonly int[] ExAbility =
    {
        ExAbilityData1,
        ExAbilityData2,
        ExAbilityData3,
        ExAbilityData4,
        ExAbilityData5
    };

    public readonly int[] ExAbility2 =
    {
        ExAbility2Data1,
        ExAbility2Data2,
        ExAbility2Data3,
        ExAbility2Data4,
        ExAbility2Data5
    };

    public readonly int[][] Abilities =
    {
        new[] { Abilities11, Abilities12, Abilities13, Abilities14 },
        new[] { Abilities21, Abilities22, Abilities23, Abilities24 },
        new[] { Abilities31, Abilities32, Abilities33, Abilities34 }
    };

    public int GetAbility(int abilityNo, int level)
    {
        int[] pool = Abilities[abilityNo - 1];

        int current = 0;

        for (int i = 0; i < level; i++)
        {
            int val = pool[i];

            if (val == 0)
                break;

            current = val;
        }

        return current;
    }
}
