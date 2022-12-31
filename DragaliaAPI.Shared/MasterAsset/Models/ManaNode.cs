using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models;

public record ManaNode(
    ManaNodeTypes ManaPieceType,
    int IsReleaseStory,
    int NecessaryManaPoint,
    int UniqueGrowMaterialCount1,
    int UniqueGrowMaterialCount2,
    int GrowMaterialCount,
    [property: JsonPropertyName("MC_0")] int MC_0,
    string ManaCircleName
)
{
    public bool IsReleaseStoryBool => this.IsReleaseStory == 1;
};
