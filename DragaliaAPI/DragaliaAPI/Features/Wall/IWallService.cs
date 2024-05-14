using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Wall;

public interface IWallService
{
    Task LevelupQuestWall(int wallId);

    Task SetQuestWallIsStartNextLevel(int wallId, bool value);

    Task<int> GetTotalWallLevel();

    Task GrantMonthlyRewardEntityList(IList<AtgenBuildEventRewardEntityList> rewards);

    List<AtgenBuildEventRewardEntityList> GetMonthlyRewardEntityList(int levelTotal);

    public Task<AtgenUserWallRewardList> GetUserWallRewardList();

    Task InitializeWall();
    Task<Dictionary<QuestWallTypes, int>> GetWallLevelMap();
    Task<DbWallRewardDate> GetLastRewardDate();
    Task<bool> CheckWallInitialized();
    bool CheckCanClaimReward(DateTimeOffset lastClaimDate);
    Task<DbPlayerQuestWall> GetQuestWall(int wallId);
}
