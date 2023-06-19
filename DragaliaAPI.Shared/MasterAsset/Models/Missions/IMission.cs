using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models.Missions;

public interface IMission
{
    public int Id { get; }
    public EntityTypes EntityType { get; }
    public int EntityId { get; }
    public int EntityQuantity { get; }
    public int CompleteValue { get; }
}
