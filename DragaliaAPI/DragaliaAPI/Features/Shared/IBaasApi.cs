using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.Shared;

public interface IBaasApi
{
    Task<LoadIndexResponse> GetSavefile(string gameIdToken);
    Task<string?> GetUserId(string webIdToken);
    Task<string?> GetUsername(string gameIdToken);
}
