using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.MasterAsset.Models.TimeAttack;

namespace DragaliaAPI.Features.TimeAttack;

public interface ITimeAttackService
{
    bool GetIsRankedQuest(int questId);
    IEnumerable<RankingTierReward> GetRewards();
    Task<IEnumerable<RankingTierReward>> ReceiveTierReward(int questId);
    Task RegisterRankedClear(string gameId, float clearTime);
    Task<bool> SetupRankedClear(int questId, PartyInfo partyInfo);
}
