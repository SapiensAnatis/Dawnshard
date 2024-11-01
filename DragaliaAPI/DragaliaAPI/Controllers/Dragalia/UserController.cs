using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Tool;
using DragaliaAPI.Infrastructure;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("user")]
public class UserController(
    IUserDataRepository userDataRepository,
    IUpdateDataService updateDataService,
    IBaasApi baasApi,
    ISessionService sessionService
) : DragaliaControllerBase
{
    [HttpPost("linked_n_account")]
    public async Task<DragaliaResult> LinkedNAccount(CancellationToken cancellationToken)
    {
        // This controller is meant to be used to set the 'Link a Nintendo Account' mission as complete
        DbPlayerUserData userData = await userDataRepository.UserData.SingleAsync(
            cancellationToken
        );

        userData.Crystal += 12_000;
        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync(cancellationToken);

        return this.Ok(new UserLinkedNAccountResponse() { UpdateDataList = updateDataList });
    }

    [HttpPost("get_n_account_info")]
    public async Task<DragaliaResult<UserGetNAccountInfoResponse>> GetNAccountInfo(
        [FromHeader(Name = DragaliaHttpConstants.Headers.SessionId)] string sessionId
    )
    {
        string idToken = (await sessionService.LoadSessionSessionId(sessionId)).IdToken;

        return new UserGetNAccountInfoResponse()
        {
            NAccountInfo = new()
            {
                Email = string.Empty, // This being null or empty shows the nickname instead
                Nickname = await baasApi.GetUsername(idToken),
            },
            UpdateDataList = new(),
        };
    }
}
