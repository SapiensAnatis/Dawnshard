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
    private readonly IOddsInfoService oddsInfoService;

    public DungeonController(IDungeonService dungeonService, IOddsInfoService oddsInfoService)
    {
        this.dungeonService = dungeonService;
        this.oddsInfoService = oddsInfoService;
    }

    [HttpPost("get_area_odds")]
    public async Task<DragaliaResult> GetAreaOdds(DungeonGetAreaOddsRequest request)
    {
        DungeonSession session = await dungeonService.GetDungeon(request.dungeon_key);

        OddsInfo oddsInfo = this.oddsInfoService.GetOddsInfo(
            session.QuestData.Id,
            request.area_idx
        );

        await this.dungeonService.AddEnemies(request.dungeon_key, request.area_idx, oddsInfo.enemy);

        return Ok(new DungeonGetAreaOddsData() { odds_info = oddsInfo });
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
