using DragaliaAPI.Features.Dungeon.AutoRepeat;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums.Dungeon;

namespace DragaliaAPI.Features.Dungeon.Start;

public interface IAutoRepeatService
{
    Task SetRepeatSetting(RepeatSetting repeatSetting);
    Task<RepeatInfo?> GetRepeatInfo();
    Task<RepeatData?> RecordRepeat(
        string? repeatKey,
        IngameResultData ingameResultData,
        UpdateDataList updateDataList
    );
}
