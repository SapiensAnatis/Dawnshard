using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DragaliaAPI.Models.Dragalia.Requests;
using DragaliaAPI.Models.Dragalia.Responses;
using DragaliaAPI.Services;
using DragaliaAPI.Models.Database;
using DragaliaAPI.Models.Database.Savefile;

namespace DragaliaAPI.Controllers.Dragalia.Update;

[Route("update/namechange")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class NamechangeController : ControllerBase
{
    private readonly ApiContext _apiContext;
    private readonly ISessionService _sessionService;

    public NamechangeController(ApiContext apiContext, ISessionService sessionService)
    {
        _apiContext = apiContext;
        _sessionService = sessionService;
    }

    [HttpPost]
    public async Task<DragaliaResult> Post(NamechangeRequest request)
    {
        string deviceAccountId = await _sessionService.GetDeviceAccountId_SessionId(
            Request.Headers["SID"]
        );

        DbSavefileUserData userData =
            await _apiContext.SavefileUserData.FindAsync(deviceAccountId)
            ?? throw new ArgumentException("Invalid DeviceAccountId");
        userData.Name = request.name;
        await _apiContext.SaveChangesAsync();

        NamechangeResponse response = new(new NamechangeData(request.name));
        return Ok(response);
    }
}
