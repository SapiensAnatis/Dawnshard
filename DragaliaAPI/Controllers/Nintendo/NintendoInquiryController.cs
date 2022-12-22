/*using Microsoft.AspNetCore.Mvc;
using DragaliaAPI.Services;
using DragaliaAPI.Database;

namespace DragaliaAPI.Controllers.Nintendo;

[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class NintendoInquiryController : DragaliaControllerBase
{
    private const string MarketGoogle = "GOOGLE";
    private const string MarketIOS = "IOSMAYBEDUNNO";

    private readonly ISessionService _sessionService;
    private readonly ApiContext _apiContext;

    public NintendoInquiryController(ISessionService sessionService, ApiContext apiContext)
    {
        _sessionService = sessionService;
        _apiContext = apiContext;
    }

    [Route("/inquiry/v1/users/{userId=0}")]
    [HttpGet]
    public async Task<DragaliaResult> GetDiamantium(
        [FromRoute(Name = "userId")] string userId,
        [FromHeader(Name = "Authorization")] string auth
    )
    {
        if (
            userId.Equals("0")
            || string.IsNullOrWhiteSpace(auth)
            || !auth.TrimStart().StartsWith("bearer ", true, null)
        )
        {
            return BadRequest();
        }
        string idToken = auth.Substring("bearer ".Length);
        if (!idToken.Any())
        {
            return BadRequest();
        }
        string deviceAccountId = "";
        try
        {
            deviceAccountId = await _sessionService.GetDeviceAccountId_IdToken(idToken);
        }
        catch (Exception)
        {
            return Unauthorized();
        }
        if (!string.Equals(userId, deviceAccountId))
        {
            return Unauthorized();
        }
        //TODO: get has unread cs comment
        bool hasUnreadCsComment = true;
        return Ok(
            new InquiryResponse(new InquiryData(userId, hasUnreadCsComment, DateTimeOffset.UtcNow))
        );
    }
}
*/
