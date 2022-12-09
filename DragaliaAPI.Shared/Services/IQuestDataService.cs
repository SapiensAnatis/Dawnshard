using DragaliaAPI.Shared.Definitions;

namespace DragaliaAPI.Shared.Services;

public interface IQuestDataService
{
    IEnumerable<DataQuestAreaInfo> GetMapData(int id);
}
