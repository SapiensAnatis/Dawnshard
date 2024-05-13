using DragaliaAPI.Shared.Definitions.Enums.Summon;

namespace DragaliaAPI.Shared.MasterAsset.Models.Summon;

/// <summary>
/// MasterAsset data type for SummonData.json, representing a summoning banner.
/// </summary>
/// <param name="Id">The ID of the banner.</param>
/// <param name="SummonType">The summoning type of the banner.</param>
/// <remarks>
/// Be careful adding fields to this type; Dawnshard uses a modded SummonData.json to run its banners.
/// In particular, SummonPointId, CommenceDate, CompleteDate, and Priority are liable to be updated and
/// out of sync with the JSON file.
/// </remarks>
public record SummonData(int Id, SummonTypes SummonType);
