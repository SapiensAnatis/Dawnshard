using DragaliaAPI.Shared.Definitions.Enums;

public record QuestTreasureData(
    int Id,
    int AddMaxDragonStorage,
    EntityTypes EntityType, // 4 - Rupies, 18 - Mana, 8 - Material, 17 - Summon Ticket, 2 - Item (Honey)
    int EntityId,
    int EntityQuantity
);
