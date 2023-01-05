using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models;

public record FortPlant(
    int Id,
    int AssetGroup,
    int Level,
    int LevelType,
    int NeedLevel,
    int Time,
    int Cost,
    Materials MaterialsId1,
    int MaterialsNum1,
    Materials MaterialsId2,
    int MaterialsNum2,
    Materials MaterialsId3,
    int MaterialsNum3,
    Materials MaterialsId4,
    int MaterialsNum4,
    Materials MaterialsId5,
    int MaterialsNum5,
    int EffectId,
    int EffType1,
    int EffType2,
    double EffArgs1,
    double EffArgs2,
    int MaterialMaxTime,
    int MaterialMax,
    int StaminaMaxTime,
    int StaminaMax,
    int EventEffectType,
    double EventEffectArgs
);
