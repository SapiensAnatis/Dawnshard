using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models;

public record QuestTreasureData(
    int Id, // 126201 - 1 26 (chapter) 2 (difficulty: 1 - normal, 2 - hard, 3 - very hard) 01 (chest)
    int AddMaxDragonStorage,
    EntityTypes EntityType, // 4 - Rupies, 18 - Mana, 8 - Material, 17 - Summon Ticket, 2 - Item (Honey)
    int EntityId,
    int EntityQuantity
);
