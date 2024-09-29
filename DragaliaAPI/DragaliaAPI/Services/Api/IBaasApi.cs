using DragaliaAPI.Models.Generated;
using Microsoft.IdentityModel.Tokens;

namespace DragaliaAPI.Services.Api;

public interface IBaasApi
{
    Task<IList<SecurityKey>> GetKeys();
    Task<LoadIndexResponse> GetSavefile(string idToken);
    Task<string?> GetUserId(string idToken);
    Task<string?> GetUsername(string idToken);
}
