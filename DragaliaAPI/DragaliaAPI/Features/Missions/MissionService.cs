using System.Collections.Frozen;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Features.Missions.InitialProgress;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Features.Shared.Options;
using DragaliaAPI.Helpers;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Missions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DragaliaAPI.Features.Missions;

public class MissionService(
    ILogger<MissionService> logger,
    IMissionRepository missionRepository,
    IRewardService rewardService,
    IMissionInitialProgressionService missionInitialProgressionService,
    IUserDataRepository userDataRepository,
    IResetHelper resetHelper,
    IOptionsMonitor<EventOptions> eventOptionsMonitor
) : IMissionService
{
    private static readonly FrozenSet<int> ObsoleteMemoryEventMissions = new[]
    {
        10020301, // The Miracle of Dragonyule: Collect 100 Holiday Cheer in One Go. Rewards a V1 weapon.
        10020701, // The Miracle of Dragonyule: Clear Three Challenge Battles. Rewards a V1 weapon.
    }.ToFrozenSet();

    private readonly ILogger<MissionService> logger = logger;
    private readonly IMissionRepository missionRepository = missionRepository;
    private readonly IMissionInitialProgressionService missionInitialProgressionService =
        missionInitialProgressionService;
    private readonly IUserDataRepository userDataRepository = userDataRepository;
    private readonly IRewardService rewardService = rewardService;
    private readonly IResetHelper resetHelper = resetHelper;
    private readonly EventOptions eventOptions = eventOptionsMonitor.CurrentValue;

    public async Task<DbPlayerMission> StartMission(
        MissionType type,
        int id,
        int groupId = 0,
        DateTimeOffset? startTime = null,
        DateTimeOffset? endTime = null
    )
    {
        logger.LogInformation("Starting mission {missionId} ({missionType})", id, type);

        DbPlayerMission mission = await this.missionInitialProgressionService.StartMission(
            type,
            id,
            startTime: startTime,
            endTime: endTime,
            groupId: groupId
        );

        return mission;
    }

    public DbPlayerMission AddCompletedMission(
        MissionType type,
        int id,
        int groupId = 0,
        DateTimeOffset? startTime = null,
        DateTimeOffset? endTime = null
    )
    {
        logger.LogInformation("Adding completed mission {missionId} ({missionType})", id, type);
        Mission missionInfo = Mission.From(type, id);

        DbPlayerMission dbMission = missionRepository.AddMission(
            type,
            id,
            progress: missionInfo.CompleteValue,
            state: MissionState.Completed,
            startTime: startTime,
            endTime: endTime,
            groupId: groupId
        );

        return dbMission;
    }

    public async Task<(
        IEnumerable<MainStoryMissionGroupReward>,
        IEnumerable<DbPlayerMission>
    )> UnlockMainMissionGroup(int groupId)
    {
        IEnumerable<MainStoryMission> missions = MasterAsset
            .MainStoryMission.Enumerable.Where(x => x.MissionMainStoryGroupId == groupId)
            .ToList();

        List<MainStoryMissionGroupReward> rewards = MasterAsset
            .MainStoryMissionGroupRewards.Get(groupId)
            .Rewards.ToList();

        logger.LogInformation(
            "Unlocking main story mission group {groupId} ({groupMissionIds})",
            groupId,
            missions.Select(x => x.Id)
        );

        List<DbPlayerMission> dbMissions = new();
        foreach (MainStoryMission mission in missions)
        {
            dbMissions.Add(
                await this.StartMission(MissionType.MainStory, mission.Id, groupId: groupId)
            );
        }

        if (rewards.Count > 0)
        {
            foreach (MainStoryMissionGroupReward reward in rewards)
            {
                await this.rewardService.GrantReward(new(reward.Type, reward.Id, reward.Quantity));
            }
        }

        return (rewards, dbMissions);
    }

    public async Task<IEnumerable<DbPlayerMission>> UnlockDrillMissionGroup(int groupId)
    {
        IEnumerable<DrillMission> missions = MasterAsset
            .DrillMission.Enumerable.Where(x => x.MissionDrillGroupId == groupId)
            .ToList();

        if (
            await this.missionRepository.Missions.AnyAsync(x =>
                x.GroupId == groupId && x.Type == MissionType.Drill
            )
        )
            return await this
                .missionRepository.Missions.Where(x =>
                    x.GroupId == groupId && x.Type == MissionType.Drill
                )
                .ToListAsync();

        logger.LogInformation(
            "Unlocking drill story mission group {groupId} ({groupMissionIds})",
            groupId,
            missions.Select(x => x.Id)
        );

        List<DbPlayerMission> dbMissions = new();
        foreach (DrillMission mission in missions)
        {
            dbMissions.Add(
                await this.StartMission(MissionType.Drill, mission.Id, groupId: groupId)
            );
        }

        return dbMissions;
    }

    public async Task<IEnumerable<DbPlayerMission>> UnlockMemoryEventMissions(int eventId)
    {
        IEnumerable<MemoryEventMission> missions = MasterAsset
            .MemoryEventMission.Enumerable.Where(x => x.EventId == eventId)
            .ExceptBy(ObsoleteMemoryEventMissions, mission => mission.Id)
            .ToList();

        if (
            await this.missionRepository.Missions.AnyAsync(x =>
                x.GroupId == eventId && x.Type == MissionType.MemoryEvent
            )
        )
            return await this
                .missionRepository.Missions.Where(x =>
                    x.GroupId == eventId && x.Type == MissionType.MemoryEvent
                )
                .ToListAsync();

        logger.LogInformation(
            "Unlocking memory event mission group {eventId} ({groupMissionIds})",
            eventId,
            missions.Select(x => x.Id)
        );

        List<DbPlayerMission> dbMissions = new();
        foreach (MemoryEventMission mission in missions)
        {
            dbMissions.Add(
                await this.StartMission(
                    MissionType.MemoryEvent,
                    mission.Id,
                    groupId: mission.EventId
                )
            );
        }

        return dbMissions;
    }

    public async Task<IEnumerable<DbPlayerMission>> UnlockEventMissions(int eventId)
    {
        EventRunInformation runInfo =
            this.eventOptions.EventList.FirstOrDefault(x => x.Id == eventId)
            ?? throw new DragaliaException(
                ResultCode.EventOutThePeriod,
                "Attempted to start missions for an event without any configuration in appsettings.json"
            );

        this.logger.LogDebug("Starting missions for event with run info {@info}", runInfo);

        if (
            this.resetHelper.LastDailyReset < runInfo.Start
            || this.resetHelper.LastDailyReset > runInfo.End
        )
        {
            throw new DragaliaException(
                ResultCode.EventOutThePeriod,
                "Attempted to start missions for an event which has either not started or has ended."
            );
        }

        List<PeriodMission> periodMissions = MasterAsset
            .PeriodMission.Enumerable.Where(x => x.QuestGroupId == eventId)
            .ToList();

        List<DailyMission> dailyMissions = MasterAsset
            .DailyMission.Enumerable.Where(x => x.QuestGroupId == eventId)
            .ToList();

        logger.LogInformation("Unlocking event missions for event {eventId}", eventId);

        List<DbPlayerMission> dbMissions = new(periodMissions.Count + dailyMissions.Count);
        foreach (PeriodMission mission in periodMissions)
        {
            dbMissions.Add(
                await this.StartMission(
                    MissionType.Period,
                    mission.Id,
                    groupId: mission.QuestGroupId,
                    startTime: runInfo.Start,
                    endTime: runInfo.End
                )
            );
        }
        foreach (DailyMission mission in dailyMissions)
        {
            dbMissions.Add(
                await this.StartMission(
                    MissionType.Daily,
                    mission.Id,
                    groupId: mission.QuestGroupId,
                    startTime: this.resetHelper.LastDailyReset,
                    endTime: this.resetHelper.LastDailyReset.AddDays(1)
                )
            );
        }

        return dbMissions;
    }

    public async Task RedeemMission(MissionType type, int id)
    {
        logger.LogInformation("Redeeming mission {missionId}", id);

        DbPlayerMission dbMission = await missionRepository.GetMissionByIdAsync(type, id);
        if (dbMission.State != MissionState.Completed)
        {
            throw new DragaliaException(
                ResultCode.CommonUserStatusError,
                "Invalid mission state for redemption"
            );
        }

        await this.GrantMissionReward(type, id);

        dbMission.State = MissionState.Claimed;
    }

    private async Task GrantMissionReward(MissionType type, int id)
    {
        Mission missionInfo = Mission.From(type, id);

        switch (missionInfo.Type)
        {
            case MissionType.Daily:
            case MissionType.MemoryEvent:
            case MissionType.Period:
                IExtendedRewardMission extendedRewardMission = (IExtendedRewardMission)
                    missionInfo.MasterAssetMission;

                // These fields are always 0 (invalid) for everything but the FEH wyrmprints
                int buildupCount = Math.Max(extendedRewardMission.EntityBuildupCount, 1);
                int equipableCount = Math.Max(extendedRewardMission.EntityEquipableCount, 1);

                await this.rewardService.GrantReward(
                    new(
                        extendedRewardMission.EntityType,
                        extendedRewardMission.EntityId,
                        extendedRewardMission.EntityQuantity,
                        extendedRewardMission.EntityLimitBreakCount,
                        buildupCount,
                        equipableCount
                    )
                );
                break;
            default:
                await this.rewardService.GrantReward(
                    new Entity(
                        missionInfo.EntityType,
                        missionInfo.EntityId,
                        missionInfo.EntityQuantity
                    )
                );
                break;
        }
    }

    public async Task RedeemMissions(MissionType type, IEnumerable<int> ids)
    {
        foreach (int id in ids)
        {
            await RedeemMission(type, id);
        }
    }

    public async Task RedeemDailyMissions(IEnumerable<AtgenMissionParamsList> missions)
    {
        this.logger.LogDebug("Claiming daily missions: {@missions}", missions);

        int[] ids = missions.Select(x => x.DailyMissionId).ToArray();

        List<DbCompletedDailyMission> completed = await this
            .missionRepository.CompletedDailyMissions.Where(x => ids.Contains(x.Id))
            .ToListAsync();

        List<DbPlayerMission> regularMissions = await this
            .missionRepository.Missions.Where(x =>
                x.Type == MissionType.Daily && ids.Contains(x.Id)
            )
            .ToListAsync();

        foreach (AtgenMissionParamsList claimRequest in missions)
        {
            if (
                claimRequest.DayNo
                == DateOnly.FromDateTime(this.resetHelper.LastDailyReset.UtcDateTime)
            )
            {
                DbPlayerMission regularMission = regularMissions.First(x =>
                    x.Id == claimRequest.DailyMissionId
                );

                regularMission.State = MissionState.Claimed;
            }

            DbCompletedDailyMission? dbMission = completed.FirstOrDefault(x =>
                x.Id == claimRequest.DailyMissionId && x.Date == claimRequest.DayNo
            );

            if (dbMission is null)
            {
                // throw new DragaliaException(
                //     ResultCode.MissionIdNotFound,
                //     "Tried to claim non-existent daily mission"
                // );
                continue;
            }

            await this.GrantMissionReward(MissionType.Daily, claimRequest.DailyMissionId);

            completed.Remove(dbMission);
            missionRepository.RemoveCompletedDailyMission(dbMission);
        }
    }

    public async Task<IEnumerable<DrillMissionGroupList>> GetCompletedDrillGroups()
    {
        List<int> completedGroups = [];

        int currentMission = await this
            .missionRepository.GetMissionsByType(MissionType.Drill)
            .Where(x => x.State == MissionState.Claimed)
            .OrderByDescending(x => x.Id)
            .Select(x => x.Id)
            .FirstOrDefaultAsync();

        if (currentMission == 0)
            return [];

        IEnumerable<(int Group, int MaxId)> maxIds = MasterAsset
            .DrillMission.Enumerable.GroupBy(x => x.MissionDrillGroupId)
            .Select(group => (group.Key, group.Max(x => x.Id)));

        foreach ((int group, int maxId) in maxIds)
            if (currentMission >= maxId)
                completedGroups.Add(group);

        this.logger.LogDebug("Returning completed drill groups: {groups}", completedGroups);

        return completedGroups.Select(x => new DrillMissionGroupList(x));
    }

    public async Task<IEnumerable<AtgenBuildEventRewardEntityList>> TryRedeemDrillMissionGroups(
        IEnumerable<int> groupIds
    )
    {
        List<AtgenBuildEventRewardEntityList> rewards = new();

        foreach (int groupId in groupIds)
        {
            if (
                await this
                    .missionRepository.GetMissionsByType(MissionType.Drill)
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
                    new Entity(
                        group.UnlockEntityType1,
                        group.UnlockEntityId1,
                        group.UnlockEntityQuantity1
                    )
                );
            }
        }

        return rewards;
    }

    public async Task<CurrentMainStoryMission> GetCurrentMainStoryMission()
    {
        int? mainStoryMissionGroupId = await this
            .missionRepository.GetMissionsByType(MissionType.MainStory)
            .MaxAsync(x => x.GroupId);

        if (mainStoryMissionGroupId == null)
            return new CurrentMainStoryMission();

        return new CurrentMainStoryMission()
        {
            MainStoryMissionGroupId = mainStoryMissionGroupId.Value,
            MainStoryMissionStateList = (
                await this.missionRepository.GetMissionsByType(MissionType.MainStory).ToListAsync()
            )
                .Where(x => x.GroupId == mainStoryMissionGroupId.Value)
                .Select(x => new AtgenMainStoryMissionStateList()
                {
                    MainStoryMissionId = x.Id,
                    State = (int)x.State,
                })
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

        notice.MainStoryMissionNotice = await BuildNotice(MissionType.MainStory);
        notice.AlbumMissionNotice = await BuildNotice(MissionType.Album);
        notice.BeginnerMissionNotice = await BuildNotice(MissionType.Beginner);
        notice.DailyMissionNotice = await BuildNotice(MissionType.Daily);
        notice.DrillMissionNotice = await BuildNotice(MissionType.Drill);
        notice.MemoryEventMissionNotice = await BuildNotice(MissionType.MemoryEvent);
        notice.NormalMissionNotice = await BuildNotice(MissionType.Normal);
        notice.PeriodMissionNotice = await BuildNotice(MissionType.Period);
        notice.SpecialMissionNotice = await BuildNotice(MissionType.Special);

        return notice;
    }

    private async Task<AtgenNormalMissionNotice> BuildMissionNotice(
        MissionType type,
        IEnumerable<int> newCompletedMissionList
    )
    {
        List<DbPlayerMission> allMissions = await this
            .missionRepository.GetMissionsByType(type)
            .ToListAsync();

        int totalCount = allMissions.Count;
        int completedCount = allMissions.Count(x => x.State >= MissionState.Completed);
        int receivableRewardCount = allMissions.Count(x => x.State == MissionState.Completed);

        int currentMissionId = 0;

        if (type == MissionType.Drill)
        {
            DbPlayerMission? activeMission = allMissions
                .OrderBy(x => x.Id)
                .FirstOrDefault(x => x.State < MissionState.Claimed);

            if (activeMission != null)
            {
                currentMissionId = activeMission.Id;
            }
            else if (allMissions.Count == 0)
            {
                currentMissionId = 100100;
            }
            else if (completedCount < MasterAsset.DrillMission.Count)
            {
                // Prevent icon from disappearing while waiting to start the next group
                receivableRewardCount = 1;
            }
        }

        return new AtgenNormalMissionNotice
        {
            IsUpdate = true,
            AllMissionCount = totalCount,
            CompletedMissionCount = completedCount,
            ReceivableRewardCount = receivableRewardCount,
            NewCompleteMissionIdList = newCompletedMissionList,
            PickupMissionCount = type == MissionType.Daily ? allMissions.Count(x => x.Pickup) : 0,
            CurrentMissionId = currentMissionId
        };
    }

    public async Task<IEnumerable<QuestEntryConditionList>> GetEntryConditions()
    {
        List<DbPlayerMission> mainMissions = await this
            .missionRepository.GetMissionsByType(MissionType.MainStory)
            .ToListAsync();
        ILookup<int, DbPlayerMission> groupedMissions = mainMissions
            .Where(x => x.GroupId is not null)
            .ToLookup(x => x.GroupId!.Value);

        return groupedMissions
            .Where(x => x.All(y => y.State == MissionState.Claimed))
            .Select(x => x.Key)
            .Distinct()
            .Select(x => new QuestEntryConditionList(x));
    }

    public async Task<TResponse> BuildNormalResponse<TResponse>()
        where TResponse : INormalMissionEndpointResponse, new()
    {
        ILookup<MissionType, DbPlayerMission> allMissions =
            await this.missionRepository.GetActiveMissionsPerTypeAsync();

        int activeEventId = await this
            .userDataRepository.UserData.Select(x => x.ActiveMemoryEventId)
            .FirstAsync();

        TResponse response =
            new()
            {
                AlbumMissionList = allMissions[MissionType.Album]
                    .Select(x => new AlbumMissionList(
                        x.Id,
                        x.Progress,
                        (int)x.State,
                        x.End,
                        x.Start
                    )),
                BeginnerMissionList = allMissions[MissionType.Beginner]
                    .Select(x => new BeginnerMissionList(
                        x.Id,
                        x.Progress,
                        (int)x.State,
                        x.End,
                        x.Start
                    )),
                MainStoryMissionList = allMissions[MissionType.MainStory]
                    .Select(x => new MainStoryMissionList(
                        x.Id,
                        x.Progress,
                        (int)x.State,
                        x.End,
                        x.Start
                    )),
                NormalMissionList = allMissions[MissionType.Normal]
                    .Select(x => new NormalMissionList(
                        x.Id,
                        x.Progress,
                        (int)x.State,
                        x.End,
                        x.Start
                    )),
                PeriodMissionList = allMissions[MissionType.Period]
                    .Select(x => new PeriodMissionList(
                        x.Id,
                        x.Progress,
                        (int)x.State,
                        x.End,
                        x.Start
                    )),
                SpecialMissionList = allMissions[MissionType.Special]
                    .Select(x => new SpecialMissionList(
                        x.Id,
                        x.Progress,
                        (int)x.State,
                        x.End,
                        x.Start
                    )),
                MemoryEventMissionList = allMissions[MissionType.MemoryEvent]
                    .Where(x => x.GroupId == activeEventId)
                    .Select(x => new MemoryEventMissionList(
                        x.Id,
                        x.Progress,
                        (int)x.State,
                        x.End,
                        x.Start
                    )),
            };

        List<DailyMissionList> historicalDailyMissions = await this.GetHistoricalDailyMissions();
        IEnumerable<DailyMissionList> currentDailyMissions = allMissions[MissionType.Daily]
            .Select(x => new DailyMissionList()
            {
                DailyMissionId = x.Id,
                Progress = x.Progress,
                State = x.State,
                StartDate = x.Start,
                EndDate = x.End,
                DayNo = DateOnly.FromDateTime(this.resetHelper.LastDailyReset.UtcDateTime)
            });

        response.DailyMissionList = currentDailyMissions.UnionBy(
            historicalDailyMissions,
            x => new { daily_mission_id = x.DailyMissionId, day_no = x.DayNo }
        );

        return response;
    }

    private Task<List<DailyMissionList>> GetHistoricalDailyMissions() =>
        this
            .missionRepository.CompletedDailyMissions.Select(x => new DailyMissionList()
            {
                DailyMissionId = x.Id,
                Progress = x.Progress,
                State = MissionState.Completed,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                DayNo = x.Date
            })
            .ToListAsync();
}
