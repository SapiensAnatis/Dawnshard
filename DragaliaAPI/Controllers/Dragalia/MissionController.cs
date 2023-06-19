using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.MasterAsset.Models.Missions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("mission")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class MissionController : DragaliaControllerBase
{
    private readonly IMissionService missionService;
    private readonly IMissionRepository missionRepository;
    private readonly IUpdateDataService updateDataService;

    public MissionController(
        IMissionService missionService,
        IMissionRepository missionRepository,
        IUpdateDataService updateDataService
    )
    {
        this.missionService = missionService;
        this.missionRepository = missionRepository;
        this.updateDataService = updateDataService;
    }

    [HttpPost("get_mission_list")]
    public async Task<DragaliaResult> GetMissionList()
    {
        MissionGetMissionListData response = new();
        response.mission_notice = await this.missionService.GetMissionNotice(null);
        response.current_main_story_mission =
            await this.missionService.GetCurrentMainStoryMission();

        await BuildNormalResponse(response);
        return Ok(response);
    }

    [HttpPost("get_drill_mission_list")]
    public async Task<DragaliaResult> GetDrillMissionList()
    {
        MissionGetDrillMissionListData response = new();
        response.mission_notice = await this.missionService.GetMissionNotice(null);

        IEnumerable<DbPlayerMission> drillMissions = await this.missionRepository
            .GetMissionsByType(MissionType.Drill)
            .ToListAsync();

        response.drill_mission_list = drillMissions.Select(
            x => new DrillMissionList(x.Id, x.Progress, (int)x.State, x.End, x.Start)
        );

        return Ok(response);
    }

    [HttpPost("unlock_drill_mission_group")]
    public async Task<DragaliaResult> UnlockDrillMissionGroup(
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

        return Ok(response);
    }

    [HttpPost("unlock_main_story_group")]
    public async Task<DragaliaResult> UnlockMainStoryMissionGroup(
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

        return Ok(response);
    }

    [HttpPost("receive_drill_reward")]
    public async Task<DragaliaResult> ReceiveDrillStoryReward(
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

        response.converted_entity_list = Enumerable.Empty<ConvertedEntityList>();
        response.entity_result = new()
        {
            converted_entity_list = Enumerable.Empty<ConvertedEntityList>()
        };

        return Ok(response);
    }

    [HttpPost("receive_main_story_reward")]
    public async Task<DragaliaResult> ReceiveMainStoryReward(
        MissionReceiveMainStoryRewardRequest request
    )
    {
        MissionReceiveMainStoryRewardData response = new();

        await this.missionService.RedeemMissions(
            MissionType.MainStory,
            request.main_story_mission_id_list
        );

        await BuildNormalResponse(response);
        response.update_data_list = await this.updateDataService.SaveChangesAsync();
        response.converted_entity_list = Enumerable.Empty<ConvertedEntityList>();
        response.entity_result = new()
        {
            converted_entity_list = Enumerable.Empty<ConvertedEntityList>()
        };

        return Ok(response);
    }

    [HttpPost("receive_period_reward")]
    public async Task<DragaliaResult> ReceivePeriodReward(MissionReceivePeriodRewardRequest request)
    {
        MissionReceivePeriodRewardData response = new();

        await this.missionService.RedeemMissions(
            MissionType.Period,
            request.period_mission_id_list
        );

        await BuildNormalResponse(response);
        response.update_data_list = await this.updateDataService.SaveChangesAsync();
        response.converted_entity_list = Enumerable.Empty<ConvertedEntityList>();
        response.entity_result = new()
        {
            converted_entity_list = Enumerable.Empty<ConvertedEntityList>()
        };

        return Ok(response);
    }

    [HttpPost("receive_normal_reward")]
    public async Task<DragaliaResult> ReceiveNormalReward(MissionReceiveNormalRewardRequest request)
    {
        MissionReceiveNormalRewardData response = new();

        await this.missionService.RedeemMissions(
            MissionType.Normal,
            request.normal_mission_id_list
        );

        await BuildNormalResponse(response);
        response.update_data_list = await this.updateDataService.SaveChangesAsync();
        response.converted_entity_list = Enumerable.Empty<ConvertedEntityList>();
        response.entity_result = new()
        {
            converted_entity_list = Enumerable.Empty<ConvertedEntityList>()
        };

        return Ok(response);
    }

    [HttpPost("receive_album_reward")]
    public async Task<DragaliaResult> ReceiveAlbumReward(MissionReceiveAlbumRewardRequest request)
    {
        MissionReceiveAlbumRewardData response = new();

        await this.missionService.RedeemMissions(MissionType.Album, request.album_mission_id_list);

        await BuildNormalResponse(response);
        response.update_data_list = await this.updateDataService.SaveChangesAsync();
        response.converted_entity_list = Enumerable.Empty<ConvertedEntityList>();
        response.entity_result = new()
        {
            converted_entity_list = Enumerable.Empty<ConvertedEntityList>()
        };

        return Ok(response);
    }

    [HttpPost("receive_memory_event_reward")]
    public async Task<DragaliaResult> ReceiveNormalReward(
        MissionReceiveMemoryEventRewardRequest request
    )
    {
        MissionReceiveMemoryEventRewardData response = new();

        await this.missionService.RedeemMissions(
            MissionType.MemoryEvent,
            request.memory_event_mission_id_list
        );

        await BuildNormalResponse(response);
        response.update_data_list = await this.updateDataService.SaveChangesAsync();
        response.converted_entity_list = Enumerable.Empty<ConvertedEntityList>();
        response.entity_result = new()
        {
            converted_entity_list = Enumerable.Empty<ConvertedEntityList>()
        };

        return Ok(response);
    }

    [HttpPost("receive_beginner_reward")]
    public async Task<DragaliaResult> ReceiveBeginnerReward(
        MissionReceiveBeginnerRewardRequest request
    )
    {
        MissionReceiveBeginnerRewardData response = new();

        await this.missionService.RedeemMissions(
            MissionType.Beginner,
            request.beginner_mission_id_list
        );

        await BuildNormalResponse(response);
        response.update_data_list = await this.updateDataService.SaveChangesAsync();
        response.converted_entity_list = Enumerable.Empty<ConvertedEntityList>();
        response.entity_result = new()
        {
            converted_entity_list = Enumerable.Empty<ConvertedEntityList>()
        };

        return Ok(response);
    }

    [HttpPost("receive_special_reward")]
    public async Task<DragaliaResult> ReceiveBeginnerReward(
        MissionReceiveSpecialRewardRequest request
    )
    {
        MissionReceiveSpecialRewardData response = new();

        await this.missionService.RedeemMissions(
            MissionType.Special,
            request.special_mission_id_list
        );

        await BuildNormalResponse(response);
        response.update_data_list = await this.updateDataService.SaveChangesAsync();
        response.converted_entity_list = Enumerable.Empty<ConvertedEntityList>();
        response.entity_result = new()
        {
            converted_entity_list = Enumerable.Empty<ConvertedEntityList>()
        };

        return Ok(response);
    }

    private async Task<INormalMissionEndpointResponse> BuildNormalResponse(
        INormalMissionEndpointResponse response
    )
    {
        ILookup<MissionType, DbPlayerMission> allMissions =
            await this.missionRepository.GetAllMissionsPerTypeAsync();

        response.album_mission_list = allMissions[MissionType.Album].Select(
            x => new AlbumMissionList(x.Id, x.Progress, (int)x.State, x.End, x.Start)
        );
        response.beginner_mission_list = allMissions[MissionType.Beginner].Select(
            x => new BeginnerMissionList(x.Id, x.Progress, (int)x.State, x.End, x.Start)
        );
        //response.daily_mission_list = allMissions[MissionType.Daily].Select(x => new DailyMissionList(x.Id, x.Progress, (int)x.State, x.End, x.Start));
        response.memory_event_mission_list = allMissions[MissionType.MemoryEvent].Select(
            x => new MemoryEventMissionList(x.Id, x.Progress, (int)x.State, x.End, x.Start)
        );
        response.main_story_mission_list = allMissions[MissionType.MainStory].Select(
            x => new MainStoryMissionList(x.Id, x.Progress, (int)x.State, x.End, x.Start)
        );
        response.normal_mission_list = allMissions[MissionType.Normal].Select(
            x => new NormalMissionList(x.Id, x.Progress, (int)x.State, x.End, x.Start)
        );
        response.period_mission_list = allMissions[MissionType.Period].Select(
            x => new PeriodMissionList(x.Id, x.Progress, (int)x.State, x.End, x.Start)
        );
        response.special_mission_list = allMissions[MissionType.Special].Select(
            x => new SpecialMissionList(x.Id, x.Progress, (int)x.State, x.End, x.Start)
        );
        return response;
    }
}
