using DragaliaAPI.Controllers;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Definitions;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Dungeon;

[Route("dungeon")]
public class DungeonController : DragaliaControllerBase
{
    private readonly IDungeonService dungeonService;

    public DungeonController(IDungeonService dungeonService)
    {
        this.dungeonService = dungeonService;
    }

    [HttpPost("get_area_odds")]
    public async Task<DragaliaResult> GetAreaOdds(DungeonGetAreaOddsRequest request)
    {
        DungeonSession session = await dungeonService.GetDungeon(request.dungeon_key);

        return Ok(
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

    [HttpPost("fail")]
    public async Task<DragaliaResult> Fail(DungeonFailRequest request)
    {
        DungeonSession session = await dungeonService.FinishDungeon(request.dungeon_key);

        return Ok(
            new DungeonFailData()
            {
                result = 1,
                fail_helper_list = new List<UserSupportList>(),
                fail_helper_detail_list = new List<AtgenHelperDetailList>(),
                fail_quest_detail = new()
                {
                    quest_id = session.QuestData.Id,
                    wall_id = 0,
                    wall_level = 0,
                    is_host = true,
                }
            }
        );
    }
}
