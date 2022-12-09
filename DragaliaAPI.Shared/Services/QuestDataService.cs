using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragaliaAPI.Shared.Definitions;

namespace DragaliaAPI.Shared.Services;

/// <summary>
/// Service to get data about quests.
/// <remarks>JSON generated from the following sqlite query:
/// SELECT _Id, _Scene01, _AreaName01, _Scene02, _AreaName02, _Scene03, _AreaName03, _Scene04, _AreaName04, _Scene05, _AreaName05, _Scene06, _AreaName06 FROM QuestData</remarks>
/// </summary>
public class QuestDataService : BaseDataService<DataQuest, int>, IQuestDataService
{
    private const string Filename = "quests.json";

    public QuestDataService() : base(Filename) { }

    public IEnumerable<DataQuestAreaInfo> GetMapData(int id)
    {
        DataQuest quest = this.GetData(id);

        // May be able to do this better using reflection
        List<DataQuestAreaInfo> result =
            new()
            {
                new(quest.Scene1, quest.AreaName1),
                new(quest.Scene2, quest.AreaName2),
                new(quest.Scene3, quest.AreaName3),
                new(quest.Scene4, quest.AreaName4),
                new(quest.Scene5, quest.AreaName5),
                new(quest.Scene6, quest.AreaName6),
            };

        return result.Where(
            x => !string.IsNullOrEmpty(x.ScenePath) && !string.IsNullOrEmpty(x.AreaName)
        );
    }
}
