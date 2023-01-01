using Microsoft.IdentityModel.Tokens;

namespace DragaliaAPI.Services.Helpers;

public interface IBaasRequestHelper
{
    Task<IList<SecurityKey>> GetKeys();
}
