using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Definitions;
using DragaliaAPI.Shared.Services;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("dungeon")]
public class DungeonController : DragaliaControllerBase
{
    private readonly IDungeonService dungeonService;
    private readonly IEnemyListDataService enemyListDataService;

    public DungeonController(
        IDungeonService dungeonService,
        IEnemyListDataService enemyListDataService
    )
    {
        this.dungeonService = dungeonService;
        this.enemyListDataService = enemyListDataService;
    }

    [HttpPost("get_area_odds")]
    public async Task<DragaliaResult> GetAreaOdds(DungeonGetAreaOddsRequest request)
    {
        DungeonSession session;

        try
        {
            session = await this.dungeonService.GetDungeon(request.dungeon_key);
        }
        catch (DungeonException)
        {
            return this.ResultCodeError(ResultCode.DUNGEON_AREA_NOT_FOUND);
        }

        List<int> enemyList = this.enemyListDataService
            // TODO: certain areas seem to be missing from the dict, use actual area_idx
            .GetData(session.AreaInfo.ElementAt(0))
            .Enemies;

        return this.Ok(
            new DungeonGetAreaOddsData()
            {
                odds_info = new()
                {
                    area_index = request.area_idx,
                    reaction_obj_count = 1,
                    drop_obj = new List<AtgenDropObj>(),
                    enemy = /* Enumerable
                        .Range(0, enemyList.Count())
                        .Select(
                            x =>
                                new AtgenEnemy()
                                {
                                    param_id = enemyList.ElementAt(x),
                                    enemy_idx = x+1,
                                    enemy_drop_list = new List<EnemyDropList>()
                                    {
                                        new()
                                        {
                                            coin = 10,
                                            mana = 10,
                                            drop_list = new List<AtgenDropList>()
                                        }
                                    }
                                }
                        ),*/
                        new List<AtgenEnemy>(),
                    grade = new List<AtgenGrade>(),
                },
            }
        );
    }
}
