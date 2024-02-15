using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset.Models;

namespace DragaliaAPI.Models;

public class DungeonSession
{
    public required IEnumerable<PartySettingList> Party { get; set; }

    public QuestData? QuestData { get; set; }

    public int QuestId => QuestData?.Id ?? 0;

    public int QuestGid => QuestData?.Gid ?? 0;

    public VariationTypes QuestVariation => QuestData?.VariationType ?? VariationTypes.Normal;

    public bool IsHost { get; set; } = true;

    public bool IsMulti { get; set; }

    public ulong? SupportViewerId { get; set; }

    public DateTimeOffset StartTime { get; set; }

    public Dictionary<int, IEnumerable<AtgenEnemy>> EnemyList { get; set; } = new();

    public int PlayCount { get; set; } = 1;

    public int WallId { get; set; }

    public int WallLevel { get; set; }
}
