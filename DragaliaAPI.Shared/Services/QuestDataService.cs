using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragaliaAPI.Shared.Definitions;

namespace DragaliaAPI.Shared.Services;

/// <summary>
/// Service to get data about quests.
///</summary>
/* JSON generated from the following sqlite query:
SELECT
    _Id,
    _DungeonType,
    _QuestPlayModeType,
    _Scene01, _AreaName01,
    _Scene02, _AreaName02,
    _Scene03, _AreaName03,
    _Scene04, _AreaName04,
    _Scene05, _AreaName05,
    _Scene06, _AreaName06
FROM QuestData</remarks>
*/
public class QuestDataService : BaseDataService<DataQuest, int>, IQuestDataService
{
    private const string Filename = "quests.json";

    public QuestDataService() : base(Filename) { }
}
