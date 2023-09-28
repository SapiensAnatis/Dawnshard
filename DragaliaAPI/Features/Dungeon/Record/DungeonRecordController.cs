#define ALLOW_SUB4_CLEARS

using DragaliaAPI.Controllers;
using DragaliaAPI.Features.TimeAttack;
using DragaliaAPI.Middleware;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Dungeon.Record;

[Route("dungeon_record")]
public class DungeonRecordController(
    IDungeonRecordService dungeonRecordService,
    IDungeonRecordDamageService dungeonRecordDamageService,
    IDungeonRecordHelperService dungeonRecordHelperService,
    IDungeonService dungeonService,
    ITimeAttackService timeAttackService,
    IUpdateDataService updateDataService,
    ILogger<DungeonRecordController> logger
) : DragaliaControllerBase
{
    [HttpPost("record")]
    public async Task<DragaliaResult> Record(DungeonRecordRecordRequest request)
    {
        DungeonSession session = await dungeonService.FinishDungeon(request.dungeon_key);

        IngameResultData ingameResultData = await dungeonRecordService.GenerateIngameResultData(
            request.dungeon_key,
            request.play_record,
            session
        );

        (
            IEnumerable<UserSupportList> helperList,
            IEnumerable<AtgenHelperDetailList> helperDetailList
        ) = await dungeonRecordHelperService.ProcessHelperDataSolo(session.SupportViewerId);

        ingameResultData.helper_list = helperList;
        ingameResultData.helper_detail_list = helperDetailList;

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync();

        DungeonRecordRecordData response =
            new() { ingame_result_data = ingameResultData, update_data_list = updateDataList, };

        if (session.QuestData.IsSumUpTotalDamage)
        {
            response.event_damage_ranking = await dungeonRecordDamageService.GetEventDamageRanking(
                request.play_record,
                session.QuestData.Gid
            );
        }

        return Ok(response);
    }

    [HttpPost("record_multi")]
    [Authorize(AuthenticationSchemes = nameof(PhotonAuthenticationHandler))]
    public async Task<DragaliaResult> RecordMulti(DungeonRecordRecordMultiRequest request)
    {
        logger.LogDebug(
            "Received record_multi request with connecting_viewer_id_list: {@viewerIdList}",
            request.connecting_viewer_id_list
        );

        DungeonSession session = await dungeonService.FinishDungeon(request.dungeon_key);

        IngameResultData ingameResultData = await dungeonRecordService.GenerateIngameResultData(
            request.dungeon_key,
            request.play_record,
            session
        );

        (
            IEnumerable<UserSupportList> helperList,
            IEnumerable<AtgenHelperDetailList> helperDetailList
        ) = await dungeonRecordHelperService.ProcessHelperDataMulti();

        ingameResultData.helper_list = helperList;
        ingameResultData.helper_detail_list = helperDetailList;
        ingameResultData.play_type = QuestPlayType.Multi;

        if (this.ShouldRegisterRankedClear(session.QuestData.Id, request))
        {
            await timeAttackService.RegisterRankedClear(
                session.QuestData.Id,
                request.play_record.time
            );
        }

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync();

        DungeonRecordRecordData response =
            new() { ingame_result_data = ingameResultData, update_data_list = updateDataList, };

        if (session.QuestData.IsSumUpTotalDamage)
        {
            response.event_damage_ranking = await dungeonRecordDamageService.GetEventDamageRanking(
                request.play_record,
                session.QuestData.Gid
            );
        }

        return Ok(response);
    }

    private bool ShouldRegisterRankedClear(int questId, DungeonRecordRecordMultiRequest request)
    {
        if (!timeAttackService.GetIsRankedQuest(questId))
        {
            logger.LogDebug(
                "Not registering clear: quest {id} was not a time attack quest",
                questId
            );

            return false;
        }

#if !ALLOW_SUB4_CLEARS || !DEBUG
        if (request.connecting_viewer_id_list.Count() < 4)
        {
            logger.LogDebug(
                "Not registering clear: connecting_viewer_id_list had {numPlayers} players",
                request.connecting_viewer_id_list
            );

            return false;
        }
#endif

        if (!this.HttpContext.User.IsInRole(PhotonAuthenticationHandler.Role))
        {
            logger.LogDebug("Not registering clear: user was not in Photon role");

            return false;
        }

        return true;
    }
}
