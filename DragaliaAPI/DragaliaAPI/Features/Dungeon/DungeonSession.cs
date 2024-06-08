using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset.Models;

namespace DragaliaAPI.Features.Dungeon;

public class DungeonSession
{
    public required IEnumerable<PartySettingList> Party { get; init; }

    public QuestData? QuestData { get; init; }

    public bool IsHost { get; set; } = true;

    public bool IsMulti { get; set; }

    public ulong? SupportViewerId { get; init; }

    public DateTimeOffset StartTime { get; init; }

    public Dictionary<int, IEnumerable<AtgenEnemy>> EnemyList { get; set; } = new();

    public int PlayCount { get; init; } = 1;

    public int WallId { get; init; }

    public int WallLevel { get; init; }

    public int QuestId => this.QuestData?.Id ?? 0;

    public int QuestGid => this.QuestData?.Gid ?? 0;

    public VariationTypes QuestVariation => this.QuestData?.VariationType ?? VariationTypes.None;
}
