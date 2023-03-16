using DragaliaAPI.Models.Generated;
using Microsoft.IdentityModel.Tokens;

namespace DragaliaAPI.Services.Helpers;

public interface IBaasApi
{
    Task<IList<SecurityKey>> GetKeys();
    Task<LoadIndexData> GetSavefile(string idToken);
}
