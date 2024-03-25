using System.Text.Json.Serialization;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Json;

namespace DragaliaAPI.Shared.MasterAsset.Models.Dmode;

using MemoryPack;

[MemoryPackable]
public record DmodeDungeonArea(
    int Id,
    int ThemeGroupId,
    VariationTypes VariationType,
    bool IsSelectedEntity,
    string Scene,
    string AreaName,
    string BossMultiSceneName
);
