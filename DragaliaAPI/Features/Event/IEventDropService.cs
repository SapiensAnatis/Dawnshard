using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.MasterAsset.Models;

namespace DragaliaAPI.Features.Event;

public interface IEventDropService
{
    Task<IEnumerable<AtgenEventPassiveUpList>> ProcessEventPassiveDrops(QuestData quest);
    Task<IEnumerable<AtgenDropAll>> ProcessEventMaterialDrops(
        QuestData quest,
        PlayRecord record,
        double buildDropMultiplier
    );
}
