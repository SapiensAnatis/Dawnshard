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
            new EulaAgreeAgreeResponse()
            {
                IsOptin = false,
                VersionHash = new()
                {
                    Region = request.Region,
                    Lang = request.Lang,
                    EulaVersion = request.EulaVersion,
                    PrivacyPolicyVersion = request.PrivacyPolicyVersion
                }
            }
        );
    }
}
