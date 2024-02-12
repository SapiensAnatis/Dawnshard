using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Zena;

public class GetTeamDataResponse
{
    public required string Name { get; set; }
    public Charas Unit1 { get; set; }
    public Charas Unit2 { get; set; }
    public Charas Unit3 { get; set; }
    public Charas Unit4 { get; set; }
    public Charas? Unit5 { get; set; }
    public Charas? Unit6 { get; set; }
    public Charas? Unit7 { get; set; }
    public Charas? Unit8 { get; set; }
}
