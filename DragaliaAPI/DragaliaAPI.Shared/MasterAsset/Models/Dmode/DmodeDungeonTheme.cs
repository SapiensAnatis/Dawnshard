using System.Text.Json.Serialization;
using DragaliaAPI.Shared.Json;
using MemoryPack;

namespace DragaliaAPI.Shared.MasterAsset.Models.Dmode;

[MemoryPackable]
public partial record DmodeDungeonTheme(
    int Id,
    int ThemeGroupId,
    int PlusLevelMin,
    int PlusLevelMax,
    bool BossAppear
);
