using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.Missions;

public interface IMissionService
{
    Task<DbPlayerMission> StartMission(MissionType type, int id);

    Task<IEnumerable<DbPlayerMission>> UnlockMainMissionGroup(int groupId);
    Task<IEnumerable<DbPlayerMission>> UnlockDrillMissionGroup(int groupId);

    Task RedeemMission(int id);
    Task RedeemMissions(IEnumerable<int> ids);

    Task<IEnumerable<AtgenBuildEventRewardEntityList>> TryRedeemDrillMissionGroups(IEnumerable<int> groupIds);

    Task<CurrentMainStoryMission> GetCurrentMainStoryMission();
    Task<MissionNotice> GetMissionNotice(ILookup<MissionType, DbPlayerMission>? updatedLookup);
}