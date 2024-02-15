namespace DragaliaAPI.Shared.MasterAsset.Models.Missions;

public interface IExtendedRewardMission : IMission
{
    int EntityBuildupCount { get; }
    int EntityLimitBreakCount { get; }
    int EntityEquipableCount { get; }
}
