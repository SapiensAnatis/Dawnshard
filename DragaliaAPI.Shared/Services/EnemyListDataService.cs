using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragaliaAPI.Shared.Definitions;

namespace DragaliaAPI.Shared.Services;

public class EnemyListDataService : BaseDataService<DataEnemyList, string>, IEnemyListDataService
{
    private const string Filename = "enemy_lists.json";

    public EnemyListDataService() : base(Filename) { }

    public DataEnemyList GetData(DataQuestAreaInfo areaInfo)
    {
        string path = areaInfo.ScenePath.ToLower() + "/" + areaInfo.AreaName.ToLower();
        return GetData(path);
    }
}
