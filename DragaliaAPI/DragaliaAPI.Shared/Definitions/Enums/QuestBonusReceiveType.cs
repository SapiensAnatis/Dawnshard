namespace DragaliaAPI.Shared.Definitions.Enums;

public enum QuestBonusReceiveType
{
    /// <summary>
    /// Quest bonus type that is automatically claimed (e.g. Elemental Ruins).
    /// </summary>
    AutoReceive,

    /// <summary>
    /// Quest bonus type that permits deferring claims of a bonus (e.g. Agito), and instead prompts the player asking
    /// if they would like to claim it on the clear screen.
    /// </summary>
    /// <remarks>
    /// Confirming Yes on the clear screen initiates a separate call to /dungeon/receive_quest_bonus. The client will
    /// only prompt the user if the QuestData.QuestBonusReserveCount and QuestBonusReserveTime are set in the dungeon
    /// record response (I think).
    /// </remarks>
    SelectReceive,

    /// <summary>
    /// Unused quest bonus type.
    /// </summary>
    /// <remarks>
    /// SELECT COUNT(*) FROM QuestEvent WHERE _QuestBonusReceiveType = 2;
    /// => 0
    /// </remarks>
    StackReceive,

    /// <summary>
    /// Trials of the Mighty quest bonus type. Similar to SelectReceive in behaviour, but additionally allows an
    /// otherwise once per day bonus to be stacked up and received multiple times in one day.
    /// </summary>
    StackSelectReceive,

    /// <summary>
    /// Unused quest bonus type.
    /// </summary>
    /// <remarks>
    /// SELECT COUNT(*) FROM QuestEvent WHERE _QuestBonusReceiveType = 4;
    /// => 0
    /// </remarks>
    Max,
}
