namespace DragaliaAPI.Shared.Definitions.Enums;

public enum EventKindType
{
    None,
    Raid,
    FieldQuest,
    Random,
    Build,
    Collect,
    Clb01,
    ExRush,
    ExHunter,
    Simple,
    Combat,
    BattleRoyal,
    Earn
}

public static class EventKindTypeExtensions
{
    public static EntityTypes ToItemType(this EventKindType type)
    {
        return type switch
        {
            EventKindType.Raid => EntityTypes.RaidEventItem,
            EventKindType.Random => EntityTypes.MazeEventItem,
            EventKindType.Build => EntityTypes.BuildEventItem,
            EventKindType.Collect => EntityTypes.CollectEventItem,
            EventKindType.Clb01 => EntityTypes.Clb01EventItem,
            EventKindType.ExRush => EntityTypes.ExRushEventItem,
            EventKindType.ExHunter => EntityTypes.ExHunterEventItem,
            EventKindType.Simple => EntityTypes.SimpleEventItem,
            EventKindType.Combat => EntityTypes.CombatEventItem,
            EventKindType.BattleRoyal => EntityTypes.BattleRoyalEventItem,
            EventKindType.Earn => EntityTypes.EarnEventItem,
            _ => EntityTypes.None
        };
    }
}
