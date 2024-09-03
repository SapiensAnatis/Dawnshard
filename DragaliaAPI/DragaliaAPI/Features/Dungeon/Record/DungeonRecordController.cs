﻿using DragaliaAPI.Controllers;
using DragaliaAPI.Features.Dungeon.AutoRepeat;
using DragaliaAPI.Features.TimeAttack;
using DragaliaAPI.Infrastructure.Middleware;
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
    IDungeonRecordRewardService dungeonRecordRewardService,
    IDungeonService dungeonService,
    ITimeAttackService timeAttackService,
    IAutoRepeatService autoRepeatService,
    IUpdateDataService updateDataService
) : DragaliaControllerBase
{
    [HttpPost("record")]
    public async Task<DragaliaResult<DungeonRecordRecordResponse>> Record(
        DungeonRecordRecordRequest request,
        CancellationToken cancellationToken
    )
    {
        DungeonSession session = await dungeonService.GetSession(
            request.DungeonKey,
            cancellationToken
        );

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

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync(cancellationToken);

        DungeonRecordRecordResponse response =
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

        await dungeonService.RemoveSession(request.DungeonKey, cancellationToken);

        return response;
    }

    [HttpPost("record_multi")]
    [Authorize(AuthenticationSchemes = nameof(PhotonAuthenticationHandler))]
    public async Task<DragaliaResult<DungeonRecordRecordMultiResponse>> RecordMulti(
        DungeonRecordRecordMultiRequest request,
        CancellationToken cancellationToken
    )
    {
        DungeonSession session = await dungeonService.GetSession(
            request.DungeonKey,
            cancellationToken
        );

        IngameResultData ingameResultData = await dungeonRecordService.GenerateIngameResultData(
            request.DungeonKey,
            request.PlayRecord,
            session
        );

        if (request.ConnectingViewerIdList is not null)
        {
            ingameResultData.RewardRecord.FirstMeeting =
                dungeonRecordRewardService.ProcessFirstMeetingRewards(
                    request.ConnectingViewerIdList
                );

            (ingameResultData.HelperList, ingameResultData.HelperDetailList) =
                await dungeonRecordHelperService.ProcessHelperDataMulti(
                    request.ConnectingViewerIdList
                );
        }
        else
        {
            (ingameResultData.HelperList, ingameResultData.HelperDetailList) =
                await dungeonRecordHelperService.ProcessHelperDataMulti();
        }

        ingameResultData.PlayType = QuestPlayType.Multi;

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync(cancellationToken);

        DungeonRecordRecordMultiResponse response =
            new() { IngameResultData = ingameResultData, UpdateDataList = updateDataList };

        if (session.QuestData?.IsSumUpTotalDamage ?? false)
        {
            response.EventDamageRanking = await dungeonRecordDamageService.GetEventDamageRanking(
                request.PlayRecord,
                session.QuestData.Gid
            );
        }

        await dungeonService.RemoveSession(request.DungeonKey, cancellationToken);

        return response;
    }

    [HttpPost("record_time_attack")]
    [Authorize(
        AuthenticationSchemes = nameof(PhotonAuthenticationHandler),
        Roles = PhotonAuthenticationHandler.Role
    )]
    public async Task<DragaliaResult> RecordTimeAttack(
        [FromHeader(Name = "RoomName")] string roomName,
        [FromHeader(Name = "RoomId")] int roomId,
        [FromBody] DungeonRecordRecordMultiRequest request,
        CancellationToken cancellationToken
    )
    {
        string gameId = $"{roomName}_{roomId}";

        await timeAttackService.RegisterRankedClear(gameId, request.PlayRecord.Time);
        await updateDataService.SaveChangesAsync(cancellationToken);

        return this.Ok(new ResultCodeResponse(ResultCode.Success));
    }
}
