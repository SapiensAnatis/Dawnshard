using System.Collections.Generic;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Missions;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Missions;

public class MissionService : IMissionService
{
    private readonly ILogger<MissionService> logger;
    private readonly IMissionRepository missionRepository;

    private readonly IRewardService rewardService;

    public MissionService(
        ILogger<MissionService> logger,
        IMissionRepository missionRepository,
        IRewardService rewardService
    )
    {
        this.logger = logger;
        this.missionRepository = missionRepository;
        this.rewardService = rewardService;
    }

    public async Task<DbPlayerMission> StartMission(MissionType type, int id)
    {
        logger.LogInformation("Starting mission {missionId} ({missionType})", id, type);
        return await missionRepository.AddMission(id, type);
    }

    public async Task<IEnumerable<DbPlayerMission>> UnlockMainMissionGroup(int groupId)
    {
        IEnumerable<MainStoryMission> missions = MasterAsset.MainStoryMission.Enumerable
            .Where(x => x.MissionMainStoryGroupId == groupId)
            .ToList();

        logger.LogInformation(
            "Unlocking main story mission group {groupId} ({groupMissionIds})",
            groupId,
            missions.Select(x => x.Id)
        );

        List<DbPlayerMission> dbMissions = new();
        foreach (MainStoryMission mission in missions)
        {
            dbMissions.Add(
                await missionRepository.AddMission(
                    mission.Id,
                    MissionType.MainStory,
                    groupId: groupId
                )
            );
        }

        return dbMissions;
    }

    public async Task<IEnumerable<DbPlayerMission>> UnlockDrillMissionGroup(int groupId)
    {
        IEnumerable<DrillMission> missions = MasterAsset.DrillMission.Enumerable
            .Where(x => x.MissionDrillGroupId == groupId)
            .ToList();

        logger.LogInformation(
            "Unlocking drill story mission group {groupId} ({groupMissionIds})",
            groupId,
            missions.Select(x => x.Id)
        );

        List<DbPlayerMission> dbMissions = new();
        foreach (DrillMission mission in missions)
        {
            dbMissions.Add(
                await missionRepository.AddMission(mission.Id, MissionType.Drill, groupId: groupId)
            );
        }

        return dbMissions;
    }

    public async Task RedeemMission(int id)
    {
        logger.LogInformation("Redeeming mission {missionId}", id);

        DbPlayerMission dbMission = await missionRepository.GetMissionByIdAsync(id);
        if (dbMission.State != MissionState.Completed)
        {
            throw new DragaliaException(
                ResultCode.CommonUserStatusError,
                "Invalid mission state for redemption"
            );
        }

        Mission missionInfo = Mission.From(dbMission.Type, id);

        switch (missionInfo.Type)
        {
            case MissionType.Daily:
            case MissionType.MemoryEvent:
            case MissionType.Period:
                IExtendedRewardMission extendedRewardMission = (IExtendedRewardMission)
                    missionInfo.MasterAssetMission;
                await this.rewardService.GrantReward(
                    extendedRewardMission.EntityType,
                    extendedRewardMission.Id,
                    extendedRewardMission.EntityQuantity,
                    extendedRewardMission.EntityLimitBreakCount,
                    extendedRewardMission.EntityBuildupCount,
                    extendedRewardMission.EntityEquipableCount
                );
                break;
            default:
                await this.rewardService.GrantReward(
                    missionInfo.EntityType,
                    missionInfo.EntityId,
                    missionInfo.EntityQuantity
                );
                break;
        }

        dbMission.State = MissionState.Completed;
    }

    public async Task RedeemMissions(IEnumerable<int> ids)
    {
        foreach (int id in ids)
        {
            await RedeemMission(id);
        }
    }

    public async Task<IEnumerable<AtgenBuildEventRewardEntityList>> TryRedeemDrillMissionGroups(
        IEnumerable<int> groupIds
    )
    {
        List<AtgenBuildEventRewardEntityList> rewards = new();

        foreach (int groupId in groupIds)
        {
            if (
                await this.missionRepository
                    .GetMissionsByType(MissionType.Drill)
                    .CountAsync(x => x.GroupId == groupId && x.State == MissionState.InProgress)
                == 0
            )
            {
                DrillMissionGroup group = MasterAsset.DrillMissionGroup.Get(groupId);
                rewards.Add(
                    new AtgenBuildEventRewardEntityList(
                        group.UnlockEntityType1,
                        group.UnlockEntityId1,
                        group.UnlockEntityQuantity1
                    )
                );
                await this.rewardService.GrantReward(
                    group.UnlockEntityType1,
                    group.UnlockEntityId1,
                    group.UnlockEntityQuantity1
                );
            }
        }

        return rewards;
    }

    public async Task<CurrentMainStoryMission> GetCurrentMainStoryMission()
    {
        DbPlayerMission? firstMainStoryMission = await this.missionRepository
            .GetMissionsByType(MissionType.MainStory)
            .FirstOrDefaultAsync();
        if (firstMainStoryMission == null)
            return new CurrentMainStoryMission();

        MainStoryMission mission = MasterAsset.MainStoryMission.Get(firstMainStoryMission.Id);

        return new CurrentMainStoryMission()
        {
            main_story_mission_group_id = mission.MissionMainStoryGroupId,
            main_story_mission_state_list = (
                await this.missionRepository.GetMissionsByType(MissionType.MainStory).ToListAsync()
            )
                .Where(x => x.GroupId == mission.MissionMainStoryGroupId)
                .Select(
                    x =>
                        new AtgenMainStoryMissionStateList()
                        {
                            main_story_mission_id = x.Id,
                            state = (int)x.State,
                        }
                )
        };
    }

    public async Task<MissionNotice> GetMissionNotice(
        ILookup<MissionType, DbPlayerMission>? updatedLookup
    )
    {
        MissionNotice notice = new();

        async Task<AtgenNormalMissionNotice> BuildNotice(MissionType type)
        {
            if (updatedLookup == null)
                return await BuildMissionNotice(type, Array.Empty<int>());

            if (!updatedLookup.Contains(type))
                return new AtgenNormalMissionNotice();

            IEnumerable<DbPlayerMission> missions = updatedLookup[type].ToList();
            if (!missions.Any())
                return new AtgenNormalMissionNotice();

            return await BuildMissionNotice(
                type,
                missions.Where(x => x.State == MissionState.Completed).Select(x => x.Id)
            );
        }

        notice.main_story_mission_notice = await BuildNotice(MissionType.MainStory);
        notice.album_mission_notice = await BuildNotice(MissionType.Album);
        notice.beginner_mission_notice = await BuildNotice(MissionType.Beginner);
        notice.daily_mission_notice = await BuildNotice(MissionType.Daily);
        notice.drill_mission_notice = await BuildNotice(MissionType.Drill);
        notice.memory_event_mission_notice = await BuildNotice(MissionType.MemoryEvent);
        notice.normal_mission_notice = await BuildNotice(MissionType.Normal);
        notice.period_mission_notice = await BuildNotice(MissionType.Period);
        notice.special_mission_notice = await BuildNotice(MissionType.Special);

        return notice;
    }

    private async Task<AtgenNormalMissionNotice> BuildMissionNotice(
        MissionType type,
        IEnumerable<int> newCompletedMissionList
    )
    {
        List<DbPlayerMission> allMissions = await this.missionRepository
            .GetMissionsByType(type)
            .ToListAsync();

        int totalCount = allMissions.Count;
        int completedCount = allMissions.Count(x => x.State == MissionState.Completed);
        int receivableRewardCount = allMissions.Count(x => x.State == MissionState.Receivable);

        return new AtgenNormalMissionNotice
        {
            is_update = 1,
            all_mission_count = totalCount,
            completed_mission_count = completedCount,
            receivable_reward_count = receivableRewardCount,
            new_complete_mission_id_list = newCompletedMissionList,
            pickup_mission_count = type == MissionType.Daily ? allMissions.Count(x => x.Pickup) : 0,
            current_mission_id =
                type == MissionType.Drill
                    ? allMissions.FirstOrDefault(x => x.State == MissionState.InProgress)?.Id
                        ?? 100100
                    : 0
        };
    }
}
