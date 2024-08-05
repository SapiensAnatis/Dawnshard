using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.Event.Summon;

internal static class EventSummonLogic
{
    public readonly record struct BoxSummonInfo(
        int RemainingQuantity,
        bool ResetPossible,
        List<AtgenBoxSummonDetail> DetailList
    );

    public static BoxSummonInfo GetBoxSummonInfo(
        int boxNum,
        Dictionary<int, int> idSummonQuantityDict,
        IReadOnlyDictionary<int, EventSummonItemConfiguration> itemConfigDict
    )
    {
        int totalRemainingItemCount = 0;
        bool resetPossible = true;
        List<AtgenBoxSummonDetail> detailList = new(itemConfigDict.Count);

        foreach ((int id, EventSummonItemConfiguration itemConfig) in itemConfigDict)
        {
            int totalItemCount = itemConfig.GetTotalCountInBox(boxNum);
            int remainingItemCount = totalItemCount;

            if (idSummonQuantityDict.TryGetValue(id, out int summonedCount))
            {
                remainingItemCount = Math.Max(0, totalItemCount - summonedCount);
            }

            totalRemainingItemCount += remainingItemCount;

            bool blocksReset = itemConfig.ResetItemFlag && remainingItemCount > 0;
            if (blocksReset)
            {
                resetPossible = false;
            }

            detailList.Add(
                new()
                {
                    EntityId = itemConfig.EntityId,
                    EntityQuantity = itemConfig.EntityQuantity,
                    EntityType = itemConfig.EntityType,
                    PickupItemState = itemConfig.PickupItemState,
                    ResetItemFlag = itemConfig.ResetItemFlag,
                    TwoStepId = itemConfig.TwoStepId,
                    TotalCount = totalItemCount,
                    Limit = remainingItemCount
                }
            );
        }

        return new BoxSummonInfo(totalRemainingItemCount, resetPossible, detailList);
    }
}
