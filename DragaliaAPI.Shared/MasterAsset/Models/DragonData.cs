using System.Text.Json.Serialization;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Json;

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
    int SellDewPoint
);
