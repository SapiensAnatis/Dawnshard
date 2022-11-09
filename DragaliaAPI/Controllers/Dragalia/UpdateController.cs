using Microsoft.AspNetCore.Mvc;
using DragaliaAPI.Services;
using DragaliaAPI.Models.Requests;
using DragaliaAPI.Models.Responses;
using DragaliaAPI.Database.Repositories;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("update")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class UpdateController : DragaliaController
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

        UpdateNamechangeResponse response = new(new NamechangeData(request.name));

        return this.Ok(response);
    }
}
