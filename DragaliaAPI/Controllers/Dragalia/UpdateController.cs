using Microsoft.AspNetCore.Mvc;
using DragaliaAPI.Services;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("update")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class UpdateController : DragaliaControllerBase
{
    private readonly IUserDataRepository userDataRepository;
    private readonly ISessionService _sessionService;

    public UpdateController(IUserDataRepository userDataRepository, ISessionService sessionService)
    {
        this.userDataRepository = userDataRepository;
        _sessionService = sessionService;
    }

    [HttpPost]
    [Route("namechange")]
    public async Task<DragaliaResult> Post(UpdateNamechangeRequest request)
    {
        await userDataRepository.UpdateName(this.DeviceAccountId, request.name);
        await userDataRepository.SaveChangesAsync();

        return this.Ok(new UpdateNamechangeData(request.name));
    }
}
