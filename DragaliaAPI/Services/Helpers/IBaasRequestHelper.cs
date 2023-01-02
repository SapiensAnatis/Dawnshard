using DragaliaAPI.Models.Generated;
using Microsoft.IdentityModel.Tokens;

namespace DragaliaAPI.Services.Helpers;

public interface IBaasRequestHelper
{
    Task<IList<SecurityKey>> GetKeys();
    Task<LoadIndexData> GetSavefile(string idToken);
}
