using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions;
using DragaliaAPI.Shared.MasterAsset.Models;

namespace DragaliaAPI.Models;

public class DungeonSession
{
    public required IEnumerable<PartySettingList> Party { get; set; }

    public required QuestData QuestData { get; set; }

    public bool IsHost { get; set; } = true;

    public bool IsMulti { get; set; }

    public ulong? SupportViewerId { get; set; }

    public DateTimeOffset StartTime { get; set; }

    public Dictionary<int, IEnumerable<AtgenEnemy>> EnemyList { get; set; } = new();

    public int PlayCount { get; set; } = 1;
}
