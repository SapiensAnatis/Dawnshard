using DragaliaAPI.DTO;
using DragaliaAPI.Shared.Enums;
using DragaliaAPI.Shared.Enums.Dungeon;

namespace DragaliaAPI.Features.Dungeon.AutoRepeat;

public class RepeatInfo
{
    public Guid Key { get; init; }
    public RepeatSettingType Type { get; init; }
    public List<UseItem> UseItemList { get; init; } = [];
    public int MaxCount { get; init; }
    public int CurrentCount { get; set; }

    public IngameResultData? IngameResultData { get; set; }

    public UpdateDataList? UpdateDataList { get; set; }
}
