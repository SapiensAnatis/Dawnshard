using System.Text.Json.Serialization;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Json;
using MemoryPack;

namespace DragaliaAPI.Shared.MasterAsset.Models.Dmode;

[MemoryPackable]
public partial record DmodeDungeonArea(
    int Id,
    int ThemeGroupId,
    VariationTypes VariationType,
    bool IsSelectedEntity,
    string Scene,
    string AreaName,
    string BossMultiSceneName
);
