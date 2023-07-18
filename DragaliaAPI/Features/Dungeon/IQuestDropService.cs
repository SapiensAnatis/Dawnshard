using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset.Models;

namespace DragaliaAPI.Features.Dungeon;

public interface IQuestDropService
{
    IEnumerable<Materials> GetDrops(int questId);
    public Task<IEnumerable<AtgenEventPassiveUpList>> GetEventPassiveDrops(QuestData quest);
}
