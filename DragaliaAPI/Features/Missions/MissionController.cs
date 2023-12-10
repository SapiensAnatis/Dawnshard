﻿using DragaliaAPI.Controllers;
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
public class MissionController : DragaliaControllerBase
{
    private readonly IMissionService missionService;
    private readonly IMissionRepository missionRepository;
    private readonly IUserDataRepository userDataRepository;
    private readonly IRewardService rewardService;
    private readonly IUpdateDataService updateDataService;

    public MissionController(
        IMissionService missionService,
        IMissionRepository missionRepository,
        IUserDataRepository userDataRepository,
        IRewardService rewardService,
        IUpdateDataService updateDataService
    )
    {
        this.missionService = missionService;
        this.missionRepository = missionRepository;
        this.userDataRepository = userDataRepository;
        this.rewardService = rewardService;
        this.updateDataService = updateDataService;
    }

    [HttpPost("get_mission_list")]
    public async Task<DragaliaResult<MissionGetMissionListData>> GetMissionList()
    {
        MissionGetMissionListData response =
            await this.BuildNormalResponse<MissionGetMissionListData>();

        response.mission_notice = await this.missionService.GetMissionNotice(null);
        response.current_main_story_mission =
            await this.missionService.GetCurrentMainStoryMission();

        return response;
    }

    [HttpPost("get_drill_mission_list")]
    public async Task<DragaliaResult<MissionGetDrillMissionListData>> GetDrillMissionList()
    {
        MissionGetDrillMissionListData response = new();
        response.mission_notice = await this.missionService.GetMissionNotice(null);

        IEnumerable<DbPlayerMission> drillMissions = await this.missionRepository
            .GetMissionsByType(MissionType.Drill)
            .ToListAsync();

        response.drill_mission_list = drillMissions.Select(
            x => new DrillMissionList(x.Id, x.Progress, (int)x.State, x.End, x.Start)
        );

        return response;
    }

    [HttpPost("unlock_drill_mission_group")]
    public async Task<DragaliaResult<MissionUnlockDrillMissionGroupData>> UnlockDrillMissionGroup(
        MissionUnlockDrillMissionGroupRequest request
    )
    {
        MissionUnlockDrillMissionGroupData response = new();

        IEnumerable<DbPlayerMission> drillMissions =
            await this.missionService.UnlockDrillMissionGroup(request.drill_mission_group_id);

        response.drill_mission_list = drillMissions.Select(
            x => new DrillMissionList(x.Id, x.Progress, (int)x.State, x.End, x.Start)
        );

        response.update_data_list = await this.updateDataService.SaveChangesAsync();

        return response;
    }

    [HttpPost("unlock_main_story_group")]
    public async Task<DragaliaResult<MissionUnlockMainStoryGroupData>> UnlockMainStoryMissionGroup(
        MissionUnlockMainStoryGroupRequest request
    )
    {
        MissionUnlockMainStoryGroupData response = new();

        (IEnumerable<MainStoryMissionGroupReward> rewards, IEnumerable<DbPlayerMission> missions) =
            await this.missionService.UnlockMainMissionGroup(request.main_story_mission_group_id);

        response.main_story_mission_list = missions.Select(
            x => new MainStoryMissionList(x.Id, x.Progress, (int)x.State, x.End, x.Start)
        );

        response.main_story_mission_unlock_bonus_list = rewards.Select(
            x => new AtgenBuildEventRewardEntityList(x.Type, x.Id, x.Quantity)
        );

        response.update_data_list = await this.updateDataService.SaveChangesAsync();

        return response;
    }

    [HttpPost("receive_drill_reward")]
    public async Task<DragaliaResult<MissionReceiveDrillRewardData>> ReceiveDrillStoryReward(
        MissionReceiveDrillRewardRequest request
    )
    {
        MissionReceiveDrillRewardData response = new();

        await this.missionService.RedeemMissions(MissionType.Drill, request.drill_mission_id_list);

        response.drill_mission_group_complete_reward_list =
            await this.missionService.TryRedeemDrillMissionGroups(
                request.drill_mission_group_id_list
            );

        response.update_data_list = await this.updateDataService.SaveChangesAsync();

        IEnumerable<DbPlayerMission> missions = await this.missionRepository
            .GetMissionsByType(MissionType.Drill)
            .ToListAsync();

        response.drill_mission_list = missions.Select(
            x => new DrillMissionList(x.Id, x.Progress, (int)x.State, x.End, x.Start)
        );

        response.drill_mission_group_list = await this.missionService.GetCompletedDrillGroups();
        response.entity_result = this.rewardService.GetEntityResult();
        response.converted_entity_list = Enumerable.Empty<ConvertedEntityList>();

        return response;
    }

    [HttpPost("receive_main_story_reward")]
    public async Task<DragaliaResult<MissionReceiveMainStoryRewardData>> ReceiveMainStoryReward(
        MissionReceiveMainStoryRewardRequest request
    )
    {
        await this.missionService.RedeemMissions(
            MissionType.MainStory,
            request.main_story_mission_id_list
        );

        MissionReceiveMainStoryRewardData response =
            await this.BuildNormalResponse<MissionReceiveMainStoryRewardData>();
        response.update_data_list = await this.updateDataService.SaveChangesAsync();
        response.entity_result = this.rewardService.GetEntityResult();
        response.converted_entity_list = Enumerable.Empty<ConvertedEntityList>();

        return response;
    }

    [HttpPost("receive_period_reward")]
    public async Task<DragaliaResult<MissionReceivePeriodRewardData>> ReceivePeriodReward(
        MissionReceivePeriodRewardRequest request
    )
    {
        await this.missionService.RedeemMissions(
            MissionType.Period,
            request.period_mission_id_list
        );

        MissionReceivePeriodRewardData response =
            await this.BuildNormalResponse<MissionReceivePeriodRewardData>();
        response.update_data_list = await this.updateDataService.SaveChangesAsync();
        response.entity_result = this.rewardService.GetEntityResult();
        response.converted_entity_list = Enumerable.Empty<ConvertedEntityList>();

        return response;
    }

    [HttpPost("receive_normal_reward")]
    public async Task<DragaliaResult<MissionReceiveNormalRewardData>> ReceiveNormalReward(
        MissionReceiveNormalRewardRequest request
    )
    {
        await this.missionService.RedeemMissions(
            MissionType.Normal,
            request.normal_mission_id_list
        );

        MissionReceiveNormalRewardData response =
            await this.BuildNormalResponse<MissionReceiveNormalRewardData>();
        response.update_data_list = await this.updateDataService.SaveChangesAsync();
        response.entity_result = this.rewardService.GetEntityResult();
        response.converted_entity_list = Enumerable.Empty<ConvertedEntityList>();

        return response;
    }

    [HttpPost("receive_album_reward")]
    public async Task<DragaliaResult<MissionReceiveAlbumRewardData>> ReceiveAlbumReward(
        MissionReceiveAlbumRewardRequest request
    )
    {
        await this.missionService.RedeemMissions(MissionType.Album, request.album_mission_id_list);

        MissionReceiveAlbumRewardData response =
            await this.BuildNormalResponse<MissionReceiveAlbumRewardData>();
        response.update_data_list = await this.updateDataService.SaveChangesAsync();
        response.entity_result = this.rewardService.GetEntityResult();
        response.converted_entity_list = Enumerable.Empty<ConvertedEntityList>();

        return response;
    }

    [HttpPost("receive_memory_event_reward")]
    public async Task<DragaliaResult<MissionReceiveMemoryEventRewardData>> ReceiveNormalReward(
        MissionReceiveMemoryEventRewardRequest request
    )
    {
        await this.missionService.RedeemMissions(
            MissionType.MemoryEvent,
            request.memory_event_mission_id_list
        );

        MissionReceiveMemoryEventRewardData response =
            await this.BuildNormalResponse<MissionReceiveMemoryEventRewardData>();
        response.update_data_list = await this.updateDataService.SaveChangesAsync();
        response.entity_result = this.rewardService.GetEntityResult();
        response.converted_entity_list = Enumerable.Empty<ConvertedEntityList>();

        return response;
    }

    [HttpPost("receive_beginner_reward")]
    public async Task<DragaliaResult<MissionReceiveBeginnerRewardData>> ReceiveBeginnerReward(
        MissionReceiveBeginnerRewardRequest request
    )
    {
        await this.missionService.RedeemMissions(
            MissionType.Beginner,
            request.beginner_mission_id_list
        );

        MissionReceiveBeginnerRewardData response =
            await this.BuildNormalResponse<MissionReceiveBeginnerRewardData>();
        response.update_data_list = await this.updateDataService.SaveChangesAsync();
        response.entity_result = this.rewardService.GetEntityResult();
        response.converted_entity_list = Enumerable.Empty<ConvertedEntityList>();

        return response;
    }

    [HttpPost("receive_special_reward")]
    public async Task<DragaliaResult<MissionReceiveSpecialRewardData>> ReceiveBeginnerReward(
        MissionReceiveSpecialRewardRequest request
    )
    {
        await this.missionService.RedeemMissions(
            MissionType.Special,
            request.special_mission_id_list
        );

        MissionReceiveSpecialRewardData response =
            await this.BuildNormalResponse<MissionReceiveSpecialRewardData>();
        response.update_data_list = await this.updateDataService.SaveChangesAsync();
        response.entity_result = this.rewardService.GetEntityResult();
        response.converted_entity_list = Enumerable.Empty<ConvertedEntityList>();

        return response;
    }

    private async Task<TResponse> BuildNormalResponse<TResponse>()
        where TResponse : INormalMissionEndpointResponse, new()
    {
        ILookup<MissionType, DbPlayerMission> allMissions =
            await this.missionRepository.GetAllMissionsPerTypeAsync();

        int activeEventId = await this.userDataRepository
            .UserData
            .Select(x => x.ActiveMemoryEventId)
            .FirstAsync();

        int dayNo = int.Parse(DateTimeOffset.UtcNow.ToString("yyMMdd"));

        TResponse response =
            new()
            {
                album_mission_list = allMissions[MissionType.Album].Select(
                    x => new AlbumMissionList(x.Id, x.Progress, (int)x.State, x.End, x.Start)
                ),
                beginner_mission_list = allMissions[MissionType.Beginner].Select(
                    x => new BeginnerMissionList(x.Id, x.Progress, (int)x.State, x.End, x.Start)
                ),
                daily_mission_list = [],
                main_story_mission_list = allMissions[MissionType.MainStory].Select(
                    x => new MainStoryMissionList(x.Id, x.Progress, (int)x.State, x.End, x.Start)
                ),
                normal_mission_list = allMissions[MissionType.Normal].Select(
                    x => new NormalMissionList(x.Id, x.Progress, (int)x.State, x.End, x.Start)
                ),
                period_mission_list = allMissions[MissionType.Period].Select(
                    x => new PeriodMissionList(x.Id, x.Progress, (int)x.State, x.End, x.Start)
                ),
                special_mission_list = allMissions[MissionType.Special].Select(
                    x => new SpecialMissionList(x.Id, x.Progress, (int)x.State, x.End, x.Start)
                ),
                memory_event_mission_list = allMissions[MissionType.MemoryEvent]
                    .Where(x => x.GroupId == activeEventId)
                    .Select(
                        x =>
                            new MemoryEventMissionList(
                                x.Id,
                                x.Progress,
                                (int)x.State,
                                x.End,
                                x.Start
                            )
                    ),
            };

        return response;
    }
}
