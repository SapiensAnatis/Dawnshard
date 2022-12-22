using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions;

namespace DragaliaAPI.Models;

public class DungeonSession
{
    public required int DungeonId { get; set; }

    public required IEnumerable<PartySettingList> Party { get; set; }

    public required IEnumerable<DataQuestAreaInfo> AreaInfo { get; set; }
}
