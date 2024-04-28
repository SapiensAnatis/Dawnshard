using DragaliaAPI.DTO;
using DragaliaAPI.Shared.Enums;

namespace DragaliaAPI.Features.DmodeDungeon;

public interface IDmodeDungeonService
{
    Task<(DungeonState State, DmodeIngameData IngameData)> StartDungeon(
        Charas chara,
        int startFloor,
        int servitorId,
        IList<Charas> editSkillCharaIds
    );

    Task<(DungeonState State, DmodeIngameData IngameData)> RestartDungeon();
    Task<DungeonState> SkipFloor();
    Task<DungeonState> HaltDungeon(bool userHalt);

    Task<(DungeonState State, DmodeFloorData FloorData)> ProgressToNextFloor(
        DmodePlayRecord? playRecord
    );

    Task<(DungeonState State, DmodeIngameResult IngameResult)> FinishDungeon(bool isGameOver);
}
