using DragaliaAPI.DTO;
using DragaliaAPI.Infrastructure.Results;
using DragaliaAPI.Shared.Enums;
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
