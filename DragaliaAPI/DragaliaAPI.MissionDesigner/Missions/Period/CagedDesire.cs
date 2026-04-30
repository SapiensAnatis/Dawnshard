using DragaliaAPI.MissionDesigner.Models;
using DragaliaAPI.MissionDesigner.Models.Attributes;
using DragaliaAPI.MissionDesigner.Models.EventMission;
using DragaliaAPI.MissionDesigner.Models.RegularMission;

namespace DragaliaAPI.MissionDesigner.Missions.Period;

[ContainsMissionList]
public static class CagedDesire
{
    private const int EventId = 20429;

    [MissionType(MissionType.Period)]
    [EventId(EventId)]
    public static List<Mission> Missions { get; } =
    [
        // Participate in the Event
        new EventParticipationMission() { MissionId = 11330101 },
        // Unlock 10 of Sophie's Mana Nodes
        new CharacterManaNodeUnlockMission() { MissionId = 11330301, CharaId = Charas.Sophie },
        // Unlock 20 of Sophie's Mana Nodes
        new CharacterManaNodeUnlockMission() { MissionId = 11330302, CharaId = Charas.Sophie },
        // Unlock 30 of Sophie's Mana Nodes
        new CharacterManaNodeUnlockMission() { MissionId = 11330303, CharaId = Charas.Sophie },
        // Unlock 40 of Sophie's Mana Nodes
        new CharacterManaNodeUnlockMission() { MissionId = 11330304, CharaId = Charas.Sophie },
        // Unlock 50 of Sophie's Mana Nodes
        new CharacterManaNodeUnlockMission() { MissionId = 11330305, CharaId = Charas.Sophie },
        // Clear Warden Assault
        new EventRegularBattleClearMission() { MissionId = 11330401 },
        // Clear Warden Assault Five Times
        new EventRegularBattleClearMission() { MissionId = 11330402 },
        // Clear Warden Assault 10 Times
        new EventRegularBattleClearMission() { MissionId = 11330403 },
        // Clear Warden Assault 15 Times
        new EventRegularBattleClearMission() { MissionId = 11330404 },
        // Clear Warden Assault 20 Times
        new EventRegularBattleClearMission() { MissionId = 11330405 },
        // Clear Warden Assault 25 Times
        new EventRegularBattleClearMission() { MissionId = 11330406 },
        // Clear Monarch Emile Clash
        new EventChallengeBattleClearMission() { MissionId = 11330501 },
        // Clear Monarch Emile Clash Five Times
        new EventChallengeBattleClearMission() { MissionId = 11330502 },
        // Clear Monarch Emile Clash 10 Times
        new EventChallengeBattleClearMission() { MissionId = 11330503 },
        // Clear Monarch Emile Clash 15 Times
        new EventChallengeBattleClearMission() { MissionId = 11330504 },
        // Clear Monarch Emile Clash 20 Times
        new EventChallengeBattleClearMission() { MissionId = 11330505 },
        // Clear Monarch Emile Clash 25 Times
        new EventChallengeBattleClearMission() { MissionId = 11330506 },
        // Clear Monarch Emile Clash 30 Times
        new EventChallengeBattleClearMission() { MissionId = 11330507 },
        // Clear Monarch Emile Clash 35 Times
        new EventChallengeBattleClearMission() { MissionId = 11330508 },
        // Clear Monarch Emile Clash 40 Times
        new EventChallengeBattleClearMission() { MissionId = 11330509 },
        // Clear Monarch Emile Clash 45 Times
        new EventChallengeBattleClearMission() { MissionId = 11330510 },
        // Clear Monarch Emile Clash 50 Times
        new EventChallengeBattleClearMission() { MissionId = 11330511 },
        // Clear Monarch Emile Clash: Nightmare
        new EventChallengeBattleClearMission()
        {
            MissionId = 11330601,
            VariationType = VariationTypes.Hell,
        },
        // Clear Joker's Trial: Standard (Solo)
        new ClearQuestMission() { MissionId = 11330701, QuestId = 204290809 },
        // Clear Joker's Trial: Expert (Solo)
        new ClearQuestMission() { MissionId = 11330801, QuestId = 204290801 },
        // Clear Joker's Trial: Expert (Co-op)
        new ClearQuestMission() { MissionId = 11330901, QuestId = 204290805 },
        // Clear Joker's Trial: Master (Solo)
        new ClearQuestMission() { MissionId = 11331001, QuestId = 204290802 },
        // Clear Joker's Trial: Master (Co-op)
        new ClearQuestMission() { MissionId = 11331101, QuestId = 204290806 },
        // Clear Sophie's Trial: Standard (Solo)
        new ClearQuestMission() { MissionId = 11331201, QuestId = 204290810 },
        // Clear Sophie's Trial: Expert (Solo)
        new ClearQuestMission() { MissionId = 11331301, QuestId = 204290803 },
        // Clear Sophie's Trial: Expert (Co-op)
        new ClearQuestMission() { MissionId = 11331401, QuestId = 204290807 },
        // Clear Sophie's Trial: Master (Solo)
        new ClearQuestMission() { MissionId = 11331501, QuestId = 204290804 },
        // Clear Sophie's Trial: Master (Co-op)
        new ClearQuestMission() { MissionId = 11331601, QuestId = 204290808 },
        // Clear Monarch Emile Clash: Omega Level 1 (Raid)
        new ClearQuestMission() { MissionId = 11331701, QuestId = 204290602 },
        // Clear Monarch Emile Clash: Omega Level 2 (Raid)
        new ClearQuestMission() { MissionId = 11331801, QuestId = 204290604 },
        // Clear Monarch Emile Clash: Omega Level 3 (Raid)
        new ClearQuestMission() { MissionId = 11331901, QuestId = 204290606 },
        // Clear Monarch Emile Clash: Omega Level 1 (Solo)
        new ClearQuestMission() { MissionId = 11332001, QuestId = 204290601 },
        // Clear Monarch Emile Clash: Omega Level 2 (Solo)
        new ClearQuestMission() { MissionId = 11332101, QuestId = 204290603 },
        // Clear Monarch Emile Clash: Omega Level 3 (Solo)
        new ClearQuestMission() { MissionId = 11332201, QuestId = 204290605 },
    ];
}
