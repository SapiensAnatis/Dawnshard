namespace DragaliaAPI.Features.Zena;

public interface IZenaService
{
    Task<GetTeamDataResponse?> GetTeamData(IEnumerable<int> partyNumbers);
}
