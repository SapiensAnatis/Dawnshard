﻿using DragaliaAPI.Controllers;
using DragaliaAPI.Features.Dungeon.Record;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Services.Photon;
using DragaliaAPI.Shared.Definitions;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Dungeon;

[Route("dungeon")]
public class DungeonController(
    IDungeonService dungeonService,
    IOddsInfoService oddsInfoService,
    IMatchingService matchingService,
    IDungeonRecordHelperService dungeonRecordHelperService
) : DragaliaControllerBase
{
    [HttpPost("get_area_odds")]
    public async Task<DragaliaResult> GetAreaOdds(DungeonGetAreaOddsRequest request)
    {
        DungeonSession session = await dungeonService.GetDungeon(request.dungeon_key);

        OddsInfo oddsInfo = oddsInfoService.GetOddsInfo(session.QuestData.Id, request.area_idx);

        await dungeonService.ModifySession(
            request.dungeon_key,
            session => session.EnemyList[request.area_idx] = oddsInfo.enemy
        );

        return Ok(new DungeonGetAreaOddsData() { odds_info = oddsInfo });
    }

    [HttpPost("fail")]
    public async Task<DragaliaResult> Fail(DungeonFailRequest request)
    {
        DungeonSession session = await dungeonService.FinishDungeon(request.dungeon_key);

        DungeonFailData response =
            new()
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
            };

        if (session.IsMulti)
        {
            response.fail_quest_detail.is_host = await matchingService.GetIsHost();
            (response.fail_helper_list, response.fail_helper_detail_list) =
                await dungeonRecordHelperService.ProcessHelperDataMulti();
        }
        else
        {
            (response.fail_helper_list, response.fail_helper_detail_list) =
                await dungeonRecordHelperService.ProcessHelperDataSolo(session.SupportViewerId);
        }

        return this.Ok(response);
    }
}
