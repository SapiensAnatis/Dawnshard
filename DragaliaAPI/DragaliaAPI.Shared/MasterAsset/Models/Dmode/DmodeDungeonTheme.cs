using System.Text.Json.Serialization;
using DragaliaAPI.Shared.Json;

namespace DragaliaAPI.Shared.MasterAsset.Models.Dmode;

public record DmodeDungeonTheme(
    int Id,
    int ThemeGroupId,
    int PlusLevelMin,
    int PlusLevelMax,
    bool BossAppear
);
