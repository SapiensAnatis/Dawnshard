using DragaliaAPI.Controllers;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Helpers;
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
    public async Task<DragaliaResult<MissionGetMissionListData>> GetMissionList()
    {
        MissionGetMissionListData response =
            await this.missionService.BuildNormalResponse<MissionGetMissionListData>();

        response.MissionNotice = await this.missionService.GetMissionNotice(null);
        response.CurrentMainStoryMission = await this.missionService.GetCurrentMainStoryMission();

        return response;
    }

    [HttpPost("get_drill_mission_list")]
    public async Task<DragaliaResult<MissionGetDrillMissionListData>> GetDrillMissionList()
    {
        MissionGetDrillMissionListData response = new();
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
    public async Task<DragaliaResult<MissionUnlockDrillMissionGroupData>> UnlockDrillMissionGroup(
        MissionUnlockDrillMissionGroupRequest request
    )
    {
        MissionUnlockDrillMissionGroupData response = new();

        IEnumerable<DbPlayerMission> drillMissions =
            await this.missionService.UnlockDrillMissionGroup(request.DrillMissionGroupId);

        response.DrillMissionList = drillMissions.Select(x => new DrillMissionList(
            x.Id,
            x.Progress,
            (int)x.State,
            x.End,
            x.Start
        ));

        response.UpdateDataList = await this.updateDataService.SaveChangesAsync();

        return response;
    }

    [HttpPost("unlock_main_story_group")]
    public async Task<DragaliaResult<MissionUnlockMainStoryGroupData>> UnlockMainStoryMissionGroup(
        MissionUnlockMainStoryGroupRequest request
    )
    {
        MissionUnlockMainStoryGroupData response = new();

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

        response.UpdateDataList = await this.updateDataService.SaveChangesAsync();

        return response;
    }

    [HttpPost("receive_drill_reward")]
    public async Task<DragaliaResult<MissionReceiveDrillRewardData>> ReceiveDrillStoryReward(
        MissionReceiveDrillRewardRequest request
    )
    {
        MissionReceiveDrillRewardData response = new();

        await this.missionService.RedeemMissions(MissionType.Drill, request.DrillMissionIdList);

        response.DrillMissionGroupCompleteRewardList =
            await this.missionService.TryRedeemDrillMissionGroups(request.DrillMissionGroupIdList);

        response.UpdateDataList = await this.updateDataService.SaveChangesAsync();

        IEnumerable<DbPlayerMission> missions = await this
            .missionRepository.GetMissionsByType(MissionType.Drill)
            .ToListAsync();

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
    public async Task<DragaliaResult<MissionReceiveMainStoryRewardData>> ReceiveMainStoryReward(
        MissionReceiveMainStoryRewardRequest request
    )
    {
        await this.missionService.RedeemMissions(
            MissionType.MainStory,
            request.MainStoryMissionIdList
        );

        MissionReceiveMainStoryRewardData response =
            await this.missionService.BuildNormalResponse<MissionReceiveMainStoryRewardData>();
        response.UpdateDataList = await this.updateDataService.SaveChangesAsync();
        response.EntityResult = this.rewardService.GetEntityResult();
        response.ConvertedEntityList = Enumerable.Empty<ConvertedEntityList>();

        return response;
    }

    [HttpPost("receive_period_reward")]
    public async Task<DragaliaResult<MissionReceivePeriodRewardData>> ReceivePeriodReward(
        MissionReceivePeriodRewardRequest request
    )
    {
        await this.missionService.RedeemMissions(MissionType.Period, request.PeriodMissionIdList);

        MissionReceivePeriodRewardData response =
            await this.missionService.BuildNormalResponse<MissionReceivePeriodRewardData>();
        response.UpdateDataList = await this.updateDataService.SaveChangesAsync();
        response.EntityResult = this.rewardService.GetEntityResult();
        response.ConvertedEntityList = Enumerable.Empty<ConvertedEntityList>();

        return response;
    }

    [HttpPost("receive_normal_reward")]
    public async Task<DragaliaResult<MissionReceiveNormalRewardData>> ReceiveNormalReward(
        MissionReceiveNormalRewardRequest request
    )
    {
        await this.missionService.RedeemMissions(MissionType.Normal, request.NormalMissionIdList);

        MissionReceiveNormalRewardData response =
            await this.missionService.BuildNormalResponse<MissionReceiveNormalRewardData>();
        response.UpdateDataList = await this.updateDataService.SaveChangesAsync();
        response.EntityResult = this.rewardService.GetEntityResult();
        response.ConvertedEntityList = Enumerable.Empty<ConvertedEntityList>();

        return response;
    }

    [HttpPost("receive_album_reward")]
    public async Task<DragaliaResult<MissionReceiveAlbumRewardData>> ReceiveAlbumReward(
        MissionReceiveAlbumRewardRequest request
    )
    {
        await this.missionService.RedeemMissions(MissionType.Album, request.AlbumMissionIdList);

        MissionReceiveAlbumRewardData response =
            await this.missionService.BuildNormalResponse<MissionReceiveAlbumRewardData>();
        response.UpdateDataList = await this.updateDataService.SaveChangesAsync();
        response.EntityResult = this.rewardService.GetEntityResult();
        response.ConvertedEntityList = Enumerable.Empty<ConvertedEntityList>();

        return response;
    }

    [HttpPost("receive_memory_event_reward")]
    public async Task<DragaliaResult<MissionReceiveMemoryEventRewardData>> ReceiveNormalReward(
        MissionReceiveMemoryEventRewardRequest request
    )
    {
        await this.missionService.RedeemMissions(
            MissionType.MemoryEvent,
            request.MemoryEventMissionIdList
        );

        MissionReceiveMemoryEventRewardData response =
            await this.missionService.BuildNormalResponse<MissionReceiveMemoryEventRewardData>();
        response.UpdateDataList = await this.updateDataService.SaveChangesAsync();
        response.EntityResult = this.rewardService.GetEntityResult();
        response.ConvertedEntityList = Enumerable.Empty<ConvertedEntityList>();

        return response;
    }

    [HttpPost("receive_beginner_reward")]
    public async Task<DragaliaResult<MissionReceiveBeginnerRewardData>> ReceiveBeginnerReward(
        MissionReceiveBeginnerRewardRequest request
    )
    {
        await this.missionService.RedeemMissions(
            MissionType.Beginner,
            request.BeginnerMissionIdList
        );

        MissionReceiveBeginnerRewardData response =
            await this.missionService.BuildNormalResponse<MissionReceiveBeginnerRewardData>();
        response.UpdateDataList = await this.updateDataService.SaveChangesAsync();
        response.EntityResult = this.rewardService.GetEntityResult();
        response.ConvertedEntityList = Enumerable.Empty<ConvertedEntityList>();

        return response;
    }

    [HttpPost("receive_special_reward")]
    public async Task<DragaliaResult<MissionReceiveSpecialRewardData>> ReceiveBeginnerReward(
        MissionReceiveSpecialRewardRequest request
    )
    {
        await this.missionService.RedeemMissions(MissionType.Special, request.SpecialMissionIdList);

        MissionReceiveSpecialRewardData response =
            await this.missionService.BuildNormalResponse<MissionReceiveSpecialRewardData>();
        response.UpdateDataList = await this.updateDataService.SaveChangesAsync();
        response.EntityResult = this.rewardService.GetEntityResult();
        response.ConvertedEntityList = Enumerable.Empty<ConvertedEntityList>();

        return response;
    }

    [HttpPost("receive_daily_reward")]
    public async Task<DragaliaResult<MissionReceiveDailyRewardData>> ReceiveDailyReward(
        MissionReceiveDailyRewardRequest request
    )
    {
        await this.missionService.RedeemDailyMissions(request.MissionParamsList);

        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync();

        MissionReceiveDailyRewardData response =
            await this.missionService.BuildNormalResponse<MissionReceiveDailyRewardData>();

        response.EntityResult = this.rewardService.GetEntityResult();
        response.ConvertedEntityList = Enumerable.Empty<ConvertedEntityList>();
        response.UpdateDataList = updateDataList;

        return response;
    }
}
