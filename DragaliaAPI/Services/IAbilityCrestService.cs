using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset.Models;

namespace DragaliaAPI.Services;

public interface IAbilityCrestService
{
    Task AddOrRefund(AbilityCrests abilityCrestId);
    Task<ResultCode> TryBuildup(
        AbilityCrest abilityCrest,
        AtgenBuildupAbilityCrestPieceList buildup
    );

    Task<ResultCode> TryBuildupAugments(
        AbilityCrest abilityCrest,
        AtgenPlusCountParamsList buildup
    );

    Task<ResultCode> TryResetAugments(AbilityCrests abilityCrestId, PlusCountType augmentType);
}
