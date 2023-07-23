using System.Text.Json.Serialization;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models.ManaCircle;

public record ManaNode(
    ManaNodeTypes ManaPieceType,
    bool IsReleaseStory,
    int NecessaryManaPoint,
    int UniqueGrowMaterialCount1,
    int UniqueGrowMaterialCount2,
    int GrowMaterialCount,
    [property: JsonPropertyName("MC_0")] int MC_0,
    string ManaCircleName,
    int Step
);
