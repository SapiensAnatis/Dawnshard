using DragaliaAPI.Shared.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models.Dmode;

public record DmodeDungeonArea(
    int Id,
    int ThemeGroupId,
    VariationTypes VariationType,
    bool IsSelectedEntity,
    string Scene,
    string AreaName,
    string BossMultiSceneName
);
