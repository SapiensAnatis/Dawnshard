using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Features.Presents;

namespace DragaliaAPI.Features.Present;

/// <summary>
/// Entity used to create presents in code.
/// </summary>
/// <param name="MessageId">The message ID.</param>
/// <param name="EntityType">The entity type.</param>
/// <param name="EntityId">The entity ID.</param>
/// <param name="EntityQuantity">The entity quantity.</param>
/// <param name="EntityLevel">The entity level.</param>
/// <param name="EntityLimitBreakCount">The entity unbind count.</param>
public record Present(
    PresentMessage MessageId,
    EntityTypes EntityType,
    int EntityId,
    int EntityQuantity = 1,
    int EntityLevel = 1,
    int EntityLimitBreakCount = 0
)
{
    public DateTimeOffset CreateTime { get; } = DateTimeOffset.UtcNow;

    public TimeSpan? ExpiryTime { get; init; }

    public IEnumerable<int> MessageParamValues { get; init; } = [];

    public DbPlayerPresent ToEntity(long viewerId)
    {
        return new()
        {
            ViewerId = viewerId,
            EntityType = this.EntityType,
            EntityId = this.EntityId,
            EntityQuantity = this.EntityQuantity,
            EntityLevel = this.EntityLevel,
            EntityLimitBreakCount = this.EntityLimitBreakCount,
            MessageId = this.MessageId,
            MessageParamValue1 = this.MessageParamValues.ElementAtOrDefault(0),
            MessageParamValue2 = this.MessageParamValues.ElementAtOrDefault(1),
            MessageParamValue3 = this.MessageParamValues.ElementAtOrDefault(2),
            MessageParamValue4 = this.MessageParamValues.ElementAtOrDefault(3),
            CreateTime = this.CreateTime,
            ReceiveLimitTime = CreateTime + ExpiryTime
        };
    }

    public AtgenBuildEventRewardEntityList ToBuildEventRewardList()
    {
        return new()
        {
            EntityId = this.EntityId,
            EntityType = this.EntityType,
            EntityQuantity = this.EntityQuantity
        };
    }
}
