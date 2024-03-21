using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("user")]
public class UserController : DragaliaControllerBase
{
    private readonly IUserDataRepository userDataRepository;
    private readonly IUpdateDataService updateDataService;
    private readonly IMapper mapper;

    public UserController(
        IUserDataRepository userDataRepository,
        IUpdateDataService updateDataService,
        IMapper mapper
    )
    {
        this.userDataRepository = userDataRepository;
        this.updateDataService = updateDataService;
        this.mapper = mapper;
    }

    [HttpPost("linked_n_account")]
    public async Task<DragaliaResult> LinkedNAccount(CancellationToken cancellationToken)
    {
        // This controller is meant to be used to set the 'Link a Nintendo Account' mission as complete
        DbPlayerUserData userData = await this.userDataRepository.UserData.SingleAsync(
            cancellationToken
        );

        userData.Crystal += 12_000;
        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync(
            cancellationToken
        );

        return this.Ok(new UserLinkedNAccountResponse() { UpdateDataList = updateDataList });
    }

    [HttpPost("get_n_account_info")]
    public DragaliaResult GetNAccountInfo()
    {
        // TODO: Replace this with an API call to BaaS to return actual information
        return this.Ok(
            new UserGetNAccountInfoResponse()
            {
                NAccountInfo = new()
                {
                    Email = "placeholder@email.com",
                    Nickname = "placeholder nickname"
                },
                UpdateDataList = new()
            }
        );
    }
}
