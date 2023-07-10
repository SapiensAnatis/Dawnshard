using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.Dungeon;

public class OddsInfoService : IOddsInfoService
{
    private readonly IQuestEnemyService questEnemyService;
    private readonly ILogger<OddsInfoService> logger;

    public OddsInfoService(IQuestEnemyService questEnemyService, ILogger<OddsInfoService> logger)
    {
        this.questEnemyService = questEnemyService;
        this.logger = logger;
    }

    public OddsInfo GetOddsInfo(int questId, int areaNum)
    {
        OddsInfo odds = StubData.OddsInfo;

        // TODO: drop_obj (treasure chests / crates, I think?)
        odds.enemy = this.questEnemyService.BuildQuestEnemyList(questId, areaNum);
        odds.area_index = areaNum;

        // this.logger.LogTrace("Generated enemy list: {@list}", odds.enemy);

        return odds;
    }

    private static class StubData
    {
        public static OddsInfo OddsInfo =>
            new()
            {
                area_index = 0,
                reaction_obj_count = 1,
                drop_obj = new List<AtgenDropObj>() { },
                grade = new List<AtgenGrade>()
            };
    }
}
