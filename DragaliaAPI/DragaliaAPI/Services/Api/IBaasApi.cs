using DragaliaAPI.Models.Generated;
using Microsoft.IdentityModel.Tokens;

namespace DragaliaAPI.Services.Api;

public interface IBaasApi
{
    Task<LoadIndexResponse> GetSavefile(string gameIdToken);
    Task<string?> GetUserId(string webIdToken);
    Task<string?> GetUsername(string gameIdToken);
}
