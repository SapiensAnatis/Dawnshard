using DragaliaAPI.Features.Shared.Models.Generated;

namespace DragaliaAPI.Features.Dungeon;

public record struct QuestMissionStatus(
    bool[] Missions,
    IEnumerable<AtgenMissionsClearSet> MissionsClearSet,
    IEnumerable<AtgenFirstClearSet> MissionCompleteSet
);
