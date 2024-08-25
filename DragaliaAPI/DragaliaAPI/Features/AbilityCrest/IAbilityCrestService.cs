using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.AbilityCrest;

public interface IAbilityCrestService
{
    Task<ResultCode> TryBuildup(
        DragaliaAPI.Shared.MasterAsset.Models.AbilityCrest abilityCrest,
        AtgenBuildupAbilityCrestPieceList buildup
    );

    Task<ResultCode> TryBuildupAugments(
        DragaliaAPI.Shared.MasterAsset.Models.AbilityCrest abilityCrest,
        AtgenPlusCountParamsList buildup
    );

    Task<ResultCode> TryResetAugments(
        DragaliaAPI.Shared.Definitions.Enums.AbilityCrests abilityCrestId,
        PlusCountType augmentType
    );
}
