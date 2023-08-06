using System.Text.Json.Serialization;
using DragaliaAPI.Photon.Shared.Enums;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Json;

namespace DragaliaAPI.Shared.MasterAsset.Models.Wall;

public record QuestWallMonthlyReward(
    int Id,
    int TotalWallLevel,
    int RewardStageNum,
    string RewardImage,
    bool IsShowBadge,
    EntityTypes RewardEntityType,
    int RewardEntityId,
    int RewardEntityQuantity,
    string ReceiveStartDate,
    string ReceiveEndDate
)
{

}
