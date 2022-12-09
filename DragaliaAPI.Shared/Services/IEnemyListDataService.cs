using DragaliaAPI.Shared.Definitions;

namespace DragaliaAPI.Shared.Services;

public interface IEnemyListDataService : IBaseDataService<DataEnemyList, string>
{
    DataEnemyList GetData(DataQuestAreaInfo areaInfo);
}
