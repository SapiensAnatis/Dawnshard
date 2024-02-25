using DragaliaAPI.Controllers;
using DragaliaAPI.Features.Dungeon.AutoRepeat;
using DragaliaAPI.Features.Dungeon.Start;
using DragaliaAPI.Features.TimeAttack;
using DragaliaAPI.Middleware;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Definitions.Enums.Dungeon;
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
    IAutoRepeatService autoRepeatService,
    IUpdateDataService updateDataService
) : DragaliaControllerBase
{
    [HttpPost("record")]
    public async Task<DragaliaResult<DungeonRecordRecordData>> Record(
        DungeonRecordRecordRequest request
    )
    {
        DungeonSession session = await dungeonService.FinishDungeon(request.DungeonKey);

        IngameResultData ingameResultData = await dungeonRecordService.GenerateIngameResultData(
            request.DungeonKey,
            request.PlayRecord,
            session
        );

        (
            IEnumerable<UserSupportList> helperList,
            IEnumerable<AtgenHelperDetailList> helperDetailList
        ) = await dungeonRecordHelperService.ProcessHelperDataSolo(session.SupportViewerId);

        ingameResultData.HelperList = helperList;
        ingameResultData.HelperDetailList = helperDetailList;

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync();

        DungeonRecordRecordData response =
            new() { IngameResultData = ingameResultData, UpdateDataList = updateDataList };

        if (session.QuestData?.IsSumUpTotalDamage ?? false)
        {
            response.EventDamageRanking = await dungeonRecordDamageService.GetEventDamageRanking(
                request.PlayRecord,
                session.QuestData.Gid
            );
        }

        if (request.RepeatState != 0)
        {
            response.RepeatData = await autoRepeatService.RecordRepeat(
                request.RepeatKey,
                ingameResultData,
                updateDataList
            );
        }

        return response;
    }

    [HttpPost("record_multi")]
    [Authorize(AuthenticationSchemes = nameof(PhotonAuthenticationHandler))]
    public async Task<DragaliaResult> RecordMulti(DungeonRecordRecordMultiRequest request)
    {
        DungeonSession session = await dungeonService.FinishDungeon(request.DungeonKey);

        IngameResultData ingameResultData = await dungeonRecordService.GenerateIngameResultData(
            request.DungeonKey,
            request.PlayRecord,
            session
        );

        (
            IEnumerable<UserSupportList> helperList,
            IEnumerable<AtgenHelperDetailList> helperDetailList
        ) = await dungeonRecordHelperService.ProcessHelperDataMulti();

        ingameResultData.HelperList = helperList;
        ingameResultData.HelperDetailList = helperDetailList;
        ingameResultData.PlayType = QuestPlayType.Multi;

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync();

        DungeonRecordRecordData response =
            new() { IngameResultData = ingameResultData, UpdateDataList = updateDataList, };

        if (session.QuestData?.IsSumUpTotalDamage ?? false)
        {
            response.EventDamageRanking = await dungeonRecordDamageService.GetEventDamageRanking(
                request.PlayRecord,
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

        await timeAttackService.RegisterRankedClear(gameId, request.PlayRecord.Time);
        await updateDataService.SaveChangesAsync();

        return this.Ok(new ResultCodeResponse(ResultCode.Success));
    }
}
