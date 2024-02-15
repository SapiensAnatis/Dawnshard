using DragaliaAPI.Models.Generated;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("eula_agree")]
public class EulaAgreeController : DragaliaControllerBase
{
    [HttpPost("agree")]
    public DragaliaResult Agree(EulaAgreeAgreeRequest request)
    {
        return this.Ok(
            new EulaAgreeAgreeData()
            {
                is_optin = 0,
                version_hash = new()
                {
                    region = request.region,
                    lang = request.lang,
                    eula_version = request.eula_version,
                    privacy_policy_version = request.privacy_policy_version
                }
            }
        );
    }
}
