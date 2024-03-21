using DragaliaAPI.Controllers;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.MasterAsset.Models.Missions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Missions;

[Route("mission")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class MissionController(
    IMissionService missionService,
    IMissionRepository missionRepository,
    IRewardService rewardService,
    IUpdateDataService updateDataService
) : DragaliaControllerBase
{
    private readonly IMissionService missionService = missionService;
    private readonly IMissionRepository missionRepository = missionRepository;
    private readonly IRewardService rewardService = rewardService;
    private readonly IUpdateDataService updateDataService = updateDataService;

    [HttpPost("get_mission_list")]
    public async Task<DragaliaResult<MissionGetMissionListResponse>> GetMissionList()
    {
        MissionGetMissionListResponse response =
            await this.missionService.BuildNormalResponse<MissionGetMissionListResponse>();

        response.MissionNotice = await this.missionService.GetMissionNotice(null);
        response.CurrentMainStoryMission = await this.missionService.GetCurrentMainStoryMission();

        return response;
    }

    [HttpPost("get_drill_mission_list")]
    public async Task<DragaliaResult<MissionGetDrillMissionListResponse>> GetDrillMissionList()
    {
        MissionGetDrillMissionListResponse response = new();
        response.MissionNotice = await this.missionService.GetMissionNotice(null);

        IEnumerable<DbPlayerMission> drillMissions = await this
            .missionRepository.GetMissionsByType(MissionType.Drill)
            .ToListAsync();

        response.DrillMissionList = drillMissions.Select(x => new DrillMissionList(
            x.Id,
            x.Progress,
            (int)x.State,
            x.End,
            x.Start
        ));

        response.DrillMissionGroupList = await this.missionService.GetCompletedDrillGroups();

        return response;
    }

    [HttpPost("unlock_drill_mission_group")]
    public async Task<
        DragaliaResult<MissionUnlockDrillMissionGroupResponse>
    > UnlockDrillMissionGroup(
        MissionUnlockDrillMissionGroupRequest request,
        CancellationToken cancellationToken
    )
    {
        MissionUnlockDrillMissionGroupResponse response = new();

        IEnumerable<DbPlayerMission> drillMissions =
            await this.missionService.UnlockDrillMissionGroup(request.DrillMissionGroupId);

        response.DrillMissionList = drillMissions.Select(x => new DrillMissionList(
            x.Id,
            x.Progress,
            (int)x.State,
            x.End,
            x.Start
        ));

        response.UpdateDataList = await this.updateDataService.SaveChangesAsync(cancellationToken);

        return response;
    }

    [HttpPost("unlock_main_story_group")]
    public async Task<
        DragaliaResult<MissionUnlockMainStoryGroupResponse>
    > UnlockMainStoryMissionGroup(
        MissionUnlockMainStoryGroupRequest request,
        CancellationToken cancellationToken
    )
    {
        MissionUnlockMainStoryGroupResponse response = new();

        (IEnumerable<MainStoryMissionGroupReward> rewards, IEnumerable<DbPlayerMission> missions) =
            await this.missionService.UnlockMainMissionGroup(request.MainStoryMissionGroupId);

        response.MainStoryMissionList = missions.Select(x => new MainStoryMissionList(
            x.Id,
            x.Progress,
            (int)x.State,
            x.End,
            x.Start
        ));

        response.MainStoryMissionUnlockBonusList = rewards.Select(
            x => new AtgenBuildEventRewardEntityList(x.Type, x.Id, x.Quantity)
        );

        response.UpdateDataList = await this.updateDataService.SaveChangesAsync(cancellationToken);

        return response;
    }

    [HttpPost("receive_drill_reward")]
    public async Task<DragaliaResult<MissionReceiveDrillRewardResponse>> ReceiveDrillStoryReward(
        MissionReceiveDrillRewardRequest request,
        CancellationToken cancellationToken
    )
    {
        MissionReceiveDrillRewardResponse response = new();

        await this.missionService.RedeemMissions(MissionType.Drill, request.DrillMissionIdList);

        response.DrillMissionGroupCompleteRewardList =
            await this.missionService.TryRedeemDrillMissionGroups(request.DrillMissionGroupIdList);

        response.UpdateDataList = await this.updateDataService.SaveChangesAsync(cancellationToken);

        IEnumerable<DbPlayerMission> missions = await this
            .missionRepository.GetMissionsByType(MissionType.Drill)
            .ToListAsync(cancellationToken);

        response.DrillMissionList = missions.Select(x => new DrillMissionList(
            x.Id,
            x.Progress,
            (int)x.State,
            x.End,
            x.Start
        ));

        response.DrillMissionGroupList = await this.missionService.GetCompletedDrillGroups();
        response.EntityResult = this.rewardService.GetEntityResult();
        response.ConvertedEntityList = Enumerable.Empty<ConvertedEntityList>();

        return response;
    }

    [HttpPost("receive_main_story_reward")]
    public async Task<DragaliaResult<MissionReceiveMainStoryRewardResponse>> ReceiveMainStoryReward(
        MissionReceiveMainStoryRewardRequest request,
        CancellationToken cancellationToken
    )
    {
        await this.missionService.RedeemMissions(
            MissionType.MainStory,
            request.MainStoryMissionIdList
        );

        MissionReceiveMainStoryRewardResponse response =
            await this.missionService.BuildNormalResponse<MissionReceiveMainStoryRewardResponse>();
        response.UpdateDataList = await this.updateDataService.SaveChangesAsync(cancellationToken);

        response.EntityResult = this.rewardService.GetEntityResult();
        response.ConvertedEntityList = Enumerable.Empty<ConvertedEntityList>();

        return response;
    }

    [HttpPost("receive_period_reward")]
    public async Task<DragaliaResult<MissionReceivePeriodRewardResponse>> ReceivePeriodReward(
        MissionReceivePeriodRewardRequest request,
        CancellationToken cancellationToken
    )
    {
        await this.missionService.RedeemMissions(MissionType.Period, request.PeriodMissionIdList);

        MissionReceivePeriodRewardResponse response =
            await this.missionService.BuildNormalResponse<MissionReceivePeriodRewardResponse>();
        response.UpdateDataList = await this.updateDataService.SaveChangesAsync(cancellationToken);

        response.EntityResult = this.rewardService.GetEntityResult();
        response.ConvertedEntityList = Enumerable.Empty<ConvertedEntityList>();

        return response;
    }

    [HttpPost("receive_normal_reward")]
    public async Task<DragaliaResult<MissionReceiveNormalRewardResponse>> ReceiveNormalReward(
        MissionReceiveNormalRewardRequest request,
        CancellationToken cancellationToken
    )
    {
        await this.missionService.RedeemMissions(MissionType.Normal, request.NormalMissionIdList);

        MissionReceiveNormalRewardResponse response =
            await this.missionService.BuildNormalResponse<MissionReceiveNormalRewardResponse>();
        response.UpdateDataList = await this.updateDataService.SaveChangesAsync(cancellationToken);

        response.EntityResult = this.rewardService.GetEntityResult();
        response.ConvertedEntityList = Enumerable.Empty<ConvertedEntityList>();

        return response;
    }

    [HttpPost("receive_album_reward")]
    public async Task<DragaliaResult<MissionReceiveAlbumRewardResponse>> ReceiveAlbumReward(
        MissionReceiveAlbumRewardRequest request,
        CancellationToken cancellationToken
    )
    {
        await this.missionService.RedeemMissions(MissionType.Album, request.AlbumMissionIdList);

        MissionReceiveAlbumRewardResponse response =
            await this.missionService.BuildNormalResponse<MissionReceiveAlbumRewardResponse>();
        response.UpdateDataList = await this.updateDataService.SaveChangesAsync(cancellationToken);

        response.EntityResult = this.rewardService.GetEntityResult();
        response.ConvertedEntityList = Enumerable.Empty<ConvertedEntityList>();

        return response;
    }

    [HttpPost("receive_memory_event_reward")]
    public async Task<DragaliaResult<MissionReceiveMemoryEventRewardResponse>> ReceiveNormalReward(
        MissionReceiveMemoryEventRewardRequest request,
        CancellationToken cancellationToken
    )
    {
        await this.missionService.RedeemMissions(
            MissionType.MemoryEvent,
            request.MemoryEventMissionIdList
        );

        MissionReceiveMemoryEventRewardResponse response =
            await this.missionService.BuildNormalResponse<MissionReceiveMemoryEventRewardResponse>();
        response.UpdateDataList = await this.updateDataService.SaveChangesAsync(cancellationToken);

        response.EntityResult = this.rewardService.GetEntityResult();
        response.ConvertedEntityList = Enumerable.Empty<ConvertedEntityList>();

        return response;
    }

    [HttpPost("receive_beginner_reward")]
    public async Task<DragaliaResult<MissionReceiveBeginnerRewardResponse>> ReceiveBeginnerReward(
        MissionReceiveBeginnerRewardRequest request,
        CancellationToken cancellationToken
    )
    {
        await this.missionService.RedeemMissions(
            MissionType.Beginner,
            request.BeginnerMissionIdList
        );

        MissionReceiveBeginnerRewardResponse response =
            await this.missionService.BuildNormalResponse<MissionReceiveBeginnerRewardResponse>();
        response.UpdateDataList = await this.updateDataService.SaveChangesAsync(cancellationToken);

        response.EntityResult = this.rewardService.GetEntityResult();
        response.ConvertedEntityList = Enumerable.Empty<ConvertedEntityList>();

        return response;
    }

    [HttpPost("receive_special_reward")]
    public async Task<DragaliaResult<MissionReceiveSpecialRewardResponse>> ReceiveBeginnerReward(
        MissionReceiveSpecialRewardRequest request,
        CancellationToken cancellationToken
    )
    {
        await this.missionService.RedeemMissions(MissionType.Special, request.SpecialMissionIdList);

        MissionReceiveSpecialRewardResponse response =
            await this.missionService.BuildNormalResponse<MissionReceiveSpecialRewardResponse>();
        response.UpdateDataList = await this.updateDataService.SaveChangesAsync(cancellationToken);

        response.EntityResult = this.rewardService.GetEntityResult();
        response.ConvertedEntityList = Enumerable.Empty<ConvertedEntityList>();

        return response;
    }

    [HttpPost("receive_daily_reward")]
    public async Task<DragaliaResult<MissionReceiveDailyRewardResponse>> ReceiveDailyReward(
        MissionReceiveDailyRewardRequest request,
        CancellationToken cancellationToken
    )
    {
        await this.missionService.RedeemDailyMissions(request.MissionParamsList);

        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync(
            cancellationToken
        );

        MissionReceiveDailyRewardResponse response =
            await this.missionService.BuildNormalResponse<MissionReceiveDailyRewardResponse>();

        response.EntityResult = this.rewardService.GetEntityResult();
        response.ConvertedEntityList = Enumerable.Empty<ConvertedEntityList>();
        response.UpdateDataList = updateDataList;

        return response;
    }
}
