using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models;

public record QuestDropInfo(int QuestId, IEnumerable<Materials> Material);
