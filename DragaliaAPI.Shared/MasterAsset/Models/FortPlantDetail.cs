using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models;

/// <summary>
/// Model for FortPlantDetail masterasset table.
/// </summary>
/// <param name="Id">Unique identifier: building type ID concat level.</param>
/// <param name="AssetGroup">The building type ID.</param>
/// <param name="Level"></param>
/// <param name="LevelType">Type of level; 3 for Halidom, 2 for smithy; 1 for everything else.</param>
/// <param name="NeedLevel">Required Halidom level to construct this facility.</param>
/// <param name="Time">Time taken to construct this facility.</param>
/// <param name="Cost">Rupie cost to construct this facility.</param>
/// <param name="MaterialsId1"></param>
/// <param name="MaterialsNum1"></param>
/// <param name="MaterialsId2"></param>
/// <param name="MaterialsNum2"></param>
/// <param name="MaterialsId3"></param>
/// <param name="MaterialsNum3"></param>
/// <param name="MaterialsId4"></param>
/// <param name="MaterialsNum4"></param>
/// <param name="MaterialsId5"></param>
/// <param name="MaterialsNum5"></param>
/// <param name="EffectId">The 'target' of the effect, e.g. adventurers, dragons, weapons.</param>
/// <param name="EffType1">The parameter of the effect target, e.g. weapon type for weapons.</param>
/// <param name="EffType2">The parameter of the secondary effect target.</param>
/// <param name="EffArgs1">The magnitude of the HP effects in percentage points.</param>
/// <param name="EffArgs2">The magnitude of the strength effects in percentage points.</param>
/// <param name="EventEffectType">Event effect type; always 0 or 1.</param>
/// <param name="EventEffectArgs"></param>
/// <param name="MaterialMax">Max dragonfruit that can be claimed from the dragontree.</param>
/// <param name="MaterialMaxTime">Time taken to accrue MaterialMax.</param>
/// <param name="CostMax">Max rupies that can be claimed from rupie mines.</param>
/// <param name="CostMaxTime">Time taken to accrue CostMax.</param>
/// <param name="StaminaMax">Max stamina that can be claimed from the Halidom.</param>
/// <param name="StaminaMaxTime">Time taken to accrue StaminaMax.</param>
public record FortPlantDetail(
    int Id,
    FortPlants AssetGroup,
    int Level,
    int NextAssetGroup,
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
    FortEffectTypes EffectId,
    int EffType1,
    int EffType2,
    float EffArgs1,
    float EffArgs2,
    EventEffectTypes EventEffectType,
    float EventEffectArgs,
    int MaterialMax,
    int MaterialMaxTime,
    int CostMax,
    int CostMaxTime,
    int StaminaMax,
    int StaminaMaxTime,
    string Odds
)
{
    public Dictionary<Materials, int> CreateMaterialMap { get; } =
        new List<KeyValuePair<Materials, int>>()
        {
            new(MaterialsId1, MaterialsNum1),
            new(MaterialsId2, MaterialsNum2),
            new(MaterialsId3, MaterialsNum3),
            new(MaterialsId4, MaterialsNum4),
            new(MaterialsId5, MaterialsNum5),
        }
            .Where(x => x.Key != Materials.Empty)
            .ToDictionary(x => x.Key, x => x.Value);
}
