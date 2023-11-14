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
    IUpdateDataService updateDataService
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

    [HttpPost("record_time_attack")]
    [Authorize(
        AuthenticationSchemes = nameof(PhotonAuthenticationHandler),
        Roles = PhotonAuthenticationHandler.Role
    )]
    public async Task<DragaliaResult> RecordTimeAttack(
        [FromHeader(Name = "RoomName")] string roomName,
        [FromHeader(Name = "RoomId")] int roomId,
        [FromBody] DungeonRecordRecordMultiRequest request
    )
    {
        string gameId = $"{roomName}_{roomId}";

        await timeAttackService.RegisterRankedClear(gameId, request.play_record.time);
        await updateDataService.SaveChangesAsync();

        return this.Ok(new ResultCodeData(ResultCode.Success));
    }
}
