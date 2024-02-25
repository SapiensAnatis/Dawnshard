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
        odds.Enemy = this.questEnemyService.BuildQuestEnemyList(questId, areaNum);
        odds.AreaIndex = areaNum;

        // this.logger.LogTrace("Generated enemy list: {@list}", odds.enemy);

        return odds;
    }

    // Mercurial Gauntlet
    public OddsInfo GetWallOddsInfo(int wallId, int wallLevel)
    {
        OddsInfo odds = StubData.OddsInfo;

        odds.Enemy = this.questEnemyService.BuildQuestWallEnemyList(wallId, wallLevel);
        odds.AreaIndex = 0;

        // this.logger.LogTrace("Generated enemy list: {@list}", odds.enemy);

        return odds;
    }

    private static class StubData
    {
        public static OddsInfo OddsInfo =>
            new()
            {
                AreaIndex = 0,
                ReactionObjCount = 1,
                DropObj = new List<AtgenDropObj>() { },
                Grade = new List<AtgenGrade>()
            };
    }
}
