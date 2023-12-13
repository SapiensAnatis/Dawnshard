using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.MasterAsset.Models.Missions;

namespace DragaliaAPI.Features.Missions;

public interface IMissionService
{
    Task<DbPlayerMission> StartMission(
        MissionType type,
        int id,
        int groupId = 0,
        DateTimeOffset? startTime = null,
        DateTimeOffset? endTime = null
    );

    Task<(
        IEnumerable<MainStoryMissionGroupReward>,
        IEnumerable<DbPlayerMission>
    )> UnlockMainMissionGroup(int groupId);
    Task<IEnumerable<DbPlayerMission>> UnlockDrillMissionGroup(int groupId);

    Task RedeemMission(MissionType type, int id);
    Task RedeemMissions(MissionType type, IEnumerable<int> ids);

    Task<IEnumerable<AtgenBuildEventRewardEntityList>> TryRedeemDrillMissionGroups(
        IEnumerable<int> groupIds
    );

    Task<CurrentMainStoryMission> GetCurrentMainStoryMission();
    Task<MissionNotice> GetMissionNotice(ILookup<MissionType, DbPlayerMission>? updatedLookup);
    Task<IEnumerable<QuestEntryConditionList>> GetEntryConditions();
    Task<IEnumerable<DrillMissionGroupList>> GetCompletedDrillGroups();
    Task<IEnumerable<DbPlayerMission>> UnlockMemoryEventMissions(int eventId);
    Task<IEnumerable<DbPlayerMission>> UnlockEventMissions(int eventId);

    Task<TResponse> BuildNormalResponse<TResponse>()
        where TResponse : INormalMissionEndpointResponse, new();

    Task RedeemDailyMissions(IEnumerable<AtgenMissionParamsList> missions);
}
