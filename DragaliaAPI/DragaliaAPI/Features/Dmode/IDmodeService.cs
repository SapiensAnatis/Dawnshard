using DragaliaAPI.DTO;
using DragaliaAPI.Shared.Enums;

namespace DragaliaAPI.Features.Dmode;

public interface IDmodeService
{
    Task<DmodeInfo> GetInfo();
    Task<IEnumerable<DmodeCharaList>> GetCharaList();
    Task<DmodeDungeonInfo> GetDungeonInfo();
    Task<IEnumerable<DmodeServitorPassiveList>> GetServitorPassiveList();
    Task<DmodeExpedition> GetExpedition();

    Task UseRecovery();
    Task UseSkip();

    Task<IEnumerable<DmodeServitorPassiveList>> BuildupServitorPassive(
        IEnumerable<DmodeServitorPassiveList> buildupList
    );

    Task<DmodeExpedition> StartExpedition(int targetFloor, IEnumerable<Charas> charas);
    Task<(DmodeExpedition Expedition, DmodeIngameResult IngameResult)> FinishExpedition(
        bool forceFinish
    );
}
