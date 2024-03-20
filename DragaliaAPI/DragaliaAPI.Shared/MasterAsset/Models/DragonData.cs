using DragaliaAPI.Shared.Definitions.Enums;
using MessagePack;

namespace DragaliaAPI.Shared.MasterAsset.Models;

public record DragonData(
    Dragons Id,
    int Rarity,
    UnitElement ElementalType,
    bool IsPlayable,
    int MinHp,
    int MaxHp,
    int AddMaxHp1,
    int MinAtk,
    int MaxAtk,
    int AddMaxAtk1,
    int Skill1,
    int Skill2,
    int Abilities11,
    int Abilities12,
    int Abilities13,
    int Abilities14,
    int Abilities15,
    int Abilities16,
    int Abilities21,
    int Abilities22,
    int Abilities23,
    int Abilities24,
    int Abilities25,
    int Abilities26,
    int DefaultReliabilityLevel,
    int DmodePassiveAbilityId,
    int MaxLimitBreakCount,
    Materials LimitBreakMaterialId,
    DragonLimitBreakTypes LimitBreakId,
    int FavoriteType,
    int SellCoin,
    int SellDewPoint,
    int BaseId,
    int VariationId
) : IUnitData
{
    [IgnoreMember]
    public readonly int[][] Abilities =
    {
        new[] { Abilities11, Abilities12, Abilities13, Abilities14, Abilities15, Abilities16 },
        new[] { Abilities21, Abilities22, Abilities23, Abilities24, Abilities25, Abilities26 }
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
};
