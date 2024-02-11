using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Zena;

public record struct GetTeamDataResponse(
    string Name,
    Charas Unit1,
    Charas Unit2,
    Charas Unit3,
    Charas Unit4,
    Charas? Unit5,
    Charas? Unit6,
    Charas? Unit7,
    Charas? Unit8
);
